document.addEventListener('DOMContentLoaded', function () {
    const hairdresserSelect = document.getElementById('hairdresserSelect');
    const serviceSelect = document.getElementById('serviceSelect');

    const calendarWrapper = document.getElementById('calendarWrapper');
    const timeSlotsContainer = document.getElementById('timeSlotsContainer');
    const timeSlots = document.getElementById('timeSlots');

    const startTimeInput = document.getElementById('startTimeInput');
    const submitBtn = document.getElementById('submitBtn');
    const slotHint = document.getElementById('slotHint');

    let slotsAbort = null;        // AbortController a slot lekéréshez
    let bookedAbort = null;       // AbortController a booked-days lekéréshez
    let lastBookedKey = null;     // hogy ne töltsük le újra feleslegesen
    let calObserver = null;

    function isoDateOnly(d) {
        // Stabil yyyy-mm-dd
        return new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate()))
            .toISOString()
            .split('T')[0];
    }

    function canShowCalendar() {
        return hairdresserSelect.value !== "" && serviceSelect.value !== "";
    }

    // --- Form submit probléma: a naptár gombjai ne legyenek submit gombok ---
    function neutralizeCalendarButtons() {
        calendarWrapper.querySelectorAll('button').forEach(btn => {
            if (!btn.hasAttribute('type') || btn.type === 'submit') btn.type = 'button';
        });
    }

    function startCalendarObserver() {
        if (calObserver) return;
        calObserver = new MutationObserver(() => neutralizeCalendarButtons());
        calObserver.observe(calendarWrapper, { childList: true, subtree: true });
    }

    function resetSlotsUI() {
        timeSlots.innerHTML = '';
        slotHint.classList.add('d-none');
        startTimeInput.value = '';
        submitBtn.disabled = true;
    }

    async function loadSlotsForDate(dateObj) {
        const hairdresserId = hairdresserSelect.value;
        const serviceId = serviceSelect.value;
        console.log(encodeURIComponent(serviceId));
        if (!hairdresserId || !serviceId || !dateObj) return;

        const dateStr = isoDateOnly(dateObj);

        resetSlotsUI();
        timeSlotsContainer.classList.remove('d-none');
        slotHint.classList.remove('d-none');
        timeSlots.innerHTML = '<div class="text-muted">Időpontok betöltése...</div>';

        if (slotsAbort) slotsAbort.abort();
        slotsAbort = new AbortController();

        try {
            const resp = await fetch(
                `/Account/GetAvailableSlots?hairdresserId=${encodeURIComponent(hairdresserId)}&date=${encodeURIComponent(dateStr)}&serviceId=${encodeURIComponent(serviceId)}`,
                { signal: slotsAbort.signal }
            );
            if (!resp.ok) throw new Error(`HTTP ${resp.status}`);

            const slots = await resp.json();
            timeSlots.innerHTML = '';

            if (!slots || slots.length === 0) {
                timeSlots.innerHTML = '<div class="text-danger">Nincs szabad időpont.</div>';
                return;
            }

            for (const slot of slots) {
                const slotDate = new Date(slot);
                const timeText = isNaN(slotDate.getTime())
                    ? String(slot)
                    : slotDate.toLocaleTimeString('hu-HU', { hour: '2-digit', minute: '2-digit' });

                const btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'list-group-item list-group-item-action';
                btn.textContent = timeText;

                btn.addEventListener('click', () => {
                    [...timeSlots.querySelectorAll('button')].forEach(b => b.classList.remove('active'));
                    btn.classList.add('active');

                    startTimeInput.value = slot;
                    submitBtn.disabled = false;
                    slotHint.classList.add('d-none');
                });

                timeSlots.appendChild(btn);
            }
        } catch (err) {
            if (err && err.name === 'AbortError') return;
            console.error(err);
            timeSlots.innerHTML = '<div class="text-danger">Hiba a betöltésnél.</div>';
        }
    }

    // ✅ Foglalt napok: csak fodrász váltáskor / első megjelenéskor töltsük
    async function loadBookedDaysWide(hairdresserId) {
        if (!hairdresserId) return;

        // ugyanaz a fodrász + ugyanaz a range? akkor ne töltsük újra
        const key = `hd:${hairdresserId}`;
        if (lastBookedKey === key) return;
        lastBookedKey = key;

        const start = new Date();
        const end = new Date();
        end.setMonth(end.getMonth() + 6);

        const startStr = isoDateOnly(start);
        const endStr = isoDateOnly(end);

        if (bookedAbort) bookedAbort.abort();
        bookedAbort = new AbortController();

        try {
            const resp = await fetch(
                `/Account/GetBookedDays?hairdresserId=${encodeURIComponent(hairdresserId)}&start=${encodeURIComponent(startStr)}&end=${encodeURIComponent(endStr)}`,
                { signal: bookedAbort.signal }
            );
            if (!resp.ok) throw new Error(`HTTP ${resp.status}`);

            const dates = await resp.json();

            const events = (dates || []).map(date => ({
                start: date,
                end: date,
                name: 'Teljesen foglalt',
                color: '#dc3545',
                allDay: true
            }));

            // ⚠️ Ez a hívás redraw-ol -> ezért CSAK RITKÁN hívjuk (nem hónapváltáskor!)
            window.myCalendar.setEventsData(events);
        } catch (err) {
            if (err && err.name === 'AbortError') return;
            console.error(err);
        }
    }

    function ensureCalendarCreated() {
        if (window.myCalendar) return;

        window.myCalendar = new Calendar({
            container: '#color-calendar',
            calendarSize: 'large',
            theme: 'basic',
            initialSelectedDate: new Date(),

            onSelectedDateChange: (currentDate) => {
                if (!currentDate) return;
                // csak slotot töltünk, events-et nem állítunk itt!
                loadSlotsForDate(currentDate);
            }
            // ⛔️ NINCS onMonthChange -> ettől szűnik meg a visszaugrás + villogás
        });

        neutralizeCalendarButtons();
        startCalendarObserver();
    }

    function refreshUI() {
        resetSlotsUI();

        if (!canShowCalendar()) {
            calendarWrapper.classList.add('d-none');
            timeSlotsContainer.classList.add('d-none');
            return;
        }

        calendarWrapper.classList.remove('d-none');
        timeSlotsContainer.classList.remove('d-none');

        ensureCalendarCreated();

        // booked days egyszer (fodrász váltáskor)
        loadBookedDaysWide(hairdresserSelect.value);

        // default selected nap slotjai azonnal
        const selected = window.myCalendar.getSelectedDate() || new Date();
        loadSlotsForDate(selected);
    }

    // Ha fodrászt vált -> booked days újra kell
    hairdresserSelect.addEventListener('change', () => {
        lastBookedKey = null;
        refreshUI();
    });

    // Ha szolgáltatást vált -> slotok újraszámolódnak, booked days nem kell
    serviceSelect.addEventListener('change', refreshUI);

    refreshUI();
});
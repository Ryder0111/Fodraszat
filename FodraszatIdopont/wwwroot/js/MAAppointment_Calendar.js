document.addEventListener('DOMContentLoaded', function () {
    const hairdresserSelect = document.getElementById('hairdresserSelect');
    const serviceSelect = document.getElementById('serviceSelect');

    const calendarWrapper = document.getElementById('calendarWrapper');
    const timeSlotsContainer = document.getElementById('timeSlotsContainer');
    const timeSlots = document.getElementById('timeSlots');

    const startTimeInput = document.getElementById('startTimeInput');
    const submitBtn = document.getElementById('submitBtn');
    const slotHint = document.getElementById('slotHint');

    let selectedHairdresserId = null;

    function resetSlotsUI() {
        timeSlots.innerHTML = '';
        timeSlotsContainer.classList.add('d-none');
        slotHint.classList.add('d-none');
        startTimeInput.value = '';
        submitBtn.disabled = true;
    }

    function canShowCalendar() {
        return !!hairdresserSelect.value && !!serviceSelect.value;
    }

    function ensureCalendarCreated() {
        if (window.myCalendar) return;

        window.myCalendar = new Calendar({
            container: '#color-calendar',
            calendarSize: 'large',
            initialSelectedDate: new Date(),
            theme: 'basic',
            disableDayClick: false,
            eventsData: []
        });
    }

    // ====== FOGLALT NAPOK BETÖLTÉSE ======
    function loadBookedDays(hairdresserId) {
        const today = new Date();
        const start = today.toISOString().split('T')[0];

        const endDate = new Date(today);
        endDate.setMonth(endDate.getMonth() + 2);
        const end = endDate.toISOString().split('T')[0];

        fetch(`/Account/GetBookedDays?hairdresserId=${encodeURIComponent(hairdresserId)}&start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`)
            .then(r => r.json())
            .then(dates => {
                const events = (dates || []).map(date => ({
                    start: date,
                    end: date,
                    name: 'Teljesen foglalt',
                    color: '#dc3545',
                    allDay: true
                }));

                window.myCalendar.setEventsData(events);
            })
            .catch(err => console.error(err));
    }

    // ====== NAPTÁR MEGJELENÍTÉS LOGIKA ======
    function refreshUI() {
        resetSlotsUI();

        if (!canShowCalendar()) {
            calendarWrapper.classList.add('d-none');
            return;
        }

        calendarWrapper.classList.remove('d-none');
        ensureCalendarCreated();
        loadBookedDays(hairdresserSelect.value);
    }

    hairdresserSelect.addEventListener('change', function () {
        selectedHairdresserId = this.value || null;
        refreshUI();
    });

    serviceSelect.addEventListener('change', function () {
        refreshUI();
    });

    // ====== NAPRA KATTINTÁS → SLOTOK LEKÉRÉSE ======
    document.addEventListener('click', function (e) {
        const dayElement = e.target.closest('.calendar__day');
        if (!dayElement) return;

        const hairdresserId = hairdresserSelect.value;
        const serviceId = serviceSelect.value;

        if (!hairdresserId || !serviceId) return;

        // Color Calendar aria-label pl.: "Wednesday, March 4, 2026"
        const ariaLabel = dayElement.getAttribute('aria-label');
        if (!ariaLabel) return;

        const clickedDate = new Date(ariaLabel);
        if (isNaN(clickedDate.getTime())) return;

        const formattedDate = clickedDate.toISOString().split('T')[0];

        resetSlotsUI();
        timeSlotsContainer.classList.remove('d-none');
        slotHint.classList.remove('d-none');

        fetch(`/Account/GetAvailableSlots?hairdresserId=${encodeURIComponent(hairdresserId)}&date=${encodeURIComponent(formattedDate)}&serviceId=${encodeURIComponent(serviceId)}`)
            .then(r => r.json())
            .then(slots => {
                timeSlots.innerHTML = '';

                if (!slots || slots.length === 0) {
                    timeSlots.innerHTML = '<div class="text-danger">Nincs szabad időpont.</div>';
                    return;
                }

                slots.forEach(slot => {
                    const slotDate = new Date(slot);
                    const timeText = isNaN(slotDate.getTime())
                        ? String(slot)
                        : slotDate.toLocaleTimeString('hu-HU', { hour: '2-digit', minute: '2-digit' });

                    const btn = document.createElement('button');
                    btn.type = 'button';
                    btn.className = 'list-group-item list-group-item-action';
                    btn.textContent = timeText;

                    btn.addEventListener('click', () => {
                        // aktív kijelölés UI
                        [...timeSlots.querySelectorAll('button')].forEach(b => b.classList.remove('active'));
                        btn.classList.add('active');

                        // a hidden input-ba a backend által adott datetime stringet tesszük
                        startTimeInput.value = slot;

                        submitBtn.disabled = false;
                        slotHint.classList.add('d-none');
                    });

                    timeSlots.appendChild(btn);
                });
            })
            .catch(err => console.error(err));
    });

    // initial
    refreshUI();
});
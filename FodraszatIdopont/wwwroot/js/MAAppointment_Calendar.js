document.addEventListener('DOMContentLoaded', function () {

    const hairdresserSelect = document.getElementById('hairdresserSelect');
    const serviceSelect = document.getElementById('serviceSelect');
    const calendarWrapper = document.getElementById('calendarWrapper');
    const calendarContainer = document.getElementById('color-calendar');
    const timeSlotsContainer = document.getElementById('timeSlotsContainer');
    const timeSlots = document.getElementById('timeSlots');

    let selectedHairdresserId = null;

    // ===== FODRÁSZ VÁLASZTÁS =====
    hairdresserSelect.addEventListener('change', function () {

        selectedHairdresserId = this.value;

        if (!selectedHairdresserId) {
            calendarWrapper.classList.add('d-none');
            return;
        }

        calendarWrapper.classList.remove('d-none');

        if (!window.myCalendar) {
            window.myCalendar = new Calendar({
                container: '#color-calendar',
                calendarSize: 'large',
                initialSelectedDate: new Date(),
                theme: 'basic',
                primaryColor: '#0d6efd',
                headerColor: '#343a40',
                headerBackgroundColor: '#f8f9fa',
                weekdaysColor: '#6c757d',
                disableDayClick: false,
                eventsData: []
            });
        }

        loadBookedDays(selectedHairdresserId);
    });

    // ===== FOGLALT NAPOK =====
    function loadBookedDays(hairdresserId) {

        const today = new Date();
        const start = today.toISOString().split('T')[0];

        const endDate = new Date(today);
        endDate.setMonth(endDate.getMonth() + 2);
        const end = endDate.toISOString().split('T')[0];

        fetch(`/Account/GetBookedDays?hairdresserId=${hairdresserId}&start=${start}&end=${end}`)
            .then(r => r.json())
            .then(dates => {

                const events = dates.map(date => ({
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

    // ===== NAPRA KATTINTÁS =====
    document.addEventListener('click', function (e) {

        const dayElement = e.target.closest('.calendar__day');
        if (!dayElement) return;

        if (!selectedHairdresserId) return;

        const serviceId = serviceSelect.value;
        if (!serviceId) {
            alert("Válassz szolgáltatást!");
            return;
        }

        // 📌 Dátum kinyerése aria-label-ből
        // pl: "Wednesday, March 4, 2026"
        const ariaLabel = dayElement.getAttribute('aria-label');
        const clickedDate = new Date(ariaLabel);

        const formattedDate = clickedDate.toISOString().split('T')[0];

        const option = serviceSelect.querySelector(`option[value="${serviceId}"]`);
        const duration = option ? parseInt(option.dataset.duration) : 60;

        fetch(`/Account/GetAvailableSlots?hairdresserId=${selectedHairdresserId}&date=${formattedDate}&serviceDuration=${duration}`)
            .then(r => r.json())
            .then(slots => {

                timeSlots.innerHTML = '';

                if (!slots || slots.length === 0) {
                    timeSlots.innerHTML = '<p class="text-danger">Nincs szabad időpont.</p>';
                } else {
                    slots.forEach(slot => {
                        const time = new Date(slot)
                            .toLocaleTimeString('hu-HU', { hour: '2-digit', minute: '2-digit' });

                        timeSlots.innerHTML += `
                            <button type="button" 
                                    class="list-group-item list-group-item-action">
                                ${time}
                            </button>`;
                    });
                }

                timeSlotsContainer.classList.remove('d-none');
            })
            .catch(err => console.error(err));
    });

});
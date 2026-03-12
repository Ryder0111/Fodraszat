// ==========================================
// 3. OKOS NAPTÁR LOGIKA
// ==========================================
document.addEventListener("DOMContentLoaded", function () {
    let currentDate = new Date();
    let currentMonth = currentDate.getMonth();
    let currentYear = currentDate.getFullYear();

    const calendarSection = document.getElementById('calendarSection');
    const calendarBody = document.getElementById('calendarBody');
    const monthYearDisplay = document.getElementById('currentMonthYear');
    const messageDisplay = document.getElementById('calendarMessage');

    const hairdresserSelect = document.getElementById('hairdresserSelect');
    const serviceSelect = document.getElementById('serviceSelect');

    if (!calendarBody || !hairdresserSelect || !serviceSelect) return;

    function checkSelectionsAndRender() {
        if (hairdresserSelect.value !== "" && serviceSelect.value !== "") {
            calendarSection.style.display = 'block';
            renderCalendar(currentYear, currentMonth);
        } else {
            calendarSection.style.display = 'none';
        }
    }

    hairdresserSelect.addEventListener('change', checkSelectionsAndRender);
    serviceSelect.addEventListener('change', checkSelectionsAndRender);

    function renderCalendar(year, month) {
        calendarBody.innerHTML = '';
        messageDisplay.innerText = '';
        removeSlotsRow();

        const monthNames = ["Január", "Február", "Március", "Április", "Május", "Június", "Július", "Augusztus", "Szeptember", "Október", "November", "December"];
        monthYearDisplay.innerText = `${year}. ${monthNames[month]}`;

        let firstDay = new Date(year, month, 1).getDay();
        firstDay = firstDay === 0 ? 6 : firstDay - 1;

        let daysInMonth = new Date(year, month + 1, 0).getDate();

        let date = 1;
        for (let i = 0; i < 6; i++) {
            let row = document.createElement('tr');

            for (let j = 0; j < 7; j++) {
                let cell = document.createElement('td');

                if (i === 0 && j < firstDay) {
                    cell.classList.add('empty-day');
                } else if (date > daysInMonth) {
                    cell.classList.add('empty-day');
                } else {
                    let cellDate = `${year}-${String(month + 1).padStart(2, '0')}-${String(date).padStart(2, '0')}`;
                    cell.dataset.date = cellDate;

                    let span = document.createElement('span');
                    span.classList.add('day-number');
                    span.innerText = date;
                    cell.appendChild(span);

                    let today = new Date();
                    if (date === today.getDate() && year === today.getFullYear() && month === today.getMonth()) {
                        cell.classList.add('today');
                    }

                    if (j === 6) {
                        cell.classList.add('sunday-day');
                        cell.dataset.isSunday = "true";
                    }

                    cell.addEventListener('click', handleDayClick);
                    date++;
                }
                row.appendChild(cell);
            }
            calendarBody.appendChild(row);
            if (date > daysInMonth) break;
        }

        fetchBookedDays(year, month);
    }

    async function fetchBookedDays(year, month) {
        let hairdresserId = hairdresserSelect.value;
        if (!hairdresserId) return;

        let start = `${year}-${String(month + 1).padStart(2, '0')}-01`;
        let end = `${year}-${String(month + 1).padStart(2, '0')}-${new Date(year, month + 1, 0).getDate()}`;

        try {
            // JAVÍTVA: Fetch az Account controllerből
            let response = await fetch(`/Account/GetBookedDays?hairdresserId=${hairdresserId}&start=${start}&end=${end}`);
            if (response.ok) {
                let bookedDates = await response.json();

                bookedDates.forEach(bDate => {
                    let formattedDate = bDate.split('T')[0];
                    let cell = document.querySelector(`td[data-date="${formattedDate}"]`);
                    if (cell) {
                        cell.classList.add('booked-day');
                        cell.dataset.isBooked = "true";
                    }
                });
            }
        } catch (error) {
            console.error("Hiba a foglalt napok lekérésekor:", error);
        }
    }

    async function handleDayClick(event) {
        let cell = event.currentTarget;
        let date = cell.dataset.date;
        let isSunday = cell.dataset.isSunday === "true";
        let isBooked = cell.dataset.isBooked === "true";
        let tr = cell.parentNode;

        removeSlotsRow();
        messageDisplay.innerText = '';

        if (isSunday) {
            messageDisplay.innerText = "Vasárnap zárva vagyunk";
            return;
        }

        if (isBooked) {
            messageDisplay.innerText = "Erre a napra már nincs időpont";
            return;
        }

        let slotsRow = document.createElement('tr');
        slotsRow.classList.add('slots-row');
        slotsRow.id = 'activeSlotsRow';

        let slotsCell = document.createElement('td');
        slotsCell.colSpan = 7;
        slotsCell.innerHTML = `<div class="slots-container" id="slotsContainer">Betöltés... <span class="spinner-border spinner-border-sm"></span></div>`;

        slotsRow.appendChild(slotsCell);
        tr.parentNode.insertBefore(slotsRow, tr.nextSibling);

        let hairdresserId = hairdresserSelect.value;
        let serviceId = serviceSelect.value;

        try {
            // JAVÍTVA: Fetch az Account controllerből
            let response = await fetch(`/Account/GetAvailableSlots?hairdresserId=${hairdresserId}&date=${date}&serviceId=${serviceId}`);
            let container = document.getElementById('slotsContainer');

            if (response.ok) {
                let slots = await response.json();
                container.innerHTML = '';

                if (slots && slots.length > 0) {
                    slots.forEach(slot => {
                        let btn = document.createElement('button');
                        btn.className = 'slot-btn';
                        btn.innerText = slot.time || slot;

                        btn.onclick = (e) => {
                            e.preventDefault();

                            // JAVÍTVA: C#-kompatibilis dátum formátum szóközökkel!
                            const timeString = btn.innerText.trim();
                            document.getElementById('startTimeInput').value = `${date} ${timeString}:00`;

                            const submitBtn = document.getElementById('submitBtn');
                            submitBtn.disabled = false;
                            submitBtn.style.opacity = "1";
                            submitBtn.style.cursor = "pointer";
                            document.getElementById('submitHelperText').style.display = "none";

                            document.querySelectorAll('.slot-btn').forEach(b => {
                                b.style.backgroundColor = 'var(--accent-color)';
                                b.style.transform = 'scale(1)';
                            });
                            btn.style.backgroundColor = 'var(--primary-color)';
                            btn.style.transform = 'scale(1.1)';
                        };
                        container.appendChild(btn);
                    });
                } else {
                    container.innerHTML = '<p class="text-danger m-0">Nincs elérhető időpont erre a napra.</p>';
                }
            } else {
                container.innerHTML = '<p class="text-danger m-0">Hiba a szerver kommunikációban.</p>';
            }
        } catch (error) {
            console.error("Hiba az időpontok lekérésekor:", error);
            document.getElementById('slotsContainer').innerHTML = '<p class="text-danger m-0">Hiba történt a betöltés során.</p>';
        }
    }

    function removeSlotsRow() {
        let existingRow = document.getElementById('activeSlotsRow');
        if (existingRow) {
            existingRow.remove();
        }
    }

    const btnPrevMonth = document.getElementById('btnPrevMonth');
    const btnNextMonth = document.getElementById('btnNextMonth');

    if (btnPrevMonth && btnNextMonth) {
        btnPrevMonth.addEventListener('click', () => {
            removeSlotsRow();
            currentMonth--;
            if (currentMonth < 0) {
                currentMonth = 11;
                currentYear--;
            }
            renderCalendar(currentYear, currentMonth);
        });

        btnNextMonth.addEventListener('click', () => {
            removeSlotsRow();
            currentMonth++;
            if (currentMonth > 11) {
                currentMonth = 0;
                currentYear++;
            }
            renderCalendar(currentYear, currentMonth);
        });
    }

    checkSelectionsAndRender();
});
// ==========================================
// 1. SMART NAVBAR LOGIKA
// ==========================================
const navbar = document.querySelector(".navbar");
let lastScroll = window.scrollY; // Kezdőérték beállítása az aktuális pozícióra
let navbarY = 0; // A navbar jelenlegi függőleges eltolása (kezdetben 0, azaz teljesen látszik)

if (navbar) {
    const navbarHeight = navbar.offsetHeight;

    window.addEventListener("scroll", () => {
        let currentScroll = window.scrollY;
        let diff = currentScroll - lastScroll;

        navbarY -= diff;

        if (navbarY > 0) {
            navbarY = 0;
        }
        else if (navbarY < -navbarHeight) {
            navbarY = -navbarHeight;
        }

        navbar.style.transform = `translateY(${navbarY}px)`;
        lastScroll = currentScroll;
    });
}

// ==========================================
// 2. OFFCANVAS / NAVBAR PADDING LOGIKA
// ==========================================
function setOffcanvasOffset() {
    const mainNavbar = document.getElementById("mainNavbar");
    if (mainNavbar) {
        const height = mainNavbar.offsetHeight;
        document.documentElement.style.setProperty("--navbar-height", height + "px");
    }
}

window.addEventListener("load", setOffcanvasOffset);
window.addEventListener("resize", setOffcanvasOffset);


// ==========================================
// 4. JELSZÓ LOGIKA
// ==========================================
document.addEventListener("DOMContentLoaded", function () {

    const toggleIcons = document.querySelectorAll(".fa-eye");

    toggleIcons.forEach(function (icon) {

        icon.addEventListener("click", function () {

            //Megkeressük a KATTINTOTT szemhez (this) tartozó inputot.
            // Felmegyünk az 'input-group' dobozig, majd ott megkeressük a '.password' mezőt.
            const input = this.closest('.input-group').querySelector('.password');

            const type = input.getAttribute("type") === "password" ? "text" : "password";
            input.setAttribute("type", type);

            this.classList.toggle('fa-eye');
            this.classList.toggle('fa-eye-slash');
        });
    });
});

$(document).ready(function () {
    // Csak akkor maszkolunk, ha van ilyen elem az oldalon
    if ($('#phoneInput').length > 0) {
        $('#phoneInput').mask('+36 (00) 000-0000');
    }
});
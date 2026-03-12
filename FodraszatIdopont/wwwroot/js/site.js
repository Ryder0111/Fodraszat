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
// 4. RECAPTCHA LOGIKA
// ==========================================
if (typeof grecaptcha !== 'undefined') {
    grecaptcha.ready(function () {
        const form = document.querySelector("form");
        if (form) { // Csak akkor kössük rá a formra, ha van is form az oldalon!
            form.addEventListener("submit", function (e) {
                e.preventDefault();

                grecaptcha.execute('SITE_KEY', { action: 'login' })
                    .then(function (token) {
                        const tokenInput = document.getElementById("recaptchaToken");
                        if (tokenInput) {
                            tokenInput.value = token;
                        }
                        e.target.submit();
                    });
            });
        }
    });
}
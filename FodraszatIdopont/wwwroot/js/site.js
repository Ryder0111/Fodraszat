const navbar = document.querySelector(".navbar");
let lastScroll = window.scrollY; // Kezdőérték beállítása az aktuális pozícióra
let navbarY = 0; // A navbar jelenlegi függőleges eltolása (kezdetben 0, azaz teljesen látszik)

// Lekérjük a navbar magasságát pixelben, hogy tudjuk, mi a maximum, ameddig elrejthetjük
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

window.addEventListener("load", updatePadding);
window.addEventListener("resize", updatePadding);

function setOffcanvasOffset() {
    const navbar = document.getElementById("mainNavbar");
    const height = navbar.offsetHeight;

    document.documentElement.style.setProperty("--navbar-height", height + "px");
}

window.addEventListener("load", setOffcanvasOffset);
window.addEventListener("resize", setOffcanvasOffset);


grecaptcha.ready(function () {

    document.querySelector("form").addEventListener("submit", function (e) {

        e.preventDefault();

        grecaptcha.execute('SITE_KEY', { action: 'login' })
            .then(function (token) {

                document.getElementById("recaptchaToken").value = token;

                e.target.submit();

            });

    });

});
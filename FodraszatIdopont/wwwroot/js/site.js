const navbar = document.getElementById("mainNavbar");
const content = document.getElementById("pageContent");
let lastScroll = window.pageYOffset;
let offset = 0;
const navbarHeight = navbar.offsetHeight;

window.addEventListener("scroll", () => {
    const currentScroll = window.pageYOffset;
    const delta = currentScroll - lastScroll;

    // lefelé: elrejt
    if (delta > 0) {
        offset = Math.min(offset + delta, navbarHeight);
    }
    // felfelé: visszahoz
    else {
        offset = Math.max(offset + delta, 0);
    }

    navbar.style.transform = `translateY(${-offset}px)`;
    lastScroll = currentScroll;
});



function updatePadding() {
    content.style.paddingTop = navbar.offsetHeight + "px";
}

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
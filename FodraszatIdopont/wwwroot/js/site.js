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

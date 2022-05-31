// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const navbar = document.querySelector('.header-bottom')
window.addEventListener('scroll', e => {
    console.dir(window.pageYOffset);
    if (window.pageYOffset > 50) {
        navbar.style.position = "fixed";
        navbar.style.right = "0";
        navbar.style.left = "0";
        navbar.style.top = "0";

    } else {
        navbar.style.position = "";
    }
})
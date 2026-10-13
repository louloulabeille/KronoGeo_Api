// wwwroot/app.js (ou dans App.razor / index.html)
// -- IA claude 13/10/2026
// -- ferme tous les accordeons de boostrap
window.fermerAccordeon = (id) => {
    document.querySelectorAll(`#${id} .accordion-collapse.show`).forEach(el => {
        bootstrap.Collapse.getOrCreateInstance(el, { toggle: false }).hide();
    });
};
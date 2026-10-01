const positions = {
    "top-center": "top-0 start-50 translate-middle-x",
    "top-right": "top-0 end-0",
    "top-left": "top-0 start-0",
    "bottom-center": "bottom-0 start-50 translate-middle-x",
    "bottom-right": "bottom-0 end-0",
    "bottom-left": "bottom-0 start-0"
};

window.afficherToastDynamique = (message, type = "primary", delay = 4000, position = "top-center") => {
    const containerId = `toast-container-${position}`;
    let container = document.getElementById(containerId);
    if (!container) {
        container = document.createElement("div");
        container.id = containerId;
        container.className = `toast-container position-fixed p-3 ${positions[position] ?? positions["top-center"]}`;
        document.body.appendChild(container);
    }    
    
   /* let container = document.getElementById("toast-container");
    if (!container) {
        container = document.createElement("div");
        container.id = "toast-container";
        container.className = "toast-container position-fixed bottom-0 end-0 p-3";
        document.body.appendChild(container);
    }*/

    const toastEl = document.createElement("div");
    toastEl.className = `toast align-items-center text-bg-${type} border-0`;
    toastEl.setAttribute("role", "alert");
    toastEl.setAttribute("aria-live", "assertive");
    toastEl.setAttribute("aria-atomic", "true");

    const closeClass = (type === "warning" || type === "light" || type === "info") ? "" : "btn-close-white";
    toastEl.innerHTML = `
        <div class="d-flex">
            <div class="toast-body"></div>
            <button type="button" class="btn-close ${closeClass} me-2 m-auto"
                    data-bs-dismiss="toast" aria-label="Fermer"></button>
        </div>`;
    toastEl.querySelector(".toast-body").textContent = message;
    container.appendChild(toastEl);

    toastEl.addEventListener("hidden.bs.toast", () => toastEl.remove());
    new bootstrap.Toast(toastEl, { delay }).show();
};
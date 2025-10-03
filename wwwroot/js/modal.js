const modal = document.getElementById("confirmDeleteModal");
modal.addEventListener("show.bs.modal", function (event) {
  let button = event.relatedTarget;
  let entityId = button.getAttribute("data-id");
  let descripcion = button.getAttribute("data-descripcion");
  let baseUrl = button.getAttribute("data-url");

  let message = modal.querySelector("#modalMessage");
  message.textContent = `¿Seguro que deseas eliminar: ${descripcion}?`;

  let hiddenInput = modal.querySelector("#entityId");
  hiddenInput.value = entityId;

  let form = modal.querySelector("#deleteForm");
  form.action = baseUrl + entityId;
});

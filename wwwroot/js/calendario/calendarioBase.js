export function inicializarCalendarioBase(selector = ".flatpick") {
  const inputsFecha = document.querySelectorAll(selector);

  if (!inputsFecha.length) return [];

  const pickers = [];

  inputsFecha.forEach((input) => {
    const picker = flatpickr(input, {
      dateFormat: "Y-m-d",
      locale: "es",
    });
    pickers.push(picker);
  });

  return pickers;
}

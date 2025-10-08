import { inicializarCalendarioBase } from "/js/calendario/calendarioBase.js";

document.addEventListener("DOMContentLoaded", () => {
  const selectInmueble = document.getElementById("inmueble");
  const fechaInicio = document.getElementById("fechaInicio");
  const fechaFin = document.getElementById("fechaFin");

  if (!fechaInicio || !fechaFin || !selectInmueble) return;

  const [pickerInicio, pickerFin] = inicializarCalendarioBase(
    "#fechaInicio, #fechaFin"
  );

  pickerInicio.config.onChange.push((selectedDates) => {
    if (selectedDates.length) {
      pickerFin.set("minDate", selectedDates[0]);
    }
  });

  selectInmueble.addEventListener("change", async () => {
    const id = selectInmueble.value;
    if (!id) return;

    try {
      const res = await fetch(
        `/Contrato/ObtenerFechasOcupadas?inmuebleId=${id}`
      );
      const fechasOcupadas = await res.json();

      //por defecto flatpickr trabaja con objetos Date("YYYY-MM-DD") que se interpreta en UTC y puede variar segun zona horaria del cliente.
      // solucion: parsear manualmente separando componentes y creando Date(y, m-1, d) que respeta zona local
      const parseFechaLocal = (val) => {
        const [y, m, d] = val.split("-").map(Number);
        return new Date(y, m - 1, d);
      };

      const fechasDeshabilitadas = fechasOcupadas.map((f) => ({
        from: parseFechaLocal(f.inicio),
        to: parseFechaLocal(f.fin),
      }));

      pickerInicio.set("disable", fechasDeshabilitadas);
      pickerFin.set("disable", fechasDeshabilitadas);

      pickerInicio.clear();
      pickerFin.clear();

      pickerInicio.setDate(
        fechaInicio.value ? parseFechaLocal(fechaInicio.value) : new Date()
      );
      pickerFin.setDate(
        fechaFin.value ? parseFechaLocal(fechaFin.value) : new Date()
      );
    } catch (err) {
      console.error("Error al obtener fechas ocupadas:", err);
    }
  });
});

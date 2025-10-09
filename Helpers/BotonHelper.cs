using Microsoft.AspNetCore.Http;

namespace inmobiliaria_mvc.Helpers
{
    public static class BotonHelper
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        private static bool EsAdministrador()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            return user != null && user.IsInRole("Administrador");
        }

        public static string BotonEliminar(string controller, int id, string descripcion)
        {
            var safeDescripcion = System.Net.WebUtility.HtmlEncode(descripcion);

            return EsAdministrador()
                ? $@"
                <a class='icon-btn' 
                data-bs-toggle='modal' 
                data-bs-target='#confirmDeleteModal' 
                data-url='/{controller}/Delete/' 
                data-id='{id}' 
                data-descripcion='{safeDescripcion}' 
                title='Eliminar'>
                <i class='fa-regular fa-trash-can'></i>
                </a>"
                : "";
        }

        public static string BotonEditar(string controller, int id)
        {
            return EsAdministrador()
                ? $"<a href='/{controller}/Edit/{id}' class='icon-btn' title='Editar'><i class='fa-regular fa-pen-to-square'></i></a>"
                : "";
        }

        public static string BotonDetalles(string controller, int id)
        {
            return $"<a href='/{controller}/Details/{id}' class='icon-btn' title='Detalles'><i class='fa-regular fa-eye'></i></a>";
        }

        public static string BotonRenovar(string controller, int id)
        {
            return $"<a href='/{controller}/Renovar/{id}' class='icon-btn' title='Renovar'><i class='fa-solid fa-arrows-rotate'></i></a>";
        }

    }
}

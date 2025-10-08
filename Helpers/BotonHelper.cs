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
                    <a class='btn btn-danger btn-sm' 
                       data-bs-toggle='modal' 
                       data-bs-target='#confirmDeleteModal' 
                       data-url='/{controller}/Delete/' 
                       data-id='{id}' 
                       data-descripcion='{safeDescripcion}'>
                       Eliminar
                    </a>"
                : "";
        }

        public static string BotonEditar(string controller, int id)
        {
            return EsAdministrador()
                ? $"<a href='/{controller}/Edit/{id}' class='btn btn-warning btn-sm'>Editar</a>"
                : "";
        }

        public static string BotonDetalles(string controller, int id)
        {
            return $"<a href='/{controller}/Details/{id}' class='btn btn-info btn-sm'>Detalles</a>";
        }

        public static string BotonRenovar(string controller, int id)
        {
            return $"<a href='/{controller}/Renovar/{id}' class='btn btn-success btn-sm'>Renovar</a>";
        }
    }
}

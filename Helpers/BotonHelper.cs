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
    }
}

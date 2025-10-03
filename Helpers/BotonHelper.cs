namespace inmobiliaria_mvc.Helpers
{
    public static class BotonHelper
    {
        public static string BotonEliminar(string controller, int id, string descripcion)
        {
            var safeDescripcion = System.Net.WebUtility.HtmlEncode(descripcion);

            return $@"
                <a class='btn btn-danger btn-sm' 
                   data-bs-toggle='modal' 
                   data-bs-target='#confirmDeleteModal' 
                   data-url='/{controller}/Delete/' 
                   data-id='{id}' 
                   data-descripcion='{safeDescripcion}'>
                   Eliminar
                </a>";
        }
    }
}
using inmobiliaria_mvc.Helpers;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria_mvc.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly IRepositoryAuditoria _repo;
        private readonly IRepositoryUsuario _repoUsuario;

        public AuditoriaController(IRepositoryAuditoria repo, IRepositoryUsuario repoUsuario)
        {
            _repo = repo;
            _repoUsuario = repoUsuario;
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Filtrar(int page = 1, int pageSize = 10)
        {
            var tabla = ConstruirTabla(page, pageSize);
            return PartialView("_Tabla", tabla);
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var tabla = ConstruirTabla(page, pageSize);
            if (TempData.ContainsKey("Id")) ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];

            return View(tabla);
        }

        [Authorize(Policy = "Administrador")]
        private TablaViewModel<Auditoria> ConstruirTabla(int page, int pageSize)
        {
            var auditoria = _repo.Paginar(page, pageSize);

            var tabla = TablaHelper.MapToTablaViewModel(auditoria, a =>
            {
                var usuario = _repoUsuario.ObtenerPorId(a.UsuarioId);
                var avatar = string.IsNullOrEmpty(usuario?.Avatar) ? "/img/default-user-avatar.png" : usuario.Avatar;

                string usuarioConAvatar;
                if (usuario != null)
                {
                    usuarioConAvatar = $@"
                        <div style='display:flex; align-items:center; gap:8px;'>
                            <img src='{avatar}' alt='{usuario.Nombre} {usuario.Apellido}' width='32' height='32' style='border-radius:50%; object-fit:cover;' />
                            <span>{usuario.Nombre} {usuario.Apellido}</span>
                        </div>";
                }
                else
                {
                    usuarioConAvatar = $"Usuario #{a.UsuarioId}";
                }

                return new Dictionary<string, object>
                {
                    {"Código", a.Id},
                    {"Objeto", a.Entidad},
                    {"Código del objeto", a.EntidadId},
                    {"Acción", a.Accion},
                    {"Usuario", usuarioConAvatar}
                };
            });

            return tabla;
        }
    }
}
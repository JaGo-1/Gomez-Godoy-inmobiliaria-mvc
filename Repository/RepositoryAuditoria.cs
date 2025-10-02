using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace inmobiliaria_mvc.Repository;

public class RepositoryAuditoria : RepositorioBase, IRepositoryAuditoria
{
    public RepositoryAuditoria(IConfiguration configuration) : base(configuration)
    {
    }

    public int Alta(Auditoria p)
    {
        try
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var sql = @"INSERT INTO Auditoria (entidad, entidad_id, accion, usuario_id, fecha, datos_anteriores, datos_nuevos)
                VALUES (@entidad, @entidadId, @accion, @usuarioId, @fecha, @datosAnteriores, @datosNuevos)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@entidad", p.Entidad);
                    cmd.Parameters.AddWithValue("@entidadId", p.EntidadId);
                    cmd.Parameters.AddWithValue("@accion", p.Accion);
                    cmd.Parameters.AddWithValue("@usuarioId", p.UsuarioId);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@datosanteriores", p.DatosAnteriores == null ? DBNull.Value : (object)p.DatosAnteriores).NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
                    cmd.Parameters.AddWithValue("@datosnuevos", p.DatosNuevos == null ? DBNull.Value : (object)p.DatosNuevos).NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;


                    var id = cmd.ExecuteScalar();
                    res = id != null ? Convert.ToInt32(id) : -1;

                    conn.Close();
                }
            }
            return res;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public int Baja(int id)
    {
        throw new NotImplementedException();
    }

    public int Modificacion(Auditoria p)
    {
        throw new NotImplementedException();
    }

    public Auditoria ObtenerPorId(int id)
    {
        throw new NotImplementedException();
    }

    public IList<Auditoria> ObtenerTodos()
    {
        throw new NotImplementedException();
    }

    public PagedResult<Auditoria> Paginar(int pagina, int tamPagina)
    {
        var res = new List<Auditoria>();
        int totalItems = 0;

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            string countSql = "SELECT COUNT (*) FROM auditoria WHERE estado = true";
            using (var countCmd = new NpgsqlCommand(countSql, conn))
            {
                totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
            }

            string sql = "SELECT id, entidad, entidad_id, accion, usuario_id, fecha FROM auditoria WHERE Estado = true ORDER BY id LIMIT @tamPagina OFFSET (@pagina - 1) * @tamPagina";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("pagina", pagina);
                cmd.Parameters.AddWithValue("tamPagina", tamPagina);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Auditoria
                        {
                            Id = reader.GetInt32(0),
                            Entidad = reader.GetString(1),
                            EntidadId = reader.GetInt32(2),
                            Accion = reader.GetString(3),
                            UsuarioId = reader.GetInt32(4),
                            Fecha = reader.GetDateTime(5)
                        });
                    }
                }
            }
            conn.Close();
        }

        return new PagedResult<Auditoria>
        {
            Items = res,
            TotalItems = totalItems,
            PageNumber = pagina,
            PageSize = tamPagina
        };
    }
}
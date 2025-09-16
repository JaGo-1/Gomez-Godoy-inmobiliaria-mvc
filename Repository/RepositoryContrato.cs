
using System.Data.SqlTypes;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;
using Npgsql;
using Xunit.Sdk;

namespace inmobiliaria_mvc.Repository
{
    public class RepositoryContrato : RepositorioBase, IRepositoryContrato
    {
        public RepositoryContrato(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Contrato p)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO contrato (id_inmueble, id_inquilino, fecha_inicio, fecha_fin, monto, estado) VALUES (@id_inmueble, @id_inquilino, @fecha_inicio, @fecha_fin, @monto, @estado) RETURNING id;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id_inmueble", p.IdInmueble);
                    cmd.Parameters.AddWithValue("@id_inquilino", p.IdInquilino);
                    cmd.Parameters.AddWithValue("@fecha_inicio", p.Fecha_inicio);
                    cmd.Parameters.AddWithValue("@fecha_fin", p.Fecha_fin);
                    cmd.Parameters.AddWithValue("@monto", p.Monto);
                    cmd.Parameters.AddWithValue("@estado", true);

                    conn.Open();
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        res = Convert.ToInt32(id);
                    }
                    conn.Close();
                }

            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = "UPDATE contrato SET estado = false WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int Modificacion(Contrato p)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                var sql = @"UPDATE contrato SET id_inmueble = @id_inmueble, id_inquilino = @id_inquilino, fecha_inicio = @fecha_inicio, fecha_fin = @fecha_fin, monto = @monto WHERE id = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", p.Id);
                    cmd.Parameters.AddWithValue("@id_inmueble", p.IdInmueble);
                    cmd.Parameters.AddWithValue("@id_inquilino", p.IdInquilino);
                    cmd.Parameters.AddWithValue("@fecha_inicio", p.Fecha_inicio);
                    cmd.Parameters.AddWithValue("@fecha_fin", p.Fecha_fin);
                    cmd.Parameters.AddWithValue("@monto", p.Monto);

                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public Contrato? ObtenerPorId(int id)
        {
            Contrato? contrato = null;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = @"SELECT id, id_inmueble, id_inquilino, fecha_inicio, fecha_fin, monto FROM contrato WHERE id = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32(0),
                                IdInmueble = reader.GetInt32(1),
                                IdInquilino = reader.GetInt32(2),
                                Fecha_inicio = reader.GetDateTime(3),
                                Fecha_fin = reader.GetDateTime(4),
                                Monto = reader.GetInt32(5)
                            };
                        }
                    }
                    conn.Close();
                }
            }
            return contrato;
        }

        public IList<Contrato> ObtenerTodos()
        {
            var res = new List<Contrato>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = @"SELECT id, id_inmueble, id_inquilino, fecha_inicio, fecha_fin, monto
                FROM contrato WHERE estado = true;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        var repoInquilino = new RepositoryInquilino(configuration);
                        var repoInmueble = new RepositoryInmueble(configuration);

                        while (reader.Read())
                        {
                            res.Add(new Contrato
                            {
                                Id = reader.GetInt32(0),
                                Inmueble = repoInmueble.ObtenerPorId(reader.GetInt32(1)),
                                Inquilino = repoInquilino.ObtenerPorId(reader.GetInt32(2)),
                                Fecha_inicio = reader.GetDateTime(3),
                                Fecha_fin = reader.GetDateTime(4),
                                Monto = reader.GetInt32(5),
                            });
                        }
                    }
                    conn.Close();
                }

            }
            return res;
        }

        //Verificar que no se solapen fechas
        public bool ExisteSolapado(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int? contratoId = null)
        {
            using var conn = new NpgsqlConnection(connectionString);

            string sql = @"SELECT COUNT(*) FROM contrato WHERE id_inmueble = @id_inmueble AND estado = true AND (fecha_inicio, fecha_fin) OVERLAPS (@fecha_inicio, @fecha_fin)";

            if (contratoId.HasValue)
            {
                sql += " AND id <> @id";
            }

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id_inmueble", inmuebleId);
            cmd.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
            cmd.Parameters.AddWithValue("@fecha_fin", fechaFin);

            if (contratoId.HasValue)
            {
                cmd.Parameters.AddWithValue("@id", contratoId.Value);
            }

            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public List<Contrato> ObtenerContratosPorInmueble(int idInmueble)
        {
            try
            {
                var res = new List<Contrato>();
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    string sql = @"SELECT id_inquilino, fecha_inicio, fecha_fin, monto, estado FROM contrato WHERE id_inmueble = @idInmueble AND estado = TRUE;";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idInmueble", idInmueble);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res.Add(new Contrato
                                {
                                    IdInquilino = reader.GetInt32(0),
                                    Inquilino = new RepositoryInquilino(configuration).ObtenerPorId(reader.GetInt32(0)),
                                    Fecha_inicio = reader.GetDateTime(1),
                                    Fecha_fin = reader.GetDateTime(2),
                                    Monto = reader.GetInt32(3),
                                    Estado = reader.GetBoolean(4)
                                });
                            }
                        }
                        conn.Close();
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ObtenerContratosPorInmueble: " + ex.Message);
                throw;
            }
        }

        public PagedResult<Contrato> Paginar(int pagina, int tamPagina)
        {
            var res = new List<Contrato>();
            int totalItems = 0;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string countSql = "SELECT COUNT (*) FROM contrato WHERE estado = true";
                using (var countCmd = new NpgsqlCommand(countSql, conn))
                {
                    totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                string sql = @"
                SELECT c.id, i.direccion, inq.nombre, inq.apellido, c.fecha_inicio, c.fecha_fin, c.monto
                FROM contrato c
                INNER JOIN inmueble i ON c.id_inmueble = i.id
                INNER JOIN inquilino inq ON c.id_inquilino = inq.idInquilino
                WHERE c.estado = true;
                ";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Contrato
                            {
                                Id = reader.GetInt32(0),
                                Inmueble = new Inmueble
                                {
                                    Direccion = reader.GetString(1)
                                },
                                Inquilino = new Inquilino
                                {
                                    Nombre = reader.GetString(2),
                                    Apellido = reader.GetString(3)
                                },
                                Fecha_inicio = reader.GetDateTime(4),
                                Fecha_fin = reader.GetDateTime(5),
                                Monto = reader.GetInt32(6)
                            });
                        }
                    }
                }
                conn.Close();
            }
            return new PagedResult<Contrato>
            {
                Items = res,
                TotalItems = totalItems,
                PageNumber = tamPagina,
                PageSize = tamPagina
            };
        }
    }
}


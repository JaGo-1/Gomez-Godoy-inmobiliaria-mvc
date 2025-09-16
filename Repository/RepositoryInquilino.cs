using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;
using Npgsql;

namespace inmobiliaria_mvc.Repository
{
    public class RepositoryInquilino : RepositorioBase, IRepositoryInquilino
    {
        private readonly IConfiguration _configuration;

        public RepositoryInquilino(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public int Alta(Inquilino inquilino)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO Inquilino (Dni, Nombre, Apellido, Telefono, Email, Estado) 
                            VALUES (@dni, @nombre, @apellido, @telefono, @email, @estado) 
                            RETURNING IdInquilino;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("dni", inquilino.Dni);
                    cmd.Parameters.AddWithValue("nombre", inquilino.Nombre);
                    cmd.Parameters.AddWithValue("apellido", inquilino.Apellido);
                    cmd.Parameters.AddWithValue("telefono", inquilino.Telefono);
                    cmd.Parameters.AddWithValue("email", inquilino.Email);
                    cmd.Parameters.AddWithValue("estado", true);

                    conn.Open();
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        res = Convert.ToInt32(id);
                        inquilino.IdInquilino = res;
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
                string sql = "UPDATE Inquilino SET Estado = false WHERE IdInquilino = @id;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public int Modificacion(Inquilino inquilino)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = @"UPDATE Inquilino SET 
                               Dni = @dni, 
                               Nombre = @nombre, 
                               Apellido = @apellido, 
                               Telefono = @telefono, 
                               Email = @email 
                               WHERE IdInquilino = @idinquilino;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("idinquilino", inquilino.IdInquilino);
                    cmd.Parameters.AddWithValue("dni", inquilino.Dni);
                    cmd.Parameters.AddWithValue("nombre", inquilino.Nombre);
                    cmd.Parameters.AddWithValue("apellido", inquilino.Apellido);
                    cmd.Parameters.AddWithValue("telefono", inquilino.Telefono);
                    cmd.Parameters.AddWithValue("email", inquilino.Email);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public IList<Inquilino> ObtenerTodos()
        {
            var res = new List<Inquilino>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = "SELECT IdInquilino, Dni, Nombre, Apellido, Telefono, Email FROM Inquilino WHERE Estado = true;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32(0),
                                Dni = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5)
                            });
                        }
                    }
                    conn.Close();
                }
            }
            return res;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = "SELECT IdInquilino, Dni, Nombre, Apellido, Telefono, Email FROM Inquilino WHERE IdInquilino = @id;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(0),
                                Dni = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5)
                            };
                        }
                    }
                    conn.Close();
                }
            }
            return inquilino;
        }

        public PagedResult<Inquilino> Paginar(int pagina, int tamPagina)
        {
            var res = new List<Inquilino>();
            int totalItems = 0;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string countSql = "SELECT COUNT (*) FROM inquilino WHERE estado = true";
                using (var countCmd = new NpgsqlCommand(countSql, conn))
                {
                    totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                string sql = "SELECT IdInquilino, Dni, Nombre, Apellido, Telefono, Email FROM Inquilino WHERE Estado = true ORDER BY IdInquilino LIMIT @tamPagina OFFSET (@pagina - 1) * @tamPagina";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("pagina", pagina);
                    cmd.Parameters.AddWithValue("tamPagina", tamPagina);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inquilino
                            {
                                IdInquilino = (int)reader.GetInt64(0),
                                Dni = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5)
                            });
                        }
                    }
                }
                conn.Close();
            }

            return new PagedResult<Inquilino>
            {
                Items = res,
                TotalItems = totalItems,
                PageNumber = pagina,
                PageSize = tamPagina
            };
        }
    }
}

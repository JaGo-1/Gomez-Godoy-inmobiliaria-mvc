using inmobiliaria_mvc.Models;
using Npgsql;

namespace inmobiliaria_mvc.Repository
{
    public class RepositoryPropietario : RepositorioBase, IRepositoryPropietario
    {
        public RepositoryPropietario(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Propietario propietario)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO Propietario (dni, nombre, apellido, telefono, email, clave, estado) VALUES (@dni, @nombre, @apellido, @telefono, @email, @clave, @estado)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("dni", propietario.Dni);
                    cmd.Parameters.AddWithValue("nombre", propietario.Nombre);
                    cmd.Parameters.AddWithValue("apellido", propietario.Apellido);
                    cmd.Parameters.AddWithValue("telefono", propietario.Telefono);
                    cmd.Parameters.AddWithValue("email", propietario.Email);
                    cmd.Parameters.AddWithValue("clave", propietario.Clave);
                    cmd.Parameters.AddWithValue("estado", true);

                    conn.Open();
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        res = Convert.ToInt32(id);
                        propietario.Id = res;
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
                string sql = "UPDATE propietario SET estado = false WHERE id = @id";
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

        public int Modificacion(Propietario p)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = "UPDATE propietario SET dni = @dni, nombre = @nombre, apellido = @apellido, telefono = @telefono, email = @email, clave = @clave WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", p.Id);
                    cmd.Parameters.AddWithValue("dni", p.Dni);
                    cmd.Parameters.AddWithValue("nombre", p.Nombre);
                    cmd.Parameters.AddWithValue("apellido", p.Apellido);
                    cmd.Parameters.AddWithValue("telefono", p.Telefono);
                    cmd.Parameters.AddWithValue("email", p.Email);
                    cmd.Parameters.AddWithValue("clave", p.Clave);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? propietario = null;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = "SELECT id, dni, nombre, apellido, telefono, email, clave FROM propietario WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            propietario = new Propietario
                            {
                                Id = reader.GetInt32(0),
                                Dni = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5),
                                Clave = reader.GetString(6)
                            };
                        }
                    }
                    conn.Close();
                }
            }
            return propietario;
        }

        public IList<Propietario> ObtenerTodos()
        {
            var res = new List<Propietario>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = "SELECT id, dni, nombre, apellido, telefono, email, clave FROM propietario WHERE estado = true";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Propietario
                            {
                                Id = reader.GetInt32(0),
                                Dni = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Apellido = reader.GetString(3),
                                Telefono = reader.GetString(4),
                                Email = reader.GetString(5),
                                Clave = reader.GetString(6)
                            });
                        }
                    }
                    conn.Close();
                }
            }
            return res;
        }
    }
}
using System.Data;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;
using Npgsql;
using Npgsql.Internal;

namespace inmobiliaria_mvc.Repository;

public class RepositoryInmueble : RepositorioBase, IRepositoryInmueble
{
    private readonly IConfiguration _configuration;

    public RepositoryInmueble(IConfiguration configuration) : base(configuration)
    {
        _configuration = configuration;
    }

    public int Alta(Inmueble inmueble)
    {
        int res = -1;
        using (var conn = new NpgsqlConnection(connectionString))
        {
            var sql = @"
                INSERT INTO Inmueble 
                (Direccion, Precio, Ambientes, Estado, Latitud, Longitud, Uso, Tipo, PropietarioId, Portada)
                VALUES 
                (@direccion, @precio, @ambientes, @estado, @latitud, @longitud, @uso, @tipo, @propietarioId, @portada)
                RETURNING Id;";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                cmd.Parameters.AddWithValue("@precio", inmueble.Precio);
                cmd.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                cmd.Parameters.AddWithValue("@estado", true);
                cmd.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                cmd.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                cmd.Parameters.AddWithValue("@uso", inmueble.Uso.ToString());
                cmd.Parameters.AddWithValue("@tipo", inmueble.Tipo.ToString());
                cmd.Parameters.AddWithValue("@propietarioId", inmueble.PropietarioId);
                cmd.Parameters.AddWithValue("@portada", string.IsNullOrEmpty(inmueble.Portada) ? (object)DBNull.Value : inmueble.Portada);

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
            string sql = "UPDATE Inmueble SET Estado = false WHERE Id = @id";
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

    public int Modificacion(Inmueble p)
    {
        int res = -1;
        using (var conn = new NpgsqlConnection(connectionString))
        {
            string sql = @"
                UPDATE Inmueble SET
                    Direccion = @direccion,
                    Precio = @precio,
                    Ambientes = @ambientes,
                    Estado = @estado,
                    Latitud = @latitud,
                    Longitud = @longitud,
                    Uso = @uso,
                    Tipo = @tipo,
                    PropietarioId = @propietarioId,
                    Portada = @portada
                WHERE Id = @id";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@direccion", p.Direccion);
                cmd.Parameters.AddWithValue("@precio", p.Precio);
                cmd.Parameters.AddWithValue("@ambientes", p.Ambientes);
                cmd.Parameters.AddWithValue("@estado", p.Estado);
                cmd.Parameters.AddWithValue("@latitud", p.Latitud);
                cmd.Parameters.AddWithValue("@longitud", p.Longitud);
                cmd.Parameters.AddWithValue("@uso", p.Uso.ToString());
                cmd.Parameters.AddWithValue("@tipo", p.Tipo.ToString());
                cmd.Parameters.AddWithValue("@propietarioId", p.PropietarioId);
                cmd.Parameters.AddWithValue("@portada", string.IsNullOrEmpty(p.Portada) ? (object)DBNull.Value : p.Portada);
                cmd.Parameters.AddWithValue("@id", p.Id);

                conn.Open();
                res = cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        return res;
    }

    public IList<Inmueble> ObtenerTodos()
    {
        var res = new List<Inmueble>();
        using (var conn = new NpgsqlConnection(connectionString))
        {
            string sql = @"
            SELECT i.Id, i.Direccion, i.Precio, i.Ambientes, i.Estado,
                   i.Latitud, i.Longitud, i.Uso, i.Tipo, i.PropietarioId, i.Portada,
                   p.Nombre, p.Apellido
            FROM Inmueble i
            INNER JOIN Propietario p ON i.PropietarioId = p.Id
            WHERE i.Estado = true;";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader.GetString(1),
                            Precio = reader.GetDecimal(2),
                            Ambientes = reader.GetInt32(3),
                            Estado = reader.GetBoolean(4),
                            Latitud = reader.GetDouble(5),
                            Longitud = reader.GetDouble(6),
                            Uso = Enum.Parse<UsoInmueble>(reader.GetString(7)),
                            Tipo = Enum.Parse<TipoInmueble>(reader.GetString(8)),
                            PropietarioId = reader.GetInt32(9),
                            Portada = reader.IsDBNull(10) ? null : reader.GetString(10),
                            Propietario = new Propietario
                            {
                                Nombre = reader.GetString(11),
                                Apellido = reader.GetString(12)
                            }
                        });
                    }
                }
                conn.Close();
            }
        }
        return res;
    }

    public Inmueble ObtenerPorId(int id)
    {
        Inmueble? inmueble = null;
        using (var conn = new NpgsqlConnection(connectionString))
        {
            string sql = @"
                SELECT Id, Direccion, Precio, Ambientes, Estado, Latitud, Longitud, Uso, Tipo, PropietarioId, Portada
                FROM Inmueble 
                WHERE Id = @Id";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader.GetString(1),
                            Precio = reader.GetDecimal(2),
                            Ambientes = reader.GetInt32(3),
                            Estado = reader.GetBoolean(4),
                            Latitud = reader.GetDouble(5),
                            Longitud = reader.GetDouble(6),
                            Uso = Enum.Parse<UsoInmueble>(reader.GetString(7)),
                            Tipo = Enum.Parse<TipoInmueble>(reader.GetString(8)),
                            PropietarioId = reader.GetInt32(9),
                            Portada = reader.IsDBNull(10) ? null : reader.GetString(10)
                        };
                    }
                }
                conn.Close();
            }
        }
        return inmueble!;
    }

    public List<Inmueble> ObtenerPorPropietario(int propietarioId)
    {
        try
        {
            var res = new List<Inmueble>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = @"SELECT direccion, tipo, uso, ambientes, precio FROM inmueble WHERE propietarioid = @propietarioId AND estado = true;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@propietarioId", propietarioId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Inmueble
                            {
                                Direccion = reader.GetString(0),
                                Tipo = Enum.Parse<TipoInmueble>(reader.GetString(1)),
                                Uso = Enum.Parse<UsoInmueble>(reader.GetString(2)),
                                Ambientes = reader.GetInt32(3),
                                Precio = reader.GetDecimal(4)
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
            Console.WriteLine("Error en ObtenerPorPropietario: " + ex.Message);
            throw;
        }

    }

    public PagedResult<Inmueble> Paginar(int pagina, int tamPagina)
    {
        var res = new List<Inmueble>();
        int totalItems = 0;

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            string countSql = "SELECT COUNT (*) FROM inmueble WHERE estado = true";
            using (var countCmd = new NpgsqlCommand(countSql, conn))
            {
                totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
            }

            string sql = @"
            SELECT i.Id, i.Direccion, i.Precio, i.Ambientes, i.Estado, i.Latitud, i.Longitud, i.Uso, i.Tipo, i.PropietarioId, i.Portada, p.Nombre, p.Apellido 
            FROM inmueble i
            INNER JOIN Propietario p ON i.PropietarioId = p.Id
            WHERE i.Estado = true
            ORDER BY i.Id
            LIMIT @tamPagina OFFSET (@pagina - 1) * @tamPagina
            ";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("pagina", pagina);
                cmd.Parameters.AddWithValue("tamPagina", tamPagina);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader.GetString(1),
                            Precio = reader.GetDecimal(2),
                            Ambientes = reader.GetInt32(3),
                            Estado = reader.GetBoolean(4),
                            Latitud = reader.GetDouble(5),
                            Longitud = reader.GetDouble(6),
                            Uso = Enum.Parse<UsoInmueble>(reader.GetString(7)),
                            Tipo = Enum.Parse<TipoInmueble>(reader.GetString(8)),
                            PropietarioId = reader.GetInt32(9),
                            Portada = reader.IsDBNull(10) ? null : reader.GetString(10),
                            Propietario = new Propietario
                            {
                                Nombre = reader.GetString(11),
                                Apellido = reader.GetString(12)
                            }
                        });
                    }
                }
            }
            conn.Close();
        }
        return new PagedResult<Inmueble>
        {
            Items = res,
            TotalItems = totalItems,
            PageNumber = pagina,
            PageSize = tamPagina
        };
    }
    public int ModificarPortada(int id, string url)
    {
        int res = -1;
        using (var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"
            UPDATE Inmueble SET
            Portada = @portada
            WHERE Id = @id";
        
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@portada", string.IsNullOrEmpty(url) ? (object)DBNull.Value : url);
                command.Parameters.AddWithValue("@id", id);
                command.CommandType = CommandType.Text;
            
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }
}
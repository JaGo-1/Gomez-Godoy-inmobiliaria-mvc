using inmobiliaria_mvc.Models;
using Npgsql;

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
                (Direccion, Precio, Ambientes, Estado, Latitud, Longitud, Uso, Tipo, PropietarioId)
                VALUES 
                (@direccion, @precio, @ambientes, @estado, @latitud, @longitud, @uso, @tipo, @propietarioId)
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
                    PropietarioId = @propietarioId
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
                SELECT Id, Direccion, Precio, Ambientes, Estado, Latitud, Longitud, Uso, Tipo, PropietarioId 
                FROM Inmueble 
                WHERE Estado = true;";

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
                            PropietarioId = reader.GetInt32(9)
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
                SELECT Id, Direccion, Precio, Ambientes, Estado, Latitud, Longitud, Uso, Tipo, PropietarioId 
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
                            PropietarioId = reader.GetInt32(9)
                        };
                    }
                }
                conn.Close();
            }
        }
        return inmueble!;
    }
}

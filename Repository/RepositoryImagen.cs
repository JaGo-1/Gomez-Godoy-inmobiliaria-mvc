using System.Data;
using inmobiliaria_mvc.Models;
using Npgsql;

namespace inmobiliaria_mvc.Repository;

public class RepositoryImagen : RepositorioBase, IRepositoryImagen
{
    public RepositoryImagen(IConfiguration configuration) : base(configuration)
    {
    }
    public int Alta(Imagen p)
    {
        int res = -1;
        using (var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"INSERT INTO Imagen
                (InmuebleId, Url) 
                VALUES (@inmuebleId, @url)
                RETURNING Id;"; 
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@inmuebleId", p.InmuebleId);
                command.Parameters.AddWithValue("@url", p.Url);
                connection.Open();
                
                var id = command.ExecuteScalar();
                if (id != null)
                {
                    res = Convert.ToInt32(id);
                }
                connection.Close();
            }
        }
        return res;
    }

    public int Baja(int id)
    {
        int res = -1;
        using (var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"DELETE FROM Imagen WHERE Id = @id";
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public int Modificacion(Imagen p)
    {
        int res = -1;
        using (var connection = new NpgsqlConnection(connectionString))
        {
            string sql = @"
            UPDATE Imagen SET Url = @url
            WHERE Id=@id";
        
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@id", p.Id);
                command.Parameters.AddWithValue("@url", p.Url);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public Imagen ObtenerPorId(int id)
    {
        Imagen? res = null;
        using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
        {
            string sql = @"
                SELECT 
                    Id, 
                    InmuebleId, 
                    Url 
                FROM Imagen
                WHERE Id=@id";
            using (NpgsqlCommand comm = new NpgsqlCommand(sql, conn))
            {
                comm.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (var reader = comm.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        res = new Imagen
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            InmuebleId = reader.GetInt32(reader.GetOrdinal("InmuebleId")),
                            Url = reader.GetString(reader.GetOrdinal("Url"))
                        };
                    }
                }
                conn.Close();
            }
        }
        return res!;
    }

    public IList<Imagen> ObtenerTodos()
    {
        List<Imagen> res = new List<Imagen>();
        using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
        {
            string sql = @"
                SELECT 
                    Id, 
                    InmuebleId, 
                    Url 
                FROM Imagen";
            using (NpgsqlCommand comm = new NpgsqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Imagen
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            InmuebleId = reader.GetInt32(reader.GetOrdinal("InmuebleId")),
                            Url = reader.GetString(reader.GetOrdinal("Url")),
                        });
                    }
                }
                conn.Close();
            }
        }
        return res;
    }

    public IList<Imagen> BuscarPorInmueble(int inmuebleId)
    {
        List<Imagen> res = new List<Imagen>();
        using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
        {
            string sql = @"
                SELECT 
                    Id, 
                    InmuebleId, 
                    Url 
                FROM Imagen
                WHERE InmuebleId=@inmuebleId";
            using (NpgsqlCommand comm = new NpgsqlCommand(sql, conn))
            {
                comm.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                conn.Open();
                using (var reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new Imagen
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            InmuebleId = reader.GetInt32(reader.GetOrdinal("InmuebleId")),
                            Url = reader.GetString(reader.GetOrdinal("Url")),
                        });
                    }
                }
                conn.Close();
            }
        }
        return res;
    }
}
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using System.Linq;
using System.Threading.Tasks;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;

namespace inmobiliaria_mvc.Repository
{
    public class RepositoryUsuario : RepositorioBase, IRepositoryUsuario
    {
        public RepositoryUsuario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Usuario e)
        {
            int res = -1;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string sql =
                    @"INSERT INTO Usuario (Nombre, Apellido, Avatar, Email, Password, Rol) VALUES (@nombre, @apellido, @avatar, @email, @password, @rol) RETURNING Id;";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", e.Nombre);
                    command.Parameters.AddWithValue("@apellido", e.Apellido);
                    if (String.IsNullOrEmpty(e.Avatar))
                        command.Parameters.AddWithValue("@avatar", (object)"" ?? DBNull.Value);
                    else command.Parameters.AddWithValue("@avatar", e.Avatar);
                    command.Parameters.AddWithValue("@email", e.Email);
                    command.Parameters.AddWithValue("@password", e.Password);
                    command.Parameters.AddWithValue("@rol", e.Rol);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    e.Id = res;
                    connection.Close();
                }
            }

            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string sql = "DELETE FROM Usuario WHERE Id = @id";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
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

        public int Modificacion(Usuario e)
        {
            int res = -1;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string sql =
                    @"UPDATE Usuario SET Nombre = @nombre, Apellido = @apellido, Avatar = @avatar, Email = @email, Password = @password, Rol = @rol WHERE Id = @id";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", e.Nombre);
                    command.Parameters.AddWithValue("@apellido", e.Apellido);
                    command.Parameters.AddWithValue("@avatar", e.Avatar);
                    command.Parameters.AddWithValue("@email", e.Email);
                    command.Parameters.AddWithValue("@password", e.Password);
                    command.Parameters.AddWithValue("@rol", e.Rol);
                    command.Parameters.AddWithValue("@id", e.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return res;
        }

        public IList<Usuario> ObtenerTodos()
        {
            IList<Usuario> res = new List<Usuario>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string sql = @" SELECT Id, Nombre, Apellido, Avatar, Email, Password, Rol FROM Usuario";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Usuario e = new Usuario
                        {
                            Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"), Avatar = reader.GetString("Avatar"),
                            Email = reader.GetString("Email"), Password = reader.GetString("Password"),
                            Rol = reader.GetInt32("Rol"),
                        };
                        res.Add(e);
                    }

                    connection.Close();
                }
            }

            return res;
        }

        public Usuario ObtenerPorId(int id)
        {
            Usuario? e = null;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string sql = @"SELECT Id, Nombre, Apellido, Avatar, Email, Password, Rol FROM Usuario WHERE Id=@id";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        e = new Usuario
                        {
                            Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"), Avatar = reader.GetString("Avatar"),
                            Email = reader.GetString("Email"), Password = reader.GetString("Password"),
                            Rol = reader.GetInt32("Rol"),
                        };
                    }

                    connection.Close();
                }
            }

            return e;
        }

        public Usuario ObtenerPorEmail(string email)
        {
            Usuario? e = null;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                string sql =
                    @"SELECT Id, Nombre, Apellido, Avatar, Email, Password, Rol FROM Usuario WHERE Email=@email";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@email", NpgsqlDbType.Varchar).Value = email;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        e = new Usuario
                        {
                            Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"), Avatar = reader.GetString("Avatar"),
                            Email = reader.GetString("Email"), Password = reader.GetString("Password"),
                            Rol = reader.GetInt32("Rol"),
                        };
                    }

                    connection.Close();
                }
            }

            return e;
        }
    }
}
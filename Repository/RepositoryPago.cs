using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;
using Npgsql;

namespace inmobiliaria_mvc.Repository
{
    public class RepositoryPago : RepositorioBase, IRepositoryPago
    {
        private readonly IConfiguration _configuration;

        public RepositoryPago(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public int Alta(Pago pago)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO Pago 
                            (ContratoId, NumeroPago, FechaEsperada, FechaPago, Importe, Detalle, Estado, EsMulta)
                            VALUES (@contratoId, @numeroPago, @fechaEsperada, @fechaPago, @importe, @detalle, @estado, @EsMulta)
                            RETURNING IdPago;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("contratoId", pago.ContratoId);
                    cmd.Parameters.AddWithValue("numeroPago", pago.NumeroPago);
                    cmd.Parameters.AddWithValue("fechaEsperada", pago.FechaEsperada);
                    cmd.Parameters.AddWithValue("fechaPago", (object)pago.FechaPago ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("importe", pago.Importe);
                    cmd.Parameters.AddWithValue("detalle", pago.Detalle ?? string.Empty);
                    cmd.Parameters.AddWithValue("estado", pago.Estado);
                    cmd.Parameters.AddWithValue("EsMulta", pago.EsMulta);

                    conn.Open();
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        res = Convert.ToInt32(id);
                        pago.IdPago = res;
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
                string sql = "UPDATE Pago SET Estado = false WHERE IdPago = @id;";
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

        public int Modificacion(Pago p)
        {
            throw new NotImplementedException();
        }

        public int Modificacion(Pago pago, bool esRegistroReal = false)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = esRegistroReal
                    ? @"UPDATE Pago SET Detalle = @detalle, FechaPago = @fechaPago WHERE IdPago = @idPago;"
                    : @"UPDATE Pago SET Detalle = @detalle WHERE IdPago = @idPago;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("idPago", pago.IdPago);
                    cmd.Parameters.AddWithValue("detalle", pago.Detalle ?? string.Empty);
                    if (esRegistroReal)
                        cmd.Parameters.AddWithValue("fechaPago", (object)pago.FechaPago ?? DBNull.Value);

                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public IList<Pago> ObtenerTodos()
        {
            var res = new List<Pago>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = @"SELECT IdPago, ContratoId, NumeroPago, FechaEsperada, FechaPago, Importe, Detalle, Estado, EsMulta
                               FROM Pago WHERE Estado = true;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Pago
                            {
                                IdPago = reader.GetInt32(0),
                                ContratoId = reader.GetInt32(1),
                                NumeroPago = reader.GetInt32(2),
                                FechaEsperada = reader.GetDateTime(3),
                                FechaPago = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Importe = reader.GetDecimal(5),
                                Detalle = reader.GetString(6),
                                Estado = reader.GetBoolean(7),
                                EsMulta = reader.GetBoolean(8)
                            });
                        }
                    }
                    conn.Close();
                }
            }
            return res;
        }

        public IList<Pago> ObtenerPorContrato(int contratoId, bool incluirAnulados = false)
        {
            var res = new List<Pago>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string condicionEstado = incluirAnulados ? "" : "AND Estado = true";
                string sql = $@"SELECT IdPago, ContratoId, NumeroPago, FechaEsperada, FechaPago, Importe, Detalle, Estado, EsMulta
                               FROM Pago 
                               WHERE ContratoId = @contratoId {condicionEstado}
                               ORDER BY NumeroPago;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("contratoId", contratoId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(new Pago
                            {
                                IdPago = reader.GetInt32(0),
                                ContratoId = reader.GetInt32(1),
                                NumeroPago = reader.GetInt32(2),
                                FechaEsperada = reader.GetDateTime(3),
                                FechaPago = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Importe = reader.GetDecimal(5),
                                Detalle = reader.GetString(6),
                                Estado = reader.GetBoolean(7),
                                EsMulta = reader.GetBoolean(8)
                            });
                        }
                    }
                    conn.Close();
                }
            }
            return res;
        }

        public Pago? ObtenerPorId(int id)
        {
            Pago? pago = null;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = @"SELECT IdPago, ContratoId, NumeroPago, FechaEsperada, FechaPago, Importe, Detalle, Estado, EsMulta
                               FROM Pago WHERE IdPago = @id;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago
                            {
                                IdPago = reader.GetInt32(0),
                                ContratoId = reader.GetInt32(1),
                                NumeroPago = reader.GetInt32(2),
                                FechaEsperada = reader.GetDateTime(3),
                                FechaPago = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                Importe = reader.GetDecimal(5),
                                Detalle = reader.GetString(6),
                                Estado = reader.GetBoolean(7),
                                EsMulta = reader.GetBoolean(8)
                            };
                        }
                    }
                    conn.Close();
                }
            }
            return pago;
        }



        public int GenerarPrimerPagoParaContrato(int contratoId, decimal montoMensual, DateTime fechaInicio)
        {
            var pago = new Pago
            {
                ContratoId = contratoId,
                NumeroPago = 1,
                FechaEsperada = fechaInicio.Date,
                FechaPago = null,
                Importe = montoMensual,
                Detalle = "Mes 1 - Pendiente",
                Estado = true
            };
            return Alta(pago);
        }
        public int? CrearSiguientePagoSiAplica(int contratoId, int ultimoNumeroPago, decimal montoMensual, DateTime fechaInicio, DateTime fechaFin)
        {
            var siguienteNumero = ultimoNumeroPago + 1;
            var fechaSiguiente = fechaInicio.AddMonths(siguienteNumero - 1).Date;
            if (fechaSiguiente > fechaFin)
                return null;

            var existente = ObtenerPorContrato(contratoId, true)
                .FirstOrDefault(p => p.NumeroPago == siguienteNumero);
            if (existente != null)
                return null;

            var pagoSiguiente = new Pago
            {
                ContratoId = contratoId,
                NumeroPago = siguienteNumero,
                FechaEsperada = fechaSiguiente,
                FechaPago = null,
                Importe = montoMensual,
                Detalle = $"Mes {siguienteNumero} - Pendiente",
                Estado = true
            };
            return Alta(pagoSiguiente);
        }


        public int AnularPago(int idPago)
        {
            int res = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = "UPDATE Pago SET Estado = false WHERE IdPago = @idPago;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("idPago", idPago);
                    conn.Open();
                    res = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return res;
        }

        public PagedResult<PagoVM> Paginar(int page, int pageSize)
        {
            var res = new List<PagoVM>();
            int totalItems = 0;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string countSql = "SELECT COUNT(*) FROM Pago WHERE estado = true";
                using (var countCmd = new NpgsqlCommand(countSql, conn))
                {
                    totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
                }
                string sql = @"SELECT 
                p.IdPago,
                p.ContratoId,
                p.NumeroPago,
                p.FechaEsperada,
                p.FechaPago,
                p.Importe,
                p.Detalle,
                p.Estado,
                i.Direccion
                FROM Pago p
                INNER JOIN Contrato c ON p.ContratoId = c.Id
                INNER JOIN Inmueble i ON c.IdInmueble = i.Id
                WHERE p.Estado = true
                ORDER BY p.IdPago
                LIMIT @pageSize OFFSET (@page - 1) * @pageSize";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("pageSize", pageSize);
                    cmd.Parameters.AddWithValue("page", page);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pago = new Pago
                            {
                                IdPago = reader.GetInt32(0),
                                ContratoId = reader.GetInt32(1),
                                NumeroPago = reader.GetInt32(2),
                                FechaEsperada = reader.GetDateTime(3),
                                FechaPago = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                Importe = reader.GetDecimal(5),
                                Detalle = reader.GetString(6),
                                Estado = reader.GetBoolean(7)
                            };

                            res.Add(new PagoVM
                            {
                                Pago = pago,
                                DireccionInmueble = reader.GetString(8)
                            });
                        }
                    }
                }
                conn.Close();
            }
            return new PagedResult<PagoVM>
            {
                Items = res,
                TotalItems = totalItems,
                PageSize = pageSize,
                PageNumber = page
            };
        }
    }
}
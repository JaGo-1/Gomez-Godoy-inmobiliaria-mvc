using System.Data;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;
using Npgsql;
using inmobiliaria_mvc.Repository;

namespace inmobiliaria_mvc.Repository
{
    public class RepositoryContrato : RepositorioBase, IRepositoryContrato
    {
        private readonly IRepositoryPago _repoPago;

        public RepositoryContrato(IConfiguration configuration, IRepositoryPago repoPago) : base(configuration)
        {
            _repoPago = repoPago;
        }

        public int Alta(Contrato p)
        {
            int res = -1;
            int contratoId = -1;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                var sql = @"INSERT INTO contrato (idinmueble, idinquilino, fecha_inicio, fecha_fin, monto, estado) 
                            VALUES (@id_inmueble, @id_inquilino, @fecha_inicio, @fecha_fin, @monto, @estado) RETURNING id;";

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
                        contratoId = Convert.ToInt32(id);
                        res = 1;
                    }
                    conn.Close();
                }

                if (contratoId > 0)
                {
                    _repoPago.GenerarPrimerPagoParaContrato(contratoId, p.Monto, p.Fecha_inicio);
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
                var sql = @"UPDATE contrato SET idinmueble = @id_inmueble, idinquilino = @id_inquilino, 
                            fecha_inicio = @fecha_inicio, fecha_fin = @fecha_fin, monto = @monto,
                            fecha_terminacion_anticipada = @fecha_terminacion_anticipada, 
                            multa_calculada = @multa_calculada
                            WHERE id = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", p.Id);
                    cmd.Parameters.AddWithValue("@id_inmueble", p.IdInmueble);
                    cmd.Parameters.AddWithValue("@id_inquilino", p.IdInquilino);
                    cmd.Parameters.AddWithValue("@fecha_inicio", p.Fecha_inicio);
                    cmd.Parameters.AddWithValue("@fecha_fin", p.Fecha_fin);
                    cmd.Parameters.AddWithValue("@monto", p.Monto);
                    cmd.Parameters.AddWithValue("@fecha_terminacion_anticipada", (object)p.FechaTerminacionAnticipada ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@multa_calculada", (object)p.MultaCalculada ?? DBNull.Value);

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
                string sql = @"SELECT id, idinmueble, idinquilino, fecha_inicio, fecha_fin, monto, 
                               fecha_terminacion_anticipada, multa_calculada 
                               FROM contrato WHERE id = @id AND estado = true";

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
                                Id = reader.GetInt32("id"),
                                IdInmueble = reader.GetInt32("idinmueble"),
                                IdInquilino = reader.GetInt32("idinquilino"),
                                Fecha_inicio = reader.GetDateTime("fecha_inicio"),
                                Fecha_fin = reader.GetDateTime("fecha_fin"),
                                Monto = reader.GetDecimal("monto"),
                                FechaTerminacionAnticipada = reader.IsDBNull("fecha_terminacion_anticipada") ? null : reader.GetDateTime("fecha_terminacion_anticipada"),
                                MultaCalculada = reader.IsDBNull("multa_calculada") ? null : reader.GetDecimal("multa_calculada")
                            };
                        }
                    }
                    conn.Close();
                }
            }

            if (contrato != null)
            {
                var repoInquilino = new RepositoryInquilino(configuration);
                var repoInmueble = new RepositoryInmueble(configuration);
                contrato.Inquilino = repoInquilino.ObtenerPorId(contrato.IdInquilino);
                contrato.Inmueble = repoInmueble.ObtenerPorId(contrato.IdInmueble);
            }

            return contrato;
        }

        public IList<Contrato> ObtenerTodos()
        {
            var res = new List<Contrato>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string sql = @"SELECT id, idinmueble, idinquilino, fecha_inicio, fecha_fin, monto
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
                                Id = reader.GetInt32("id"),
                                IdInmueble = reader.GetInt32("idinmueble"),
                                IdInquilino = reader.GetInt32("idinquilino"),
                                Inmueble = repoInmueble.ObtenerPorId(reader.GetInt32("idinmueble")),
                                Inquilino = repoInquilino.ObtenerPorId(reader.GetInt32("idinquilino")),
                                Fecha_inicio = reader.GetDateTime("fecha_inicio"),
                                Fecha_fin = reader.GetDateTime("fecha_fin"),
                                Monto = reader.GetDecimal("monto"),
                            });
                        }
                    }
                    conn.Close();
                }
            }
            return res;
        }

        public bool ExisteSolapado(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int? contratoId = null)
        {
            using var conn = new NpgsqlConnection(connectionString);

            string sql = @"SELECT COUNT(*) FROM contrato WHERE idinmueble = @id_inmueble AND estado = true 
                           AND (fecha_inicio, fecha_fin) OVERLAPS (@fecha_inicio, @fecha_fin)";

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
                    string sql = @"SELECT id, idinquilino, fecha_inicio, fecha_fin, monto, estado, fecha_terminacion_anticipada, multa_calculada 
                                   FROM contrato WHERE idinmueble = @idInmueble AND estado = TRUE;";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idInmueble", idInmueble);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var repoInquilino = new RepositoryInquilino(configuration);
                            while (reader.Read())
                            {
                                res.Add(new Contrato
                                {
                                    Id = reader.GetInt32("id"),
                                    IdInquilino = reader.GetInt32("idinquilino"),
                                    Inquilino = repoInquilino.ObtenerPorId(reader.GetInt32("idinquilino")),
                                    Fecha_inicio = reader.GetDateTime("fecha_inicio"),
                                    Fecha_fin = reader.GetDateTime("fecha_fin"),
                                    Monto = reader.GetDecimal("monto"),
                                    Estado = reader.GetBoolean("estado"),
                                    FechaTerminacionAnticipada = reader.IsDBNull("fecha_terminacion_anticipada") ? null : reader.GetDateTime("fecha_terminacion_anticipada"),
                                    MultaCalculada = reader.IsDBNull("multa_calculada") ? null : reader.GetDecimal("multa_calculada")
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
        public PagedResult<Contrato> Paginar(int pagina, int tamPagina, bool? disponible = null, int? plazo = null)
        {
            var res = new List<Contrato>();
            int totalItems = 0;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                var whereConditions = new List<string>();

                if (disponible.HasValue)
                    whereConditions.Add("c.estado = @disponible");

                if (plazo.HasValue)
                    whereConditions.Add("(c.fecha_fin - c.fecha_inicio) = @plazo");

                string whereClause = whereConditions.Any()
                    ? "WHERE " + string.Join(" AND ", whereConditions)
                    : "";

                // COUNT
                string countSql = $"SELECT COUNT(*) FROM contrato c {whereClause}";
                using (var countCmd = new NpgsqlCommand(countSql, conn))
                {
                    if (disponible.HasValue)
                        countCmd.Parameters.AddWithValue("disponible", disponible.Value);

                    if (plazo.HasValue)
                        countCmd.Parameters.AddWithValue("plazo", plazo.Value);

                    totalItems = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                // SELECT
                string sql = $@"
                SELECT c.id, i.direccion, inq.nombre, inq.apellido, c.fecha_inicio, c.fecha_fin, c.monto   
                FROM contrato c
                INNER JOIN inmueble i ON c.idinmueble = i.id
                INNER JOIN inquilino inq ON c.idinquilino = inq.idInquilino
                {whereClause}
                ORDER BY c.id
                LIMIT @tamPagina OFFSET (@pagina - 1) * @tamPagina";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("pagina", pagina);
                    cmd.Parameters.AddWithValue("tamPagina", tamPagina);

                    if (disponible.HasValue)
                        cmd.Parameters.AddWithValue("disponible", disponible.Value);

                    if (plazo.HasValue)
                        cmd.Parameters.AddWithValue("plazo", plazo.Value);

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
            }

            return new PagedResult<Contrato>
            {
                Items = res,
                TotalItems = totalItems,
                PageNumber = pagina,
                PageSize = tamPagina
            };
        }

        public bool TerminarAnticipado(int contratoId, DateTime fechaTerminacion, bool pagarMultaAhora = false)
        {
            var contrato = ObtenerPorId(contratoId);
            if (contrato == null) return false;

            if (contrato.FechaTerminacionAnticipada.HasValue) return false;

            contrato.MultaCalculada = CalcularMultaImporte(contrato, fechaTerminacion);
            contrato.FechaTerminacionAnticipada = fechaTerminacion;
            contrato.Fecha_fin = fechaTerminacion;
            Modificacion(contrato);

            var pagos = _repoPago.ObtenerPorContrato(contratoId, incluirAnulados: true).ToList();
            var pagosFuturos = pagos
                .Where(p => p.FechaEsperada > fechaTerminacion && p.FechaPago == null && p.Estado)
                .ToList();

            foreach (var p in pagosFuturos)
            {
                _repoPago.AnularPago(p.IdPago);
            }

            var existingMulta = pagos.FirstOrDefault(p => p.EsMulta);
            if (existingMulta != null) return false;

            var ultimoNumero = pagos.Any() ? pagos.Max(p => p.NumeroPago) : 0;
            var pagoMulta = new Pago
            {
                ContratoId = contratoId,
                NumeroPago = ultimoNumero + 1,
                FechaEsperada = fechaTerminacion,
                FechaPago = pagarMultaAhora ? DateTime.Now : (DateTime?)null,
                Importe = contrato.MultaCalculada ?? 0m,
                Detalle = $"Multa por terminación anticipada ({CalcularMultaMeses(contrato, fechaTerminacion)} mes(es))",
                Estado = true,
                EsMulta = true
            };
            _repoPago.Alta(pagoMulta);

            return true;
        }

        public int CalcularMesesContrato(DateTime inicio, DateTime fin)
        {
            var meses = (fin.Year - inicio.Year) * 12 + fin.Month - inicio.Month;
            if (fin.Day < inicio.Day) meses--;
            return Math.Max(meses, 1);
        }

        public int CalcularMesesTranscurridos(Contrato contrato, DateTime fechaTerminacion)
        {
            return CalcularMesesContrato(contrato.Fecha_inicio, fechaTerminacion);
        }

        public int CalcularMesesAdeudados(Contrato contrato, DateTime fechaTerminacion)
        {
            var total = CalcularMesesContrato(contrato.Fecha_inicio, contrato.Fecha_fin);
            var transcurridos = CalcularMesesContrato(contrato.Fecha_inicio, fechaTerminacion);
            return Math.Max(total - transcurridos, 0);
        }

        public int CalcularMultaMeses(Contrato contrato, DateTime fechaTerminacion)
        {
            var transcurridos = CalcularMesesContrato(contrato.Fecha_inicio, fechaTerminacion);
            var total = CalcularMesesContrato(contrato.Fecha_inicio, contrato.Fecha_fin);
            return transcurridos < total / 2.0 ? 2 : 1;
        }

        public decimal CalcularMultaImporte(Contrato contrato, DateTime fechaTerminacion)
        {
            return CalcularMultaMeses(contrato, fechaTerminacion) * contrato.Monto;
        }
    }
}
using API.Models;
using API.Repository;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservacionesController : ControllerBase
    {
        private readonly DbContextBeach _dbContext;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly ITipoCambioService _tipoCambioService;
        private readonly ILogger<ReservacionesController> _logger;

        public ReservacionesController(
            DbContextBeach dbContext,
            IPdfService pdfService,
            IEmailService emailService,
            ITipoCambioService tipoCambioService,
            ILogger<ReservacionesController> logger)
        {
            _dbContext = dbContext;
            _pdfService = pdfService;
            _emailService = emailService;
            _tipoCambioService = tipoCambioService;
            _logger = logger;
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List()
        {
            List<Reservacion> reservaciones = _dbContext.Reservaciones
                .Include(r => r.Cliente)
                .Include(r => r.Paquete)
                .ToList();
            return Ok(reservaciones);
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(int id)
        {
            Reservacion? reservacion = _dbContext.Reservaciones
                .Include(r => r.Cliente)
                .Include(r => r.Paquete)
                .FirstOrDefault(r => r.IdReservacion == id);

            if (reservacion == null)
            {
                return NotFound($"No se encontró una reservación con el ID {id}.");
            }

            return Ok(reservacion);
        }

        [HttpGet]
        [Route("Factura/{idReservacion}")]
        public IActionResult Factura(int idReservacion)
        {
            Factura? factura = _dbContext.Facturas
                .FirstOrDefault(f => f.IdReservacion == idReservacion);

            if (factura == null)
            {
                return NotFound($"No se encontró una factura para la reservación con ID {idReservacion}.");
            }

            return Ok(factura);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Reservacion reservacion)
        {
            try
            {
                if (reservacion == null)
                {
                    return BadRequest("No se permiten datos vacíos.");
                }

                if (reservacion.CantidadNoches <= 0)
                {
                    return BadRequest("La cantidad de noches debe ser mayor que cero.");
                }

                if (reservacion.CantidadPersonas <= 0)
                {
                    return BadRequest("La cantidad de personas debe ser mayor que cero.");
                }

                if (string.IsNullOrEmpty(reservacion.MetodoPago))
                {
                    return BadRequest("Debe indicar el método de pago.");
                }

                if (reservacion.MetodoPago == "Cheque" && string.IsNullOrEmpty(reservacion.NumeroCheque))
                {
                    return BadRequest("Debe indicar el número de cheque.");
                }

                if (reservacion.MetodoPago == "Cheque" && string.IsNullOrEmpty(reservacion.BancoCheque))
                {
                    return BadRequest("Debe indicar el banco del cheque.");
                }

                Cliente? cliente = await _dbContext.Clientes
                    .FirstOrDefaultAsync(c => c.Cedula == reservacion.Cedula);

                if (cliente == null)
                {
                    return BadRequest("El cliente asociado no existe.");
                }

                Paquete? paquete = await _dbContext.Paquetes
                    .FirstOrDefaultAsync(p => p.IdPaquete == reservacion.IdPaquete);

                if (paquete == null)
                {
                    return BadRequest("El paquete asociado no existe.");
                }

                // --- Cálculos financieros ---
                decimal subTotal = paquete.PrecioPorNoche * reservacion.CantidadNoches;

                decimal descuento = 0;
                decimal porcentajeDescuento = 0;

                if (reservacion.MetodoPago == "Efectivo")
                {
                    if (reservacion.CantidadNoches >= 3 && reservacion.CantidadNoches <= 6)
                    {
                        porcentajeDescuento = 5;
                        descuento = subTotal * 0.05m;
                    }
                    else if (reservacion.CantidadNoches >= 7 && reservacion.CantidadNoches <= 9)
                    {
                        porcentajeDescuento = 15;
                        descuento = subTotal * 0.15m;
                    }
                    else if (reservacion.CantidadNoches >= 10 && reservacion.CantidadNoches <= 12)
                    {
                        porcentajeDescuento = 20;
                        descuento = subTotal * 0.20m;
                    }
                    else if (reservacion.CantidadNoches >= 13)
                    {
                        porcentajeDescuento = 25;
                        descuento = subTotal * 0.25m;
                    }
                }

                decimal montoGravable = subTotal - descuento;
                decimal iva = montoGravable * 0.13m;
                decimal totalFinal = montoGravable + iva;

                decimal prima = paquete.Prima;
                decimal mensualidad = 0;
                if (paquete.Meses > 0)
                {
                    mensualidad = (totalFinal - (totalFinal * prima)) / paquete.Meses;
                }

                decimal tipoCambio = await _tipoCambioService.ObtenerTipoCambioVentaAsync();
                decimal totalDolares = totalFinal / tipoCambio;

                // --- Asignar valores calculados ---
                reservacion.SubTotal = subTotal;
                reservacion.Descuento = descuento;
                reservacion.IVA = iva;
                reservacion.TotalFinal = totalFinal;
                reservacion.Prima = prima;
                reservacion.Mensualidad = mensualidad;
                reservacion.TipoCambio = tipoCambio;
                reservacion.TotalDolares = totalDolares;
                reservacion.FechaReservacion = DateTime.Now;

                _dbContext.Reservaciones.Add(reservacion);
                await _dbContext.SaveChangesAsync();

                // --- Crear factura asociada (snapshot en BD) ---
                var factura = new Factura
                {
                    IdReservacion = reservacion.IdReservacion,
                    Cedula = cliente.Cedula,
                    NombreCompleto = cliente.NombreCompleto,
                    CorreoElectronico = cliente.Email,
                    Telefono = cliente.Telefono,
                    NombrePaquete = paquete.Descripcion,
                    PrecioPorNoche = paquete.PrecioPorNoche,
                    CantidadNoches = reservacion.CantidadNoches,
                    CantidadPersonas = reservacion.CantidadPersonas,
                    SubTotal = subTotal,
                    Descuento = descuento,
                    PorcentajeDescuento = porcentajeDescuento,
                    MontoGravable = montoGravable,
                    IVA = iva,
                    TotalFinal = totalFinal,
                    TipoCambio = tipoCambio,
                    TotalDolares = totalDolares,
                    MetodoPago = reservacion.MetodoPago,
                    FechaEmision = DateTime.Now
                };

                _dbContext.Facturas.Add(factura);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Reservación {Id} creada exitosamente para cliente {Cedula} — Factura {IdFactura} generada.",
                    reservacion.IdReservacion, reservacion.Cedula, factura.IdFactura);

                // --- Disparar tarea en segundo plano para PDF + email ---
                // No se reusa _dbContext aquí porque el DbContext se libera al terminar
                // la petición HTTP. En su lugar, se pasan todos los datos ya cargados.
                int idReservacion = reservacion.IdReservacion;
                var reservacionSnapshot = reservacion;
                var clienteSnapshot = cliente;
                var paqueteSnapshot = paquete;
                var loggerSnapshot = _logger;
                var pdfServiceSnapshot = _pdfService;
                var emailServiceSnapshot = _emailService;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        byte[] pdfBytes = pdfServiceSnapshot.GenerarPdfReservacion(
                            reservacionSnapshot,
                            clienteSnapshot,
                            paqueteSnapshot,
                            tipoCambio,
                            porcentajeDescuento);

                        string nombreArchivo = $"Factura_Reservacion_{idReservacion:D4}.pdf";

                        string asunto = $"Beach.SA - Comprobante de Reservación N° FAC-{idReservacion:D4}";

                        string cuerpo = $@"
<html>
<body style='font-family: Calibri, sans-serif; padding: 20px;'>
    <h2 style='color: #0d47a1;'>¡Reservación confirmada!</h2>
    <p>Estimado(a) <strong>{clienteSnapshot.NombreCompleto}</strong>,</p>
    <p>Gracias por elegir <strong>Beach.SA - Hotel & Resort</strong>.</p>
    <p>Su reservación <strong>N° FAC-{idReservacion:D4}</strong> ha sido procesada exitosamente.</p>
    <p>Adjunto encontrará su comprobante de reservación en formato PDF con el detalle completo
       de su estadía, desglose económico y totalización en colones y dólares.</p>
    <hr style='border: 1px solid #0d47a1;' />
    <p style='font-size: 12px; color: #666;'>
        <strong>Resumen:</strong><br/>
        Paquete: {paqueteSnapshot.Descripcion}<br/>
        Noches: {reservacionSnapshot.CantidadNoches}<br/>
        Total: ₡{reservacionSnapshot.TotalFinal:N2} / ${reservacionSnapshot.TotalDolares:N2} USD<br/>
        Método de pago: {reservacionSnapshot.MetodoPago}
    </p>
    <hr style='border: 1px solid #0d47a1;' />
    <p style='font-size: 11px; color: #999;'>
        Beach.SA - Hotel & Resort | Tel: (506) 2222-0000<br/>
        Este es un mensaje automático, por favor no responder.
    </p>
</body>
</html>";

                        await emailServiceSnapshot.EnviarCorreoConAdjuntoAsync(
                            clienteSnapshot.Email!,
                            asunto,
                            cuerpo,
                            pdfBytes,
                            nombreArchivo);

                        loggerSnapshot.LogInformation(
                            "PDF y correo enviado para reservación {Id} a {Email}.",
                            idReservacion, clienteSnapshot.Email);
                    }
                    catch (Exception ex)
                    {
                        loggerSnapshot.LogError(ex,
                            "Error en tarea de fondo al generar PDF/email para reservación {Id}.",
                            idReservacion);
                    }
                });

                return Ok(reservacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reservación.");
                return StatusCode(500, $"Error interno al guardar la reservación: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Edit(Reservacion datosActualizados)
        {
            try
            {
                if (datosActualizados == null)
                {
                    return BadRequest("No se permiten datos vacíos.");
                }

                Reservacion? reservacionActual = await _dbContext.Reservaciones
                    .FirstOrDefaultAsync(r => r.IdReservacion == datosActualizados.IdReservacion);

                if (reservacionActual == null)
                {
                    return NotFound($"No existe una reservación con el ID {datosActualizados.IdReservacion}.");
                }

                Cliente? cliente = await _dbContext.Clientes
                    .FirstOrDefaultAsync(c => c.Cedula == datosActualizados.Cedula);

                if (cliente == null)
                {
                    return BadRequest("El cliente asociado no existe.");
                }

                Paquete? paquete = await _dbContext.Paquetes
                    .FirstOrDefaultAsync(p => p.IdPaquete == datosActualizados.IdPaquete);

                if (paquete == null)
                {
                    return BadRequest("El paquete asociado no existe.");
                }

                // --- Actualizar campos editables ---
                reservacionActual.Cedula = datosActualizados.Cedula;
                reservacionActual.IdPaquete = datosActualizados.IdPaquete;
                reservacionActual.CantidadNoches = datosActualizados.CantidadNoches;
                reservacionActual.CantidadPersonas = datosActualizados.CantidadPersonas;
                reservacionActual.MetodoPago = datosActualizados.MetodoPago;
                reservacionActual.NumeroCheque = datosActualizados.NumeroCheque;
                reservacionActual.BancoCheque = datosActualizados.BancoCheque;

                // --- Recalcular montos ---
                decimal subTotal = paquete.PrecioPorNoche * datosActualizados.CantidadNoches;
                decimal descuento = 0;

                if (datosActualizados.MetodoPago == "Efectivo")
                {
                    if (datosActualizados.CantidadNoches >= 3 && datosActualizados.CantidadNoches <= 6)
                        descuento = subTotal * 0.05m;
                    else if (datosActualizados.CantidadNoches >= 7 && datosActualizados.CantidadNoches <= 9)
                        descuento = subTotal * 0.15m;
                    else if (datosActualizados.CantidadNoches >= 10 && datosActualizados.CantidadNoches <= 12)
                        descuento = subTotal * 0.20m;
                    else if (datosActualizados.CantidadNoches >= 13)
                        descuento = subTotal * 0.25m;
                }

                decimal montoGravable = subTotal - descuento;
                decimal iva = montoGravable * 0.13m;
                decimal totalFinal = montoGravable + iva;

                decimal tipoCambio = await _tipoCambioService.ObtenerTipoCambioVentaAsync();
                decimal totalDolares = totalFinal / tipoCambio;
                decimal mensualidad = 0;
                if (paquete.Meses > 0)
                {
                    mensualidad = (totalFinal - (totalFinal * paquete.Prima)) / paquete.Meses;
                }

                reservacionActual.SubTotal = subTotal;
                reservacionActual.Descuento = descuento;
                reservacionActual.IVA = iva;
                reservacionActual.TotalFinal = totalFinal;
                reservacionActual.Prima = paquete.Prima;
                reservacionActual.Mensualidad = mensualidad;
                reservacionActual.TipoCambio = tipoCambio;
                reservacionActual.TotalDolares = totalDolares;

                _dbContext.Reservaciones.Update(reservacionActual);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Reservación {Id} actualizada exitosamente.", reservacionActual.IdReservacion);

                return Ok(reservacionActual);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reservación {Id}.", datosActualizados?.IdReservacion);
                return StatusCode(500, $"Error interno al modificar la reservación: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Reservacion? reservacion = await _dbContext.Reservaciones
                    .FirstOrDefaultAsync(r => r.IdReservacion == id);

                if (reservacion == null)
                {
                    return NotFound($"No existe una reservación con el ID {id}.");
                }

                Factura? factura = await _dbContext.Facturas
                    .FirstOrDefaultAsync(f => f.IdReservacion == id);

                if (factura != null)
                {
                    _dbContext.Facturas.Remove(factura);
                }

                _dbContext.Reservaciones.Remove(reservacion);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Reservación {Id} eliminada exitosamente.", id);

                return Ok($"Reservación {id} eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar reservación {Id}.", id);
                return StatusCode(500, $"Error interno al eliminar la reservación: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

    }
}

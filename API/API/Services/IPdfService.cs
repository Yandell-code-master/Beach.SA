using API.Models;

namespace API.Services
{
    /// <summary>
    /// Servicio para generar documentos PDF de factura/comprobante de reservación.
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Genera un comprobante de venta en PDF para una reservación.
        /// </summary>
        /// <param name="reservacion">Reservación con datos financieros ya calculados.</param>
        /// <param name="cliente">Cliente asociado a la reservación.</param>
        /// <param name="paquete">Paquete turístico seleccionado.</param>
        /// <param name="tipoCambio">Tipo de cambio USD/CRC del día.</param>
        /// <param name="porcentajeDescuento">Porcentaje de descuento aplicado (0 si no aplica).</param>
        /// <returns>Arreglo de bytes con el contenido del PDF.</returns>
        byte[] GenerarPdfReservacion(
            Reservacion reservacion,
            Cliente cliente,
            Paquete paquete,
            decimal tipoCambio,
            decimal porcentajeDescuento);
    }
}

namespace API.Services
{
    /// <summary>
    /// Servicio para envío de correos electrónicos con adjuntos.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico con un archivo PDF adjunto.
        /// </summary>
        /// <param name="destinatario">Dirección de correo del destinatario.</param>
        /// <param name="asunto">Asunto del mensaje.</param>
        /// <param name="cuerpo">Cuerpo HTML o texto plano del mensaje.</param>
        /// <param name="adjunto">Arreglo de bytes del archivo adjunto.</param>
        /// <param name="nombreAdjunto">Nombre del archivo adjunto (ej: Factura_001.pdf).</param>
        Task EnviarCorreoConAdjuntoAsync(
            string destinatario,
            string asunto,
            string cuerpo,
            byte[] adjunto,
            string nombreAdjunto);
    }
}

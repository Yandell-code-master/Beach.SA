namespace API.Services
{
    /// <summary>
    /// Servicio para obtener el tipo de cambio oficial del dólar (USD) 
    /// desde el Banco Central de Costa Rica (BCCR).
    /// </summary>
    public interface ITipoCambioService
    {
        /// <summary>
        /// Obtiene el tipo de cambio de venta del dólar en colones (CRC).
        /// Primero intenta consultar el API del BCCR; si falla, retorna el valor
        /// configurado por defecto en appsettings.json.
        /// </summary>
        Task<decimal> ObtenerTipoCambioVentaAsync();
    }
}

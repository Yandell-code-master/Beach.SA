using System.Globalization;
using System.Text.Json;

namespace API.Services
{
    /// <summary>
    /// Implementación que consulta el API REST del BCCR.
    /// Endpoint: https://gee.bccr.fi.cr/indicadores-economicos/api/v1/indicadores/317
    /// El código 317 = tipo de cambio venta USD.
    /// Requiere suscripción gratuita en https://www.bccr.fi.cr para obtener el subscription-key.
    /// Si la consulta falla, retorna el valor por defecto de configuración.
    /// </summary>
    public class TipoCambioService : ITipoCambioService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TipoCambioService> _logger;

        private decimal? _cacheValor;
        private DateTime _cacheMomento = DateTime.MinValue;
        private static readonly TimeSpan CacheDuracion = TimeSpan.FromMinutes(30);

        public TipoCambioService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TipoCambioService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<decimal> ObtenerTipoCambioVentaAsync()
        {
            // Retornar cache si aún es válido
            if (_cacheValor.HasValue && DateTime.UtcNow - _cacheMomento < CacheDuracion)
            {
                return _cacheValor.Value;
            }

            decimal valorDefecto = _configuration.GetValue<decimal>("TipoCambio:DefaultVenta", 500m);

            try
            {
                string? apiUrl = _configuration["TipoCambio:BccrApiUrl"];
                string? subscriptionKey = _configuration["TipoCambio:BccrSubscriptionKey"];

                if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(subscriptionKey))
                {
                    _logger.LogWarning("TipoCambio: BCCR no configurado (apiUrl o subscriptionKey). Usando valor por defecto {Valor}.", valorDefecto);
                    return valorDefecto;
                }

                using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("TipoCambio: Respuesta BCCR: {Json}", json);

                using var doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                // La respuesta es un array con un objeto que tiene "valor" como string
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    string valorStr = root[0].GetProperty("valor").GetString() ?? "";
                    if (decimal.TryParse(valorStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tipoCambio) && tipoCambio > 0)
                    {
                        _cacheValor = tipoCambio;
                        _cacheMomento = DateTime.UtcNow;
                        _logger.LogInformation("TipoCambio: Obtenido exitosamente del BCCR: {Valor}", tipoCambio);
                        return tipoCambio;
                    }
                }

                _logger.LogWarning("TipoCambio: Respuesta del BCCR con formato inesperado. Usando defecto {Valor}.", valorDefecto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TipoCambio: Error al consultar BCCR. Usando valor por defecto {Valor}.", valorDefecto);
            }

            return valorDefecto;
        }
    }
}

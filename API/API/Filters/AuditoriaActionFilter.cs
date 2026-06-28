using System.Security.Claims;
using API.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class AuditoriaActionFilter : IAsyncActionFilter
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaActionFilter(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string metodo = context.HttpContext.Request.Method;
            var executed = await next();

            bool esTransaccion = metodo == HttpMethods.Post
                              || metodo == HttpMethods.Put
                              || metodo == HttpMethods.Delete;
            if (!esTransaccion) return;

            // El login (anónimo) ya se audita en el AuthController; evitamos duplicado
            bool autenticado = context.HttpContext.User?.Identity?.IsAuthenticated ?? false;
            if (!autenticado) return;

            string? usuario = context.HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
            string? controller = context.RouteData.Values["controller"]?.ToString();
            string? accionRuta = context.RouteData.Values["action"]?.ToString();

            string accion = metodo switch
            {
                "POST" => "CREATE",
                "PUT" => "UPDATE",
                "DELETE" => "DELETE",
                _ => metodo
            };

            string? ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            int? status = executed.HttpContext.Response?.StatusCode;
            string detalle = $"{metodo} {context.HttpContext.Request.Path} -> {accionRuta} (HTTP {status})";

            try { _auditoriaService.Registrar(usuario, accion, controller, detalle, ip); }
            catch { /* la auditoría nunca debe romper la operación */ }
        }
    }
}

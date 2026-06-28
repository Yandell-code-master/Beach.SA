using API.DTO;
using API.Repository;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DbContextBeach _dbContextBeach;
        private readonly ITokenService _tokenService;
        private readonly IAuditoriaService _auditoriaService;

        public AuthController(DbContextBeach dbContextBeach, ITokenService tokenService, IAuditoriaService auditoriaService)
        {
            _dbContextBeach = dbContextBeach;
            _tokenService = tokenService;
            _auditoriaService = auditoriaService;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var usuario = _dbContextBeach.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Email == login.Email
                                  && u.Password == login.Password
                                  && u.Estado);

            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (usuario == null)
            {
                _auditoriaService.Registrar(login.Email, "LOGIN_FALLIDO", "Auth", "Credenciales incorrectas", ip);
                return Unauthorized(new AuthorizationResponse { Result = false, Msj = "Usuario o contraseña incorrectos." });
            }

            string token = _tokenService.DevolverToken(usuario);
            _auditoriaService.Registrar(usuario.Email, "LOGIN", "Auth", "Inicio de sesión exitoso", ip);

            return Ok(new AuthorizationResponse
            {
                Result = true,
                Msj = "Autenticación exitosa.",
                Token = token,
                Email = usuario.Email,
                Rol = usuario.Rol?.Nombre,
                Expira = DateTime.UtcNow.AddMinutes(8)
            });
        }

        [HttpGet]
        [Route("Menu")]
        [Authorize]
        public IActionResult Menu()
        {
            string? nombreRol = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(nombreRol)) return Unauthorized("No se pudo determinar el rol.");

            var funciones = (from rf in _dbContextBeach.RolFunciones
                             join r in _dbContextBeach.Roles on rf.IdRol equals r.IdRol
                             join f in _dbContextBeach.Funciones on rf.IdFuncion equals f.IdFuncion
                             where r.Nombre == nombreRol && r.Estado && f.Estado
                             orderby f.Orden
                             select new { f.IdFuncion, f.Codigo, f.Nombre, f.Url, f.Orden }).ToList();

            return Ok(funciones);
        }
    }
}

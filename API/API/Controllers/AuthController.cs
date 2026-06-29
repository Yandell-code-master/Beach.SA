using API.DTO;
using API.Repository;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DbContextBeach _dbContextBeach;
        private readonly ITokenService _tokenService;

        public AuthController(DbContextBeach dbContextBeach, ITokenService tokenService)
        {
            _dbContextBeach = dbContextBeach;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO login)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var usuario = _dbContextBeach.Usuarios
                .FirstOrDefault(u => u.Email == login.Email
                                  && u.Password == login.Password
                                  && u.Estado);

            if (usuario == null)
                return Unauthorized(new AuthorizationResponse { Result = false, Msj = "Usuario o contraseña incorrectos." });

            string token = _tokenService.DevolverToken(usuario);

            return Ok(new AuthorizationResponse
            {
                Result = true,
                Msj = "Autenticación exitosa.",
                Token = token,
                Email = usuario.Email,
                Expira = DateTime.UtcNow.AddMinutes(8)
            });
        }
    }
}

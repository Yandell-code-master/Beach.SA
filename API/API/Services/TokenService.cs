using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string DevolverToken(Usuario usuario)
        {
            string key = _configuration["Jwt:Key"] ?? string.Empty;
            string issuer = _configuration["Jwt:Issuer"] ?? string.Empty;
            string audience = _configuration["Jwt:Audience"] ?? string.Empty;
            int minutos = _configuration.GetValue<int>("Jwt:ExpireMinutes");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? string.Empty),
                new Claim("IdUsuario", usuario.IdUsuario.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims);

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(minutos),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}
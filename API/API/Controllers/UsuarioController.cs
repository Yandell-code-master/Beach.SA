using API.DTO;
using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly DbContextBeach _dbContextBeach;

        public UsuarioController(DbContextBeach dbContextBeach)
        {
            _dbContextBeach = dbContextBeach;
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List()
        {
            var usuarios = _dbContextBeach.Usuarios.Where(u => u.Estado)
                .Select(u => new { u.IdUsuario, u.Email, u.Estado })
                .ToList();
            return Ok(usuarios);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(UsuarioDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_dbContextBeach.Usuarios.Any(u => u.Email == dto.Email)) return BadRequest("Ya existe un usuario con ese email.");

            var nuevo = new Usuario
            {
                Email = dto.Email,
                Password = dto.Password,
                Estado = dto.Estado
            };
            _dbContextBeach.Usuarios.Add(nuevo);
            _dbContextBeach.SaveChanges();
            return Ok(new { nuevo.IdUsuario, nuevo.Email, Msj = "Usuario creado correctamente." });
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Edit(int id, UsuarioDTO dto)
        {
            var usuario = _dbContextBeach.Usuarios.FirstOrDefault(u => u.IdUsuario == id && u.Estado);
            if (usuario == null) return NotFound("No se encontró el usuario.");
            if (_dbContextBeach.Usuarios.Any(u => u.Email == dto.Email && u.IdUsuario != id)) return BadRequest("Ese email ya está en uso.");

            usuario.Email = dto.Email;
            usuario.Password = dto.Password;
            usuario.Estado = dto.Estado;
            _dbContextBeach.SaveChanges();
            return Ok("Usuario actualizado correctamente.");
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var usuario = _dbContextBeach.Usuarios.FirstOrDefault(u => u.IdUsuario == id && u.Estado);
            if (usuario == null) return NotFound("No se encontró el usuario.");
            usuario.Estado = false;
            _dbContextBeach.SaveChanges();
            return Ok("Usuario eliminado correctamente.");
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(int id)
        {
            var usuario = _dbContextBeach.Usuarios
                .Where(u => u.IdUsuario == id && u.Estado)
                .Select(u => new { u.IdUsuario, u.Email, u.Estado })
                .FirstOrDefault();
            if (usuario == null) return NotFound("No se encontró el usuario.");
            return Ok(usuario);
        }
    }
}

using API.DTO;
using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class RolController : ControllerBase
    {
        private readonly DbContextBeach _dbContextBeach;

        public RolController(DbContextBeach dbContextBeach)
        {
            _dbContextBeach = dbContextBeach;
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List() => Ok(_dbContextBeach.Roles.Where(r => r.Estado).ToList());

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(Rol rol)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_dbContextBeach.Roles.Any(r => r.Nombre == rol.Nombre)) return BadRequest("Ya existe un rol con ese nombre.");
            _dbContextBeach.Roles.Add(rol);
            _dbContextBeach.SaveChanges();
            return Ok(rol);
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Edit(int id, Rol nuevosDatos)
        {
            var rol = _dbContextBeach.Roles.FirstOrDefault(r => r.IdRol == id && r.Estado);
            if (rol == null) return NotFound("No se encontró el rol.");
            rol.Nombre = nuevosDatos.Nombre;
            rol.Descripcion = nuevosDatos.Descripcion;
            rol.Estado = nuevosDatos.Estado;
            _dbContextBeach.SaveChanges();
            return Ok(rol);
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var rol = _dbContextBeach.Roles.FirstOrDefault(r => r.IdRol == id && r.Estado);
            if (rol == null) return NotFound("No se encontró el rol.");
            rol.Estado = false;
            _dbContextBeach.SaveChanges();
            return Ok("Rol eliminado correctamente.");
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(int id)
        {
            var rol = _dbContextBeach.Roles.FirstOrDefault(r => r.IdRol == id && r.Estado);
            if (rol == null) return NotFound("No se encontró el rol.");
            return Ok(rol);
        }

        // Funciones asignadas a un rol
        [HttpGet]
        [Route("Funciones")]
        public IActionResult FuncionesDeRol(int idRol)
        {
            var funciones = (from rf in _dbContextBeach.RolFunciones
                             join f in _dbContextBeach.Funciones on rf.IdFuncion equals f.IdFuncion
                             where rf.IdRol == idRol && f.Estado
                             orderby f.Orden
                             select new { f.IdFuncion, f.Codigo, f.Nombre, f.Url, f.Orden }).ToList();
            return Ok(funciones);
        }

        // Reemplaza el set de funciones del rol. Body: { "idRol": 2, "idFunciones": [1,3,4] }
        [HttpPost]
        [Route("AsignarFunciones")]
        public IActionResult AsignarFunciones(AsignarFuncionesDTO dto)
        {
            var rol = _dbContextBeach.Roles.FirstOrDefault(r => r.IdRol == dto.IdRol && r.Estado);
            if (rol == null) return NotFound("No se encontró el rol.");

            var actuales = _dbContextBeach.RolFunciones.Where(rf => rf.IdRol == dto.IdRol).ToList();
            _dbContextBeach.RolFunciones.RemoveRange(actuales);

            foreach (int idFuncion in dto.IdFunciones.Distinct())
            {
                if (_dbContextBeach.Funciones.Any(f => f.IdFuncion == idFuncion && f.Estado))
                    _dbContextBeach.RolFunciones.Add(new RolFuncion { IdRol = dto.IdRol, IdFuncion = idFuncion });
            }

            _dbContextBeach.SaveChanges();
            return Ok("Funciones asignadas correctamente al rol.");
        }
    }
}

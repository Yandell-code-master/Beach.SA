using API.Models;
using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class FuncionController : ControllerBase
    {
        private readonly DbContextBeach _dbContextBeach;

        public FuncionController(DbContextBeach dbContextBeach)
        {
            _dbContextBeach = dbContextBeach;
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List() => Ok(_dbContextBeach.Funciones.Where(f => f.Estado).OrderBy(f => f.Orden).ToList());

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(Funcion funcion)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_dbContextBeach.Funciones.Any(f => f.Codigo == funcion.Codigo)) return BadRequest("Ya existe una función con ese código.");
            _dbContextBeach.Funciones.Add(funcion);
            _dbContextBeach.SaveChanges();
            return Ok(funcion);
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Edit(int id, Funcion nuevosDatos)
        {
            var funcion = _dbContextBeach.Funciones.FirstOrDefault(f => f.IdFuncion == id && f.Estado);
            if (funcion == null) return NotFound("No se encontró la función.");
            funcion.Codigo = nuevosDatos.Codigo;
            funcion.Nombre = nuevosDatos.Nombre;
            funcion.Url = nuevosDatos.Url;
            funcion.Orden = nuevosDatos.Orden;
            funcion.Estado = nuevosDatos.Estado;
            _dbContextBeach.SaveChanges();
            return Ok(funcion);
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var funcion = _dbContextBeach.Funciones.FirstOrDefault(f => f.IdFuncion == id && f.Estado);
            if (funcion == null) return NotFound("No se encontró la función.");
            funcion.Estado = false;
            _dbContextBeach.SaveChanges();
            return Ok("Función eliminada correctamente.");
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(int id)
        {
            var funcion = _dbContextBeach.Funciones.FirstOrDefault(f => f.IdFuncion == id && f.Estado);
            if (funcion == null) return NotFound("No se encontró la función.");
            return Ok(funcion);
        }
    }
}
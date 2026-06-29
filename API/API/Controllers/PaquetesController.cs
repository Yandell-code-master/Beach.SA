using API.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaqueteController : ControllerBase
    {
        private readonly DbContextBeach _dbContextBeach;

        public PaqueteController(DbContextBeach dbContextBeach)
        {
            _dbContextBeach = dbContextBeach;
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List()
        {
            return Ok(_dbContextBeach.Paquetes.Where(p => p.Estado).ToList());
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(Paquete paquete)
        {
            // Validar si ya existe un paquete con ese ID
            var paqueteExistente = _dbContextBeach.Paquetes.FirstOrDefault(p => p.IdPaquete == paquete.IdPaquete);

            if (paqueteExistente != null)
            {
                return BadRequest("Ya existe un paquete con ese ID.");
            }

            _dbContextBeach.Paquetes.Add(paquete);
            _dbContextBeach.SaveChanges();
            return Ok(paquete);
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Edit(int id, Paquete nuevosDatos)
        {
            var paqueteActualizar = _dbContextBeach.Paquetes.FirstOrDefault(p => p.IdPaquete == id && p.Estado);

            if (paqueteActualizar == null)
            {
                return NotFound("No se encontró el paquete con ese ID.");
            }

            paqueteActualizar.Descripcion = nuevosDatos.Descripcion;
            paqueteActualizar.PrecioPorNoche = nuevosDatos.PrecioPorNoche;
            paqueteActualizar.Prima = nuevosDatos.Prima;
            paqueteActualizar.Meses = nuevosDatos.Meses;
            paqueteActualizar.Estado = nuevosDatos.Estado;

            _dbContextBeach.SaveChanges();
            return Ok(paqueteActualizar);
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var paqueteEliminar = _dbContextBeach.Paquetes.FirstOrDefault(p => p.IdPaquete == id && p.Estado);

            if (paqueteEliminar == null)
            {
                return NotFound("No se encontró el paquete con ese ID.");
            }

            paqueteEliminar.Estado = false;
            _dbContextBeach.SaveChanges();
            return Ok("Paquete eliminado correctamente.");
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(int id)
        {
            var paquete = _dbContextBeach.Paquetes.FirstOrDefault(p => p.IdPaquete == id && p.Estado);

            if (paquete == null)
            {
                return NotFound("No se encontró el paquete con ese ID.");
            }

            return Ok(paquete);
        }
    }
}

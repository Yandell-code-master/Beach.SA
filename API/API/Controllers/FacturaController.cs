using API.Repository;
using Microsoft.AspNetCore.Mvc;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly DbContextBeach _dbContextBeach;

        public FacturaController(DbContextBeach dbContextBeach)
        {
            _dbContextBeach = dbContextBeach;
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List()
        {
            return Ok(_dbContextBeach.Facturas.ToList());
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(int id)
        {
            Factura? factura = _dbContextBeach.Facturas
                .FirstOrDefault(f => f.IdFactura == id);

            if (factura == null)
            {
                return NotFound($"No se encontró una factura con el ID {id}.");
            }

            return Ok(factura);
        }

        [HttpGet]
        [Route("PorReservacion/{idReservacion}")]
        public IActionResult PorReservacion(int idReservacion)
        {
            Factura? factura = _dbContextBeach.Facturas
                .FirstOrDefault(f => f.IdReservacion == idReservacion);

            if (factura == null)
            {
                return NotFound($"No se encontró una factura para la reservación ID {idReservacion}.");
            }

            return Ok(factura);
        }
    }
}

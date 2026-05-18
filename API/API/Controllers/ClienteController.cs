using API.Repository;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using Microsoft.Identity.Client;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private DbContextBeach dbContextBeach;

        public ClienteController(DbContextBeach dbContextBeach)
        {
            this.dbContextBeach = dbContextBeach;
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List()
        {
            return Ok(this.dbContextBeach.Clientes.ToList());
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(Cliente cliente)
        {
            Cliente clienteBuscado = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cliente.Cedula);


            if (clienteBuscado != null)
            {
                return BadRequest("Ya existe un cliente con esa cédula.");
            }


            this.dbContextBeach.Clientes.Add(cliente);
            this.dbContextBeach.SaveChanges();
            return Ok(cliente);
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Edit(int cedula, Cliente nuevosDatos) 
        {
            Cliente clientePorActualizar = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cedula);

            if (clientePorActualizar == null)
            {
                return NotFound("No se encontró un cliente con ese ID.");
            } 

            clientePorActualizar.Email = nuevosDatos.Email;
            clientePorActualizar.NombreCompleto = nuevosDatos.NombreCompleto;
            clientePorActualizar.Telefono = nuevosDatos.Telefono;
            clientePorActualizar.TipoCedula = nuevosDatos.TipoCedula;
            clientePorActualizar.Direccion = nuevosDatos.Direccion;
            clientePorActualizar.Cedula = nuevosDatos.Cedula;
            this.dbContextBeach.SaveChanges();
            return Ok(clientePorActualizar);
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int cedula)
        {
            Cliente clientePorEliminar = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cedula);
            if (clientePorEliminar == null)
            {
                return NotFound("No se encontró un cliente con ese ID.");
            }

            this.dbContextBeach.Clientes.Remove(clientePorEliminar);
            this.dbContextBeach.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(int cedula) 
        { 
            Cliente clienteBuscado = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cedula);

            if (clienteBuscado == null)
            {
                return NotFound("No se encontró un cliente con ese ID.");
            }

            return Ok(clienteBuscado);
        }
    }
}

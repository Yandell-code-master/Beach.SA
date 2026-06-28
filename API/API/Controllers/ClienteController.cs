using API.Repository;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using Microsoft.Identity.Client;
using API.Services;
using Newtonsoft.Json;
using API.DTO;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private DbContextBeach dbContextBeach;
        private ApiServices apiServices;
        private HttpClient client;

        public ClienteController(DbContextBeach dbContextBeach, ApiServices apiServices)
        {
            this.dbContextBeach = dbContextBeach;
            this.apiServices = apiServices;
            this.client = apiServices.GetHttp();
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List()
        {
            return Ok(this.dbContextBeach.Clientes.Where(c => c.Estado).ToList());
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Cliente cliente)
        {
                Cliente clienteBuscado = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cliente.Cedula);


            if (clienteBuscado != null)
            {
                return BadRequest("Ya existe un cliente con esa cédula.");
            }

            HttpResponseMessage response = await client.GetAsync($"cedulas/{cliente.Cedula}");
            string json = await response.Content.ReadAsStringAsync();
            ClienteGometaDTO clienteDTO = JsonConvert.DeserializeObject<ClienteGometaDTO>(json);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("La cédula no es válida.");
            }


            if (clienteDTO.Error != null)
            {
                return BadRequest(clienteDTO.Error);
            }


            if (clienteDTO == null)
            {
                return BadRequest("La cédula no es válida.");
            }

            cliente.NombreCompleto = clienteDTO.nombre;
            cliente.TipoCedula = clienteDTO.tipoIdentificacion;
            cliente.Estado = true;

            this.dbContextBeach.Clientes.Add(cliente);
            this.dbContextBeach.SaveChanges();
            return Ok(cliente);
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Edit(string cedula, Cliente nuevosDatos) 
        {
            Cliente clientePorActualizar = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cedula && c.Estado);

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
        public IActionResult Delete(string cedula)
        {
            Cliente clientePorEliminar = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cedula && c.Estado);
            if (clientePorEliminar == null)
            {
                return NotFound("No se encontró un cliente con ese ID.");
            }

            clientePorEliminar.Estado = false;
            this.dbContextBeach.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string cedula) 
        { 
            Cliente clienteBuscado = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cedula && c.Estado);

            if (clienteBuscado == null)
            {
                return NotFound("No se encontró un cliente con ese ID.");
            }

            return Ok(clienteBuscado);
        }
    }
}

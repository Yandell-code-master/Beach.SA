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
            return Ok(this.dbContextBeach.Clientes.ToList());
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Cedula) || !System.Text.RegularExpressions.Regex.IsMatch(cliente.Cedula, @"^[0-9]+$"))
            {
                return BadRequest("El número de cédula no es válido. Debe contener únicamente números, sin letras ni caracteres especiales.");
            }

            if (cliente.Cedula.Length < 9 || cliente.Cedula.Length > 15)
            {
                return BadRequest("La longitud del número de cédula es incorrecta.");
            }

            Cliente clienteBuscado = this.dbContextBeach.Clientes.FirstOrDefault(c => c.Cedula == cliente.Cedula);

            if (clienteBuscado != null)
            {
                return BadRequest("Ya existe un cliente con esa cédula.");
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync($"cedulas/{cliente.Cedula}");
                string json = await response.Content.ReadAsStringAsync();
                ClienteGometaDTO clienteDTO = JsonConvert.DeserializeObject<ClienteGometaDTO>(json);

                if (!response.IsSuccessStatusCode || clienteDTO == null)
                {
                    return BadRequest("La cédula no es válida o no fue encontrada en el padrón.");
                }

                if (clienteDTO.Error != null)
                {
                    return BadRequest(clienteDTO.Error);
                }

                cliente.NombreCompleto = clienteDTO.nombre;
                cliente.TipoCedula = clienteDTO.tipoIdentificacion;

                this.dbContextBeach.Clientes.Add(cliente);
                this.dbContextBeach.SaveChanges();
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest("Ocurrió un error al validar la cédula con el servicio externo: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Edit(string cedula, Cliente nuevosDatos) 
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
        public IActionResult Delete(string cedula)
        {
            try
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
            catch (Exception ex)
            {
               
                return BadRequest("Usuario tiene reservaciones, no se puede eliminar");
            }
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string cedula) 
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

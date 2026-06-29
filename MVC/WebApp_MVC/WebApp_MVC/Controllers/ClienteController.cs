using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebApp_MVC.Models; 

namespace WebApp_MVC.Controllers
{
    public class ClienteController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7002/api/Cliente/";

        public ClienteController()
        {
            _httpClient = new HttpClient();
        }

        // Método auxiliar para adjuntar el Token JWT en la cabecera Authorization
        private bool OrganizarToken()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            var response = await _httpClient.GetAsync(_apiUrl + "List");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<Cliente>>(data);
                return View(lista);
            }
            return View(new List<Cliente>());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_apiUrl + "Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                // CONTROL DE ERRORES: Si la API responde con un código de error (BadRequest, etc.)
                // leemos el mensaje limpio enviado por el servidor o la BD.
                var errorContent = await response.Content.ReadAsStringAsync();

                // Si el error contiene palabras técnicas de SQL, lo traducimos a algo amigable
                if (errorContent.Contains("UQ_Clientes_Email") || errorContent.Contains("duplicate key"))
                {
                    ViewBag.Error = "No se pudo registrar: El correo electrónico ya se encuentra asignado a otro cliente.";
                }
                else if (errorContent.Contains("Ya existe un cliente con esa cédula"))
                {
                    ViewBag.Error = "Ya existe un cliente registrado con ese número de cédula.";
                }
                 else
                {
                    ViewBag.Error = !string.IsNullOrEmpty(errorContent) ? errorContent : "Ocurrió un problema inesperado al registrar el cliente.";
                } ///
            }
            catch (Exception ex)
            {
                // Por si se cae la conexión física con la API
                ViewBag.Error = "Error de comunicación con el servidor central: " + ex.Message;
            }

            // Volvemos a cargar la lista actual para que la vista no se quede vacía al recargar con el error
            var listResponse = await _httpClient.GetAsync(_apiUrl + "List");
            List<Cliente> listaClientes = new List<Cliente>();

            if (listResponse.IsSuccessStatusCode)
            {
                var data = await listResponse.Content.ReadAsStringAsync();
                listaClientes = JsonConvert.DeserializeObject<List<Cliente>>(data);
            }

            return View("Index", listaClientes);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string cedula)
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiUrl}Delete?cedula={cedula}");

              
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

               
                var errorContent = await response.Content.ReadAsStringAsync();
                if (errorContent.Contains("Usuario tiene reservaciones, no se puede eliminar") || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ViewBag.Error = "No se puede eliminar el cliente porque posee reservaciones activas en el sistema.";
                }
                else
                {
                    ViewBag.Error = "Ocurrió un error inesperado al intentar eliminar el cliente.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error de comunicación con el servidor: " + ex.Message;
            }

         
            var listResponse = await _httpClient.GetAsync(_apiUrl + "List");
            List<Cliente> listaClientes = new List<Cliente>();

            if (listResponse.IsSuccessStatusCode)
            {
                var data = await listResponse.Content.ReadAsStringAsync();
                listaClientes = JsonConvert.DeserializeObject<List<Cliente>>(data);
            }

            return View("Index", listaClientes);
        }


    }
}

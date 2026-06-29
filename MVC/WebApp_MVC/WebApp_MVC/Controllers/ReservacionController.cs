using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers
{
    public class ReservacionController : Controller
    {
        private readonly HttpClient _httpClient;
        // Ajusta la URL según la ruta base de tu API
        private readonly string _apiUrl = "https://localhost:7002/api/Reservaciones/";

        public ReservacionController()
        {
            _httpClient = new HttpClient();
        }

        private bool OrganizarToken()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        // Método auxiliar para obtener la lista de paquetes
        private async Task<List<Paquete>> ObtenerPaquetesAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7002/api/Paquete/List");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Paquete>>(data) ?? new List<Paquete>();
            }
            return new List<Paquete>();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            var response = await _httpClient.GetAsync(_apiUrl + "List");
            var lista = new List<Reservacion>();

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                lista = JsonConvert.DeserializeObject<List<Reservacion>>(data);
            }

            // Cargar paquetes para el select
            ViewBag.Paquetes = await ObtenerPaquetesAsync();
            return View(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservacion reservacion)
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(reservacion), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_apiUrl + "Create", content);

                if (response.IsSuccessStatusCode) return RedirectToAction("Index");

                ViewBag.Error = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex) { ViewBag.Error = "Error: " + ex.Message; }

            // --- IMPORTANTE: Recargar datos para que la vista no falle ---
            ViewBag.Paquetes = await ObtenerPaquetesAsync();
            var listResponse = await _httpClient.GetAsync(_apiUrl + "List");
            var lista = new List<Reservacion>();
            if (listResponse.IsSuccessStatusCode)
                lista = JsonConvert.DeserializeObject<List<Reservacion>>(await listResponse.Content.ReadAsStringAsync());

            return View("Index", lista);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiUrl}Delete?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ViewBag.Error = "No se pudo eliminar la reservación. Verifique si tiene facturas asociadas o datos pendientes.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error de comunicación: " + ex.Message;
            }

            var listResponse = await _httpClient.GetAsync(_apiUrl + "List");
            var lista = listResponse.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Reservacion>>(await listResponse.Content.ReadAsStringAsync())
                : new List<Reservacion>();

            return View("Index", lista);
        }
    }
}
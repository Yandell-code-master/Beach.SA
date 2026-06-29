using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers
{
    public class FacturaController : Controller
    {
        private readonly HttpClient _httpClient;
        // Ajusta la URL base según la ruta de tu API
        private readonly string _apiUrl = "https://localhost:7002/api/Factura/";

        public FacturaController()
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

        // Listado general de facturas
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            var response = await _httpClient.GetAsync(_apiUrl + "List");
            var lista = new List<Factura>();

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                lista = JsonConvert.DeserializeObject<List<Factura>>(data) ?? new List<Factura>();
            }
            else
            {
                ViewBag.Error = "No se pudieron cargar las facturas.";
            }

            return View(lista);
        }

        // Búsqueda de una factura específica por su ID
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            var response = await _httpClient.GetAsync($"{_apiUrl}Search?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var factura = JsonConvert.DeserializeObject<Factura>(data);
                return View(factura);
            }

            ViewBag.Error = "No se pudo encontrar la factura solicitada.";
            return RedirectToAction("Index");
        }

        // Obtener factura asociada a una reservación específica
        [HttpGet]
        public async Task<IActionResult> FacturaPorReservacion(int idReservacion)
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            var response = await _httpClient.GetAsync($"{_apiUrl}PorReservacion/{idReservacion}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var factura = JsonConvert.DeserializeObject<Factura>(data);
                return View("Details", factura); // Reutilizamos la vista de detalles
            }

            ViewBag.Error = "No se encontró una factura para la reservación seleccionada.";
            return RedirectToAction("Index", "Reservacion");
        }
    }
}
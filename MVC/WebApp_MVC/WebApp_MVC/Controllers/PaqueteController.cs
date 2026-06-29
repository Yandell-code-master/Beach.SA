using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using global::WebApp_MVC.Models;

namespace WebApp_MVC.Controllers
{
    public class PaqueteController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7002/api/Paquete/";

        public PaqueteController()
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            var response = await _httpClient.GetAsync(_apiUrl + "List");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<Paquete>>(data);
                return View(lista);
            }
            return View(new List<Paquete>());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Paquete paquete)
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            var content = new StringContent(JsonConvert.SerializeObject(paquete), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl + "Create", content);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!OrganizarToken()) return RedirectToAction("Index", "Login");

            var response = await _httpClient.DeleteAsync($"{_apiUrl}Delete?id={id}");
            return RedirectToAction("Index");
        }
    }
}
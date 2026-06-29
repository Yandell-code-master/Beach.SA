using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers
{
    public class FacturaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp_MVC.Controllers
{
    public class ReservacionController : Controller
    {
        // GET: ReservacionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReservacionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReservacionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReservacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservacionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReservacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservacionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReservacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

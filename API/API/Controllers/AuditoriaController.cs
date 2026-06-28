using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class AuditoriaController : ControllerBase
    {
        private readonly DbContextBeach _dbContextBeach;

        public AuditoriaController(DbContextBeach dbContextBeach)
        {
            _dbContextBeach = dbContextBeach;
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List() =>
            Ok(_dbContextBeach.Auditorias.OrderByDescending(a => a.Fecha).Take(200).ToList());
    }
}

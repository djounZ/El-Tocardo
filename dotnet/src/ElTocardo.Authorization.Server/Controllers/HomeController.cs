using Microsoft.AspNetCore.Mvc;

namespace ElTocardo.Authorization.Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

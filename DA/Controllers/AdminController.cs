using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

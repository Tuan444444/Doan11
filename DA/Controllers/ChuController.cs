using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers
{
    public class ChuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

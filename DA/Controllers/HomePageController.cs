﻿using Microsoft.AspNetCore.Mvc;

namespace DA.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

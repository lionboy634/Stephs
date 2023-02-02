using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stephs_Shop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Stephs_Shop.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register()
        {
            if (ModelState.IsValid)
            {

            }
            return View();
        }

        [HttpPost]
        public IActionResult Login()
        {
            if (ModelState.IsValid)
            {

            }

            return View();
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateContact()
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

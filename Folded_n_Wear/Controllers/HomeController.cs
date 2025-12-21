using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Folded_n_Wear.Models;
using Microsoft.EntityFrameworkCore;
using Folded_n_Wear.Data;

namespace Folded_n_Wear.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Privacy()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            return View();
        }



        public IActionResult HomePage()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                return RedirectToAction("HomePage", "Account");
            }

            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");

            return View();
        }

        public IActionResult Items()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                return RedirectToAction("HomePage", "Account");
            }
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.custName = HttpContext.Session.GetString("custName");
            ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
            ViewBag.address = HttpContext.Session.GetString("address");
            return View();
        }
        public IActionResult About()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.custName = HttpContext.Session.GetString("custName");
            ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
            ViewBag.address = HttpContext.Session.GetString("address");
            return View();
        }
        public IActionResult Policy()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.custName = HttpContext.Session.GetString("custName");
            ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
            ViewBag.address = HttpContext.Session.GetString("address");
            return View();
        }
        public IActionResult Services()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            ViewBag.custName = HttpContext.Session.GetString("custName");
            ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
            ViewBag.address = HttpContext.Session.GetString("address");
            return View();
        }

        [HttpPost]
        public IActionResult Newpage()
        {
            List<MyData> users = _context.Users.ToList();
            return View(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
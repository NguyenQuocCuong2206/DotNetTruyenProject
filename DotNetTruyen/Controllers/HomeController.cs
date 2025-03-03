    using DotNetTruyen.Data;
    using DotNetTruyen.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    namespace DotNetTruyen.Controllers
    {
        public class HomeController : Controller
        {
            private readonly ILogger<HomeController> _logger;
            private readonly DotNetTruyenDbContext _context;

            public HomeController(ILogger<HomeController> logger,DotNetTruyenDbContext context)
            {
                _logger = logger;
                _context = context;
            }

            public IActionResult Index()
            {
                var comics = _context.Comics.ToList();

                return View(comics);
            }

            public IActionResult Privacy()
            {
                return View();
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }

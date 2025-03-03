using DotNetTruyen.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotNetTruyen.Controllers.Comic
{
    public class SearchController : Controller
    {
        private readonly DotNetTruyenDbContext _context;
        public SearchController(DotNetTruyenDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string keyword)
        {
            return View();
        }
    }
}

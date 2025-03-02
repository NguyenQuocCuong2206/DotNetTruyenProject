using DotNetTruyen.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotNetTruyen.Controllers.Comic
{
    public class DetailController : Controller
    {
        private readonly DotNetTruyenDbContext _context;
        public DetailController(DotNetTruyenDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            
            return View();
        }
    }
}

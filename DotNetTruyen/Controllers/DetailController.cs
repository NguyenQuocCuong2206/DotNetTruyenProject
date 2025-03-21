using DotNetTruyen.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetTruyen.Controllers
{
    public class DetailController : Controller
    {
        private readonly DotNetTruyenDbContext _context;

        public DetailController(DotNetTruyenDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(Guid id)
        {
            var comic = _context.Comics
                .Include(c => c.Chapters)
                .Include(c => c.ComicGenres)
                    .ThenInclude(cg => cg.Genre)
                .Include(c => c.Follows)
                .FirstOrDefault(c => c.Id == id);

            if (comic == null)
                return NotFound();

            // Tăng lượt xem
            comic.View += 1;
            _context.SaveChanges(); // Lưu thay đổi vào database

            var comments = _context.Comments
                .Include(c => c.User)
                .Where(c => c.ComicId == id)
                .OrderByDescending(c => c.Date)
                .ToList();

            ViewBag.Comments = comments;

            return View(comic);
        }
    }
}

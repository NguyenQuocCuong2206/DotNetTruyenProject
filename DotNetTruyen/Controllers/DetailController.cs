using DotNetTruyen.Data;
using DotNetTruyen.Models;
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
		public IList<Comic> Comics { get; set; }
        public IActionResult Index(Guid id, int page = 1, string sortOrder = "desc")
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
            _context.SaveChanges();

            // Lấy danh sách chương
            int pageSize = 5; // Số chương mỗi trang
            var chaptersQuery = comic.Chapters
                .Where(c => c.IsPublished && c.DeletedAt == null);

            // Sắp xếp theo sortOrder
            var orderedChapters = sortOrder == "asc"
                ? chaptersQuery.OrderBy(c => c.ChapterNumber).ToList()
                : chaptersQuery.OrderByDescending(c => c.ChapterNumber).ToList();

            // Phân trang
            int totalChapters = orderedChapters.Count;
            int totalPages = (int)Math.Ceiling(totalChapters / (double)pageSize);

            var pagedChapters = orderedChapters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var comments = _context.Comments
                .Include(c => c.User)
                .Where(c => c.ComicId == id)
                .OrderByDescending(c => c.Date)
                .ToList();

            ViewBag.Comments = comments;
            ViewBag.Comics = _context.Comics
                .Where(c => c.Id != id)
                .OrderBy(c => Guid.NewGuid())
                .Take(10)
                .ToList();

            // Truyền thông tin phân trang và sortOrder vào ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalChapters = totalChapters;
            ViewBag.PageSize = pageSize;
            ViewBag.SortOrder = sortOrder;

            // Gán danh sách chương đã phân trang vào Model.Chapters
            comic.Chapters = pagedChapters;

            return View(comic);
        }
        public async Task<IActionResult> ReadFromBeginning(Guid comicId)
        {
            var firstChapter = await _context.Chapters
                .Where(c => c.ComicId == comicId &&
                       c.IsPublished &&
                       c.DeletedAt == null)
                .OrderBy(c => c.ChapterNumber)
                .FirstOrDefaultAsync();

            if (firstChapter == null)
            {
                TempData["ErrorMessage"] = "Không có chương nào được tìm thấy.";
                return RedirectToAction("Index", "Detail", new { id = comicId });
            }

            return RedirectToAction("Index", new { id = firstChapter.Id });
        }
        public async Task<IActionResult> ReadLastChapter(Guid comicId)
        {
            var lastChapter = await _context.Chapters
                .Where(c => c.ComicId == comicId &&
                       c.IsPublished &&
                       c.DeletedAt == null)
                .OrderByDescending(c => c.ChapterNumber)
                .FirstOrDefaultAsync();

            if (lastChapter == null)
            {
                TempData["ErrorMessage"] = "Không có chương nào được tìm thấy.";
                return RedirectToAction("Index", "Detail", new { id = comicId });
            }

            return RedirectToAction("Index", new { id = lastChapter.Id });
        }


    }
}

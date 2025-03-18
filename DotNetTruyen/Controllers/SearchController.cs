using DotNetTruyen.Data;
using DotNetTruyen.Models;
using DotNetTruyen.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DotNetTruyen.Controllers
{
    public class SearchController : Controller
    {
        private readonly DotNetTruyenDbContext _context;

        public SearchController(DotNetTruyenDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index(string keyword, string sortBy = "latest", string genre = null, string status = null, int page = 1)
        {
            const int pageSize = 16;

            var query = _context.Comics
                .Where(c => c.DeletedAt == null)
                .Include(c => c.Chapters.Where(ch => ch.DeletedAt == null))
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(c => c.ComicGenres.Any(cg => cg.Genre.GenreName == genre));
            }

            if (!string.IsNullOrEmpty(status))
            {
                bool? statusValue = status.ToLower() == "completed" ? true : status.ToLower() == "ongoing" ? (bool?)false : null;
                if (statusValue.HasValue)
                {
                    query = query.Where(c => c.Status == statusValue.Value);
                }
            }

            switch (sortBy?.ToLower())
            {
                case "alphabet":
                    query = query.OrderBy(c => c.Title);
                    break;
                case "rating":
                    query = query.OrderByDescending(c => c.View);
                    break;
                case "trending":
                    query = query.OrderByDescending(c => c.View);
                    break;
                case "views":
                    query = query.OrderByDescending(c => c.View);
                    break;
                case "new-manga":
                    query = query.OrderByDescending(c => c.CreatedAt);
                    break;
                case "latest":
                default:
                    query = query.OrderByDescending(c => c.CreatedAt);
                    break;
            }

            var totalItems = query.Count();
            var comics = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new SearchViewModel
            {
                Comics = comics,
                TotalResults = totalItems,
                Keyword = keyword,
                SortBy = sortBy,
                Status = status,
                AllGenres = _context.Genres.Where(g => g.DeletedAt == null).ToList(),
                Page = page,
                GenreFilter = genre // Lưu giá trị bộ lọc thể loại
            };

            return View(viewModel);
        }
    }
}

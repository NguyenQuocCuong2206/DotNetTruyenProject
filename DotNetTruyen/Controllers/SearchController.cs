using DotNetTruyen.Data;
using DotNetTruyen.Models;
using DotNetTruyen.ViewModels;
using DotNetTruyen.ViewModels.Management;
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

        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery = "", string genre = "", string status = "", int page = 1, int pageSize = 8)
        {
            var query = _context.Comics
                .Where(c => c.DeletedAt == null)
                .Include(c => c.ComicGenres)
                .ThenInclude(cg => cg.Genre)
                .AsQueryable();

            // Ưu tiên xử lý searchQuery nếu nó không rỗng
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(c => c.Title.Contains(searchQuery));
            }

            // Chỉ áp dụng genre nếu có giá trị hợp lệ
            if (!string.IsNullOrEmpty(genre))
            {
                genre = Uri.UnescapeDataString(genre);
                Console.WriteLine($"Filtering by genre: {genre}");
                query = query.Where(c => c.ComicGenres.Any(cg => cg.Genre.GenreName.ToLower() == genre.ToLower()));
            }

            // Chỉ áp dụng status nếu có giá trị hợp lệ
            if (!string.IsNullOrEmpty(status))
            {
                bool? statusFilter = status.ToLower() switch
                {
                    "completed" => true,
                    "ongoing" => false,
                    _ => null
                };
                if (statusFilter.HasValue)
                {
                    query = query.Where(c => c.Status == statusFilter.Value);
                }
            }

            var totalItems = await query.CountAsync();
            var comics = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ViewModels.ComicViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CoverImage = c.CoverImage ?? "/images/default-cover.jpg",
                    Author = c.Author,
                    ViewCount = c.View,
                    Status = c.Status ? "Completed" : "Ongoing",
                    Genres = c.ComicGenres.Select(cg => cg.Genre.GenreName).ToList(),
                    
                    
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var genres = await _context.Genres
                .Where(g => g.DeletedAt == null)
                .Select(g => g.GenreName)
                .ToListAsync();

            ViewBag.SearchQuery = searchQuery;
            ViewBag.Genre = genre;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Genres = genres;

            return View(comics);
        }
    }
}


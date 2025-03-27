using DotNetTruyen.Data;
using DotNetTruyen.Models;
using DotNetTruyen.Services;
using DotNetTruyen.ViewModels.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DotNetTruyen.Controllers.Admin
{
    [Authorize(Policy = "CanAccessDashboard")]
    public class DashBoardController : Controller
    {
        
        private readonly DotNetTruyenDbContext _context;
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DashBoardController> _logger;

        public DashBoardController(DotNetTruyenDbContext context, UserService userService, UserManager<User> userManager, ILogger<DashBoardController> logger)
        {
            _context = context;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            
            var now = DateTime.UtcNow;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart.AddDays(-1);

            
            var totalComics = await _context.Comics.CountAsync(c => c.DeletedAt == null);
            var totalChapters = await _context.Chapters.CountAsync(c => c.DeletedAt == null);
            var totalViews = await _context.Chapters.SumAsync(c => c.Views);
            var totalUsers = await _context.Users.CountAsync();

            
            var previousComics = await _context.Comics
                .CountAsync(c => c.DeletedAt == null && c.CreatedAt < currentMonthStart);
            var previousChapters = await _context.Chapters
                .CountAsync(c => c.DeletedAt == null && c.CreatedAt < currentMonthStart);
            var previousViews = await _context.Chapters
                .Where(c => c.DeletedAt == null && c.PublishedDate.HasValue && c.PublishedDate.Value < currentMonthStart)
                .SumAsync(c => c.Views);

            
            double comicsChangePercentage = previousComics > 0 ?
                ((double)(totalComics - previousComics) / previousComics * 100) : 0;
            double chaptersChangePercentage = previousChapters > 0 ?
                ((double)(totalChapters - previousChapters) / previousChapters * 100) : 0;
            double viewsChangePercentage = previousViews > 0 ?
                ((double)(totalViews - previousViews) / previousViews * 100) : 0;
            double usersChangePercentage = 0; 

            
            var recentChapters = await _context.Chapters
                .Include(c => c.Comic)
                .Where(c => c.DeletedAt == null)
                .OrderByDescending(c => c.CreatedAt)
                .Take(6)
                .Select(c => new RecentChapterViewModel
                {
                    Id = c.Id,
                    ChapterTitle = c.ChapterTitle,
                    ComicTitle = c.Comic.Title,
                    PublishedDate = c.PublishedDate,
                    IsPublished = c.IsPublished,
                    Thumbnail = c.Comic.CoverImage
                })
                .ToListAsync();

            var topGenres = await _context.Genres
                .Include(g => g.ComicGenres)
                .Where(g => g.DeletedAt == null)
                .Select(g => new TopGenreViewModel
                {
                    GenreName = g.GenreName,
                    ComicCount = g.ComicGenres.Count(cg => cg.Comic.DeletedAt == null),
                    NewComics = g.ComicGenres.Count(cg => cg.Comic.CreatedAt >= DateTime.Now.AddMonths(-1) && cg.Comic.DeletedAt == null)
                })
                .OrderByDescending(g => g.ComicCount)
                .Take(5)
                .ToListAsync();

            var viewsByMonthRaw = await _context.Chapters
                .Where(c => c.DeletedAt == null && c.PublishedDate.HasValue)
                .GroupBy(c => new { c.PublishedDate.Value.Year, c.PublishedDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalViews = g.Sum(c => c.Views)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync();

            var lastSixMonths = viewsByMonthRaw
                .OrderByDescending(v => v.Year * 100 + v.Month)
                .Take(6)
                .OrderBy(v => v.Year * 100 + v.Month)
                .Select(v => new { YearMonth = $"{v.Month}/{v.Year}", v.TotalViews })
                .ToList();

            var previousSixMonths = viewsByMonthRaw
                .OrderByDescending(v => v.Year * 100 + v.Month)
                .Skip(6)
                .Take(6)
                .OrderBy(v => v.Year * 100 + v.Month)
                .Select(v => new { YearMonth = $"{v.Month}/{v.Year}", v.TotalViews })
                .ToList();

            var viewModel = new DashboardViewModel
            {
                TotalComics = totalComics,
                TotalChapters = totalChapters,
                TotalViews = totalViews,
                TotalUsers = totalUsers,
                ComicsChangePercentage = comicsChangePercentage,
                ChaptersChangePercentage = chaptersChangePercentage,
                ViewsChangePercentage = viewsChangePercentage,
                UsersChangePercentage = usersChangePercentage, // Sẽ là 0
                RecentChapters = recentChapters,
                TopGenres = topGenres,
                ViewsByMonthLabels = lastSixMonths.Select(v => v.YearMonth).ToList(),
                ViewsByMonthData = lastSixMonths.Select(v => v.TotalViews).ToList(),
                PreviousViewsByMonthLabels = previousSixMonths.Select(v => v.YearMonth).ToList(),
                PreviousViewsByMonthData = previousSixMonths.Select(v => v.TotalViews).ToList()
            };

            return View("~/Views/Admin/Dashboard/Index.cshtml", viewModel);
        }




        // GET: DashBoardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashBoardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashBoardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashBoardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashBoardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashBoardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

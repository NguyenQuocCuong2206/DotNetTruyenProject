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
    //[Authorize(Policy = "CanAccessDashboard")]
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
            
            var totalComics = await _context.Comics.CountAsync(c => c.DeletedAt == null);
            var totalChapters = await _context.Chapters.CountAsync(c => c.DeletedAt == null);
            var totalViews = await _context.Chapters.SumAsync(c => c.Views);
            var totalUsers = await _context.Users.CountAsync();

            
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
                    Thumbnail =  c.Comic.CoverImage
                })
                .ToListAsync();

            // Thể loại nổi bật
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

            // Lấy 6 tháng trước đó
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
                RecentChapters = recentChapters,
                TopGenres = topGenres,
                ViewsByMonthLabels = lastSixMonths.Select(v => v.YearMonth).ToList(),
                ViewsByMonthData = lastSixMonths.Select(v => v.TotalViews).ToList(),
                PreviousViewsByMonthLabels = previousSixMonths.Select(v => v.YearMonth).ToList(),
                PreviousViewsByMonthData = previousSixMonths.Select(v => v.TotalViews).ToList()
            };

            return View("~/Views/Admin/Dashboard/Index.cshtml", viewModel);
        }


        // GET: DashBoardController/Details/5
        public async Task<IActionResult> Details()
        {
            var claims = User.Claims;
            await _userService.IncreaseExpAsync(_context, Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            
            foreach (var claim in claims)
            {
                _logger.LogWarning($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }


            return View("~/Views/Admin/Dashboard/Detail.cshtml", claims);
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

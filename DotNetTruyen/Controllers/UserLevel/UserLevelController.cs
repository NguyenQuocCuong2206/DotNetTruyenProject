using DotNetTruyen.Data;
using DotNetTruyen.Hubs;
using DotNetTruyen.Models;
using DotNetTruyen.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace DotNetTruyen.Controllers.UserLevel
{
    public class UserLevelController : Controller
    {

        private readonly DotNetTruyenDbContext _context;
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private const int PageSize = 10;

        public UserLevelController(DotNetTruyenDbContext context, UserService userService, UserManager<User> userManager, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _userService = userService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet("/userProfile/level")]
        
       
            public async Task<IActionResult> Index(int page = 1)
            {

                int unreadCount = await _context.Notifications
                    .CountAsync(n => n.DeletedAt == null && !n.IsRead && n.UserId == null);
                ViewBag.UnreadCount = unreadCount;


                int totalNotifications = await _context.Notifications
                .CountAsync(n => n.DeletedAt == null && n.UserId == null);

                int totalPages = (int)Math.Ceiling((double)totalNotifications / PageSize);
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;


                var notifications = await _context.Notifications
                    .Where(n => n.DeletedAt == null && n.UserId == null)
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                return View("~/Views/User/UserLevel/Index.cshtml", notifications);
            }
            

       

        // GET: UserLevelController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserLevelController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserLevelController/Create
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

        // GET: UserLevelController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserLevelController/Edit/5
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

        // GET: UserLevelController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserLevelController/Delete/5
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

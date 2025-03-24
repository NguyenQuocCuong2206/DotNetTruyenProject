
using DotNetTruyen.Data;
using DotNetTruyen.Hubs;
using DotNetTruyen.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace DotNetTruyen.Controllers
{
    public class DetailController : Controller
    {
        private readonly DotNetTruyenDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IHubContext<CommentHub> _hubContext;

        public DetailController(DotNetTruyenDbContext context, UserManager<User> userManager, ICompositeViewEngine viewEngine, IHubContext<CommentHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _viewEngine = viewEngine;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var comic = await _context.Comics
                .Include(c => c.Chapters)
                .Include(c => c.ComicGenres)
                    .ThenInclude(cg => cg.Genre)
                .Include(c => c.Follows)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comic == null)
                return NotFound();

            // Tăng lượt xem
            //comic.View += 1;
            //_context.SaveChanges(); // Lưu thay đổi vào database

            const int pageSize = 5;
            var topLevelComments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ComicId == id && c.CommentId == null)
                .OrderByDescending(c => c.Date)
                .Take(pageSize)
                .ToListAsync();

            // Calculate the total number of top-level comments for pagination
            var totalTopLevelComments = await _context.Comments
                .CountAsync(c => c.ComicId == id && c.CommentId == null);

            foreach (var comment in topLevelComments)
            {
                comment.ReplyCount = await _context.Comments.CountAsync(c => c.CommentId == comment.Id);
            }

            ViewBag.Comments = topLevelComments;
            ViewBag.TotalComments = totalTopLevelComments;
            ViewBag.PageSize = pageSize;
            ViewBag.ComicId = id;
            return View(comic);
        }

        [HttpGet]
        public async Task<IActionResult> GetMoreComments(Guid comicId, int skip)
        {
            const int pageSize = 5;

            // Fetch the next batch of top-level comments
            var comments =await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ComicId == comicId && c.CommentId == null)
                .OrderByDescending(c => c.Date)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            foreach (var comment in comments)
            {
                comment.ReplyCount = await  _context.Comments.CountAsync(c => c.CommentId == comment.Id);
            }

            // Return the comments as a partial view
            return PartialView("_CommentPartial", comments);
        }

        [HttpPost]
        public async Task<IActionResult> Comment(Guid comicId, string content, Guid? parentId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để bình luận." });
            }

            var comment = new Comment
            {
                Content = content,
                Date = DateTime.Now,
                UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                ComicId = comicId,
                CommentId = parentId,
                ReplyCount = 0 // New comment has no replies initially
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Broadcast the reload message to all clients
            await _hubContext.Clients.Group($"Comic_{comicId}").SendAsync("ReloadComments", comicId);

            return Json(new { success = true });
        }

        // Helper method to render a partial view to a string
        private async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReplies(Guid commentId)
        {
            // Fetch replies for the given commentId
            var replies = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Comments) // Include nested replies if needed
                .ThenInclude(r => r.User)
                .Where(c => c.CommentId == commentId)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            foreach (var comment in replies)
            {
                comment.ReplyCount = await _context.Comments.CountAsync(c => c.CommentId == comment.Id);
            }

            // Return the replies as a partial view
            return PartialView("_CommentPartial", replies);
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(Guid commentId, string content)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var comment = _context.Comments
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            // Check if the current user is the author of the comment
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId.ToString() != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Update the comment content
            comment.Content = content;
            comment.Date = DateTime.Now; // Update the timestamp (optional)
            await _context.SaveChangesAsync();

            // Broadcast the reload message to all clients
            await _hubContext.Clients.Group($"Comic_{comment.ComicId}").SendAsync("ReloadComments", comment.ComicId);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            // Lấy comment để kiểm tra quyền
            var comment = await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền xóa
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId.ToString() != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var comicId = comment.ComicId;

            // Xóa tất cả comment con bằng SQL an toàn hơn
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
        WITH CommentHierarchy AS (
            SELECT Id FROM Comments WHERE Id = {commentId}
            UNION ALL
            SELECT c.Id FROM Comments c
            INNER JOIN CommentHierarchy ch ON c.ReplyId = ch.Id
        )
        DELETE FROM Comments WHERE Id IN (SELECT Id FROM CommentHierarchy);");

            // Gửi tín hiệu reload đến tất cả client
            await _hubContext.Clients.Group($"Comic_{comicId}").SendAsync("ReloadComments", comicId);

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetComments(Guid comicId, int skip = 0)
        {
            const int pageSize = 5;

            var comments = _context.Comments
                .Include(c => c.User)
                .Where(c => c.ComicId == comicId && c.CommentId == null)
                .OrderByDescending(c => c.Date)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            var totalTopLevelComments = _context.Comments
                .Count(c => c.ComicId == comicId && c.CommentId == null);

            foreach (var comment in comments)
            {
                comment.ReplyCount = _context.Comments.Count(c => c.CommentId == comment.Id);
            }

            return Json(new
            {
                html = RenderPartialViewToString("_CommentPartial", comments),
                totalComments = totalTopLevelComments
            });
        }
    }
}

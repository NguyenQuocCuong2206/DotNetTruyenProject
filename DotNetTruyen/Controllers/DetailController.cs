using DotNetTruyen.Data;
using DotNetTruyen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
		public IActionResult Index(Guid id)
		{
			var comic = _context.Comics
				.Include(c => c.Chapters)
				.Include(c => c.ComicGenres)
					.ThenInclude(cg => cg.Genre)
				.Include(c => c.Follows)
                .Include(c => c.Likes)
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
			ViewBag.Comics = _context.Comics
			 .Where(c => c.Id != id)
			 .OrderBy(c => Guid.NewGuid())
			.Take(10)
		 .ToList();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.IsFollowing = _context.Follows.Any(f => f.ComicId == id && f.UserId == Guid.Parse(userId));
                ViewBag.IsLiked = _context.Likes.Any(l => l.ComicId == id && l.UserId == Guid.Parse(userId));
            }
            else
            {
                ViewBag.IsFollowing = false;
                ViewBag.IsLiked = false;
            }


            return View(comic);


		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleFollow([FromBody] FollowRequestModel request)
        {
            try
            {
                Console.WriteLine($"Received Comic ID: {request.Id}");
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để theo dõi" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"User ID: {userId}");
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return Json(new { success = false, message = "Không thể xác định người dùng" });
                }

                // Kiểm tra Comic tồn tại
                var comic = _context.Comics.FirstOrDefault(c => c.Id == request.Id);
                if (comic == null)
                {
                    Console.WriteLine($"Comic with ID {request.Id} not found.");
                    return Json(new { success = false, message = "Không tìm thấy truyện" });
                }

                var follow = _context.Follows
                    .FirstOrDefault(f => f.ComicId == request.Id && f.UserId == parsedUserId);

                bool wasFollowing = follow != null;

                if (!wasFollowing)
                {
                    follow = new Follow
                    {
                        ComicId = request.Id,
                        UserId = parsedUserId,
                    };
                    _context.Follows.Add(follow);
                }
                else
                {
                    _context.Follows.Remove(follow);
                }

                _context.SaveChanges();

                var followCount = _context.Follows.Count(f => f.ComicId == request.Id);
                return Json(new
                {
                    success = true,
                    isFollowing = !wasFollowing,
                    followCount = followCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleFollow: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleLike([FromBody] LikeRequestModel request)
        {
            try
            {
                Console.WriteLine($"Received Comic ID for Like: {request.Id}");
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thích" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"User ID: {userId}");
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return Json(new { success = false, message = "Không thể xác định người dùng" });
                }

                var comic = _context.Comics.FirstOrDefault(c => c.Id == request.Id);
                if (comic == null)
                {
                    Console.WriteLine($"Comic with ID {request.Id} not found.");
                    return Json(new { success = false, message = "Không tìm thấy truyện" });
                }

                var like = _context.Likes
                    .FirstOrDefault(l => l.ComicId == request.Id && l.UserId == parsedUserId);

                bool wasLiked = like != null;

                if (!wasLiked)
                {
                    like = new Like
                    {
                        ComicId = request.Id,
                        UserId = parsedUserId,
                    };
                    _context.Likes.Add(like);
                }
                else
                {
                    _context.Likes.Remove(like);
                }

                _context.SaveChanges();

                var likeCount = _context.Likes.Count(l => l.ComicId == request.Id);
                return Json(new
                {
                    success = true,
                    isLiked = !wasLiked,
                    likeCount = likeCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ToggleLike: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        // Tạo class request model
        public class FollowRequestModel
        {
            public Guid Id { get; set; }
        }

        public class LikeRequestModel
        {
            public Guid Id { get; set; }
        }




    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetTruyen.Data;
using DotNetTruyen.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DotNetTruyen.Controllers
{
    [Authorize]
    public class ReadHistoriesController : Controller
    {
        private readonly DotNetTruyenDbContext _context;

        public ReadHistoriesController(DotNetTruyenDbContext context)
        {
            _context = context;
        }

        // GET: ReadHistories
        [HttpGet("/readHistory")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _context.ReadHistories
                .Include(r => r.Chapter)
                .ThenInclude(c => c.Comic)
                .Include(r => r.User)
                .Where(r => r.UserId.ToString() == userId)
                //.GroupBy(r => r.Chapter.ComicId).Select(g => g.OrderByDescending(r => r.ReadDate).FirstOrDefault())
                .ToListAsync();
            ViewBag.HistoryTab = "active";
            return View("~/Views/User/ReadHistory.cshtml", history);
        }


        [HttpGet("/deleteHistory")]
        // GET: ReadHistories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readHistory = await _context.ReadHistories
                .Include(r => r.Chapter)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReadHistoryId == id);
            if (readHistory == null)
            {
                return NotFound();
            }
            _context.ReadHistories.Remove(readHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}

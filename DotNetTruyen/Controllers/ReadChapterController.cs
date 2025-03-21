using DotNetTruyen.Data;
using DotNetTruyen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetTruyen.Controllers
{
    public class ReadChapterController : Controller
    {
        private readonly DotNetTruyenDbContext _context;

        public ReadChapterController(DotNetTruyenDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChapterImage>> GetPublishedChapterImagesAsync()
        {
            return await _context.ChapterImages
                .Where(img => img.Chapter.IsPublished)
                .ToListAsync();
        }

    }
}

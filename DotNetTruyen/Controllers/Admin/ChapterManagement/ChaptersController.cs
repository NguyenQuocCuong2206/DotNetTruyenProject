using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetTruyen.Data;
using DotNetTruyen.Models;
using DotNetTruyen.ViewModels.Management;
using DotNetTruyen.Services;

namespace DotNetTruyen.Controllers.Admin.ChapterManagement
{
    public class ChaptersController : Controller
    {
        private readonly DotNetTruyenDbContext _context;
        private readonly IPhoToService _imageUploadService;

        public ChaptersController(DotNetTruyenDbContext context, IPhoToService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
        }


        // GET: Chapters
        public async Task<IActionResult> Index(Guid? comicId, string search, int page = 1, int pageSize = 10)
        {
            if (comicId == null) return NotFound();

            var query = _context.Chapters
                .Where(c => c.ComicId == comicId)
                .OrderBy(c => c.ChapterNumber)
                .Select(c => new ChapterViewModel
                {
                    Id = c.Id,
                    ChapterTitle = c.ChapterTitle,
                    ChapterNumber = c.ChapterNumber,
                    PublishedDate = c.PublishedDate,
                    Views = c.Views,
                    ComicId = c.ComicId
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.ChapterTitle.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var chapters = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            ViewBag.ComicId = comicId;

            return View("~/Views/Admin/Chapters/Index.cshtml", chapters);
        }

        // GET: Chapters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters
                .Include(c => c.Comic)
                .FirstOrDefaultAsync(m => m.ChapterNumber == id);
            if (chapter == null)
            {
                return NotFound();
            }

            return View(chapter);
        }

        // GET: Chapters/Create
        public IActionResult Create(Guid comicId)
        {
            var model = new CreateChapterViewModel { ComicId = comicId };
            return View("~/Views/Admin/Chapters/Create.cshtml",model);
        }

        // POST: Chapters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateChapterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Chapters/Create.cshtml", model);
            }

            var chapter = new Chapter
            {
                Id = Guid.NewGuid(),
                ChapterTitle = model.ChapterTitle,
                ChapterNumber = model.ChapterNumber,
                PublishedDate = model.PublishedDate,
                Views = 0,
                ComicId = model.ComicId,
                Images = new List<ChapterImage>()
            };

            
            if (model.Images != null && model.Images.Count > 0)
            {
                var imageUrls = await _imageUploadService.AddListPhotoAsync(model.Images);
                foreach (var imageUrl in imageUrls)
                {
                    chapter.Images.Add(new ChapterImage
                    {
                        Id = Guid.NewGuid(),
                        ChapterId = chapter.Id,
                        ImageUrl = imageUrl
                    });
                }
            }

            _context.Add(chapter);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { comicId = model.ComicId });
        }

        // GET: Chapters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return NotFound();
            }
            ViewData["ComicId"] = new SelectList(_context.Comics, "Id", "Id", chapter.ComicId);
            return View(chapter);
        }

        // POST: Chapters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChapterTitle,ChapterNumber,ComicId,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,DeletedAt")] Chapter chapter)
        {
            if (id != chapter.ChapterNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chapter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChapterExists(chapter.ChapterNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ComicId"] = new SelectList(_context.Comics, "Id", "Id", chapter.ComicId);
            return View(chapter);
        }

        // GET: Chapters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await _context.Chapters
                .Include(c => c.Comic)
                .FirstOrDefaultAsync(m => m.ChapterNumber == id);
            if (chapter == null)
            {
                return NotFound();
            }

            return View(chapter);
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter != null)
            {
                _context.Chapters.Remove(chapter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChapterExists(int id)
        {
            return _context.Chapters.Any(e => e.ChapterNumber == id);
        }
    }
}

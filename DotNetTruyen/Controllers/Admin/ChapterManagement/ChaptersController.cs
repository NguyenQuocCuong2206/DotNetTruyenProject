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
        private readonly ILogger<ChaptersController> _logger;

        public ChaptersController(DotNetTruyenDbContext context, IPhoToService imageUploadService, ILogger<ChaptersController> logger)
        {
            _context = context;
            _imageUploadService = imageUploadService;
            _logger = logger;
        }



        // GET: Chapters
        public async Task<IActionResult> Index(Guid? comicId, string searchQuery, int page = 1, int pageSize = 10)
        {
            if (comicId == null) return NotFound();

            
            var comicExists = await _context.Comics.AnyAsync(c => c.Id == comicId && c.DeletedAt == null);
            if (!comicExists)
            {
                return NotFound();
            }

            
            var query = _context.Chapters
                .Where(c => c.ComicId == comicId && c.DeletedAt == null)
                .OrderBy(c => c.ChapterNumber)
                .Select(c => new ChapterViewModel
                {
                    Id = c.Id,
                    ChapterTitle = c.ChapterTitle,
                    ChapterNumber = c.ChapterNumber,
                    PublishedDate = c.PublishedDate,
                    IsPublished = c.IsPublished,
                    Views = c.Views,
                    ComicId = c.ComicId
                })
                .AsQueryable();


            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim().ToLower();
                
                if (int.TryParse(searchQuery, out int chapterNumber))
                {
                    query = query.Where(c => c.ChapterTitle.ToLower().Contains(searchQuery) || c.ChapterNumber == chapterNumber);
                }
                else
                {
                    query = query.Where(c => c.ChapterTitle.ToLower().Contains(searchQuery));
                }
            }


            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var chapters = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            
            var viewModel = new ChapterIndexViewModel
            {
                ChapterViewModels = chapters,
                SearchQuery = searchQuery,
                CurrentPage = page,
                TotalPages = totalPages,
                ComicId = comicId.Value
            };

            return View("~/Views/Admin/Chapters/Index.cshtml", viewModel);
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

            var comicExists = await _context.Comics.AnyAsync(c => c.Id == model.ComicId && c.DeletedAt == null);
            if (!comicExists)
            {
                TempData["ErrorMessage"] = "Truyện không tồn tại hoặc đã bị xóa.";
                return View("~/Views/Admin/Chapters/Create.cshtml", model);
            }
            var chapterExists = await _context.Chapters
            .AnyAsync(c => c.ComicId == model.ComicId
                    && c.ChapterNumber == model.ChapterNumber
                    && c.DeletedAt == null);
            if (chapterExists)
            {
                TempData["ErrorMessage"] = $"Chapter số {model.ChapterNumber} đã tồn tại trong truyện này. Vui lòng chọn số chapter khác.";
                return View("~/Views/Admin/Chapters/Create.cshtml", model);
            }
            var chapter = new Chapter
            {
                Id = Guid.NewGuid(),
                ChapterTitle = model.ChapterTitle,
                ChapterNumber = model.ChapterNumber,
                PublishedDate = model.PublishedDate.HasValue ? model.PublishedDate.Value.ToUniversalTime() : DateTime.UtcNow,
                Views = 0,
                IsPublished = model.PublishedDate.HasValue ? true : false,
                ComicId = model.ComicId
            };

            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync(); 

            if (model.Images != null && model.Images.Count > 0)
            {
                var imageUrls = await _imageUploadService.AddListPhotoAsync(model.Images);
                var orders = !string.IsNullOrEmpty(model.ImageOrders)
                    ? model.ImageOrders.Split(',').Select(int.Parse).ToList()
                    : Enumerable.Range(0, imageUrls.Count).ToList();

                var chapterImages = new List<ChapterImage>();
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    chapterImages.Add(new ChapterImage
                    {
                        Id = Guid.NewGuid(),
                        ChapterId = chapter.Id,
                        ImageUrl = imageUrls[i],
                        ImageNumber = orders[i] 
                    });
                }

                _context.ChapterImages.AddRange(chapterImages);
                await _context.SaveChangesAsync(); 
            }

            TempData["SuccessMessage"] = "Chương đã được tạo thành công!";
            return RedirectToAction("Index", new { comicId = model.ComicId });
        }

        // GET: Chapters/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null)
            {
                return NotFound();
            }

            var viewModel = new EditChapterViewModel
            {
                Id = chapter.Id,
                ComicId = chapter.ComicId,
                ChapterTitle = chapter.ChapterTitle,
                ChapterNumber = chapter.ChapterNumber,
                PublishedDate = chapter.PublishedDate,
                ExistingImages = chapter.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>()
            };

            return View("~/Views/Admin/Chapters/Edit.cshtml", viewModel);
        }

        // POST: Chapters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditChapterViewModel model, string imageOrders)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("ModelState is invalid");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        _logger.LogError($"ModelState Error: {modelStateKey} - {error.ErrorMessage}");
                    }
                }
                return View("~/Views/Admin/Chapters/Edit.cshtml", model);
            }

            var chapter = await _context.Chapters
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (chapter == null)
            {
                return NotFound();
            }
                
         
            chapter.ChapterTitle = model.ChapterTitle;
            chapter.ChapterNumber = model.ChapterNumber;
            chapter.PublishedDate = model.PublishedDate;
            

            if (!string.IsNullOrEmpty(model.DeletedImages))
            {
                var deletedImageUrls = model.DeletedImages.Split(',').ToList();
                var imagesToDelete = chapter.Images
                    .Where(i => deletedImageUrls.Contains(i.ImageUrl))
                    .ToList();

                foreach (var image in imagesToDelete)
                {
                    chapter.Images.Remove(image);
                    
                    try
                    {
                        await _imageUploadService.DeletePhotoAsync(image.ImageUrl);
                    }
                    catch (Exception ex)
                    {
                        
                        ModelState.AddModelError("", $"Lỗi khi xóa ảnh: {ex.Message}");
                        return View("~/Views/Chapters/Edit.cshtml", model);
                    }
                    chapter.Images.Remove(image);
                    _context.ChapterImages.Remove(image);
                }
            }

            // Xử lý thêm ảnh mới
            if (model.Images != null && model.Images.Any())
            {
                foreach (var image in model.Images)
                {
                    if (image != null && image.Length > 0)
                    {
                        try
                        {
                            var uploadResult = await _imageUploadService.AddPhotoAsync(image);
                            if (!string.IsNullOrEmpty(uploadResult))
                            {
                                chapter.Images.Add(new ChapterImage
                                {
                                    ImageUrl = uploadResult,
                                    ImageNumber = 0 
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            
                            ModelState.AddModelError("", $"Lỗi khi upload ảnh: {ex.Message}");
                            return View("~/Views/Chapters/Edit.cshtml", model);
                        }
                    }
                }
            }

            
            if (!string.IsNullOrEmpty(imageOrders))
            {
                var orderList = imageOrders.Split(',')
                    .Select((value, index) => new { Order = index, OriginalIndex = int.Parse(value) })
                    .OrderBy(x => x.OriginalIndex)
                    .Select(x => x.Order)
                    .ToList();


                if (orderList.Count == chapter.Images.Count)
                {
                    var sortedImages = chapter.Images.OrderBy(img => img.ImageNumber).ToList();


                    for (int i = 0; i < sortedImages.Count; i++)
                    {
                        sortedImages[i].ImageNumber = orderList[i];
                    }

 
                    chapter.Images.Clear();
                    foreach (var image in sortedImages)
                    {
                        chapter.Images.Add(image);
                    }
                }
            }

            // Lưu thay đổi vào database
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Chapter đã được cập nhật thành công!";
                return RedirectToAction("Index", new { comicId = model.ComicId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi lưu chapter: {ex.Message}");
                return View("~/Views/Chapters/Edit.cshtml", model);
            }
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

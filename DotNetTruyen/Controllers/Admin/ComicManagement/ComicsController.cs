using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetTruyen.Data;
using DotNetTruyen.Models;
using DotNetTruyen.ViewModels;
using DotNetTruyen.Services;

namespace DotNetTruyen.Controllers.Admin.ComicManagement
{
    public class ComicsController : Controller
    {
        private readonly DotNetTruyenDbContext _context;
        private readonly IPhoToService _photoService;

        public ComicsController(DotNetTruyenDbContext context, IPhoToService photoService)
        {
            _context = context;
            _photoService = photoService;
        }


        // GET: Comics
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Admin/Comics/Index.cshtml", await _context.Comics.ToListAsync());
        }

        // GET: Comics/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comic = await _context.Comics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comic == null)
            {
                return NotFound();
            }

            return View(comic);
        }

        // GET: Comics/Create
        public IActionResult Create()
        {


            var viewModel = new CreateComicViewModel
            {
                
                Genres = _context.Genres.Select(g => new GenreViewModel
                {
                    Id = g.Id,
                    GenreName = g.GenreName,
                    TotalStories = g.ComicGenres.Count(),
                    UpdatedAt = g.UpdatedAt
                }).ToList()
            };

            return View("~/Views/Admin/Comics/Create.cshtml", viewModel);
        }

        // POST: Comics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Author,CoverImage,Status,GenreIds")] CreateComicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Error in {state.Key}: {state.Value.Errors[0].ErrorMessage}");
                    }
                }
                // rest of your code...
            }

            var comic = new Comic
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Description = model.Description,
                Author = model.Author,
                Status = model.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ComicGenres = new List<ComicGenre>()
            };

            if (model.CoverImage == null)
            {
                Console.WriteLine("CoverImage is null");
            }

            // Upload ảnh bìa lên Cloudinary
            if (model.CoverImage != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(model.CoverImage);
                if (uploadResult != null)
                {
                    comic.CoverImage = uploadResult.Url.ToString();
                    Console.WriteLine("Image uploaded successfully: " + comic.CoverImage);
                }
                else
                {
                    Console.WriteLine("Image upload failed");
                }
            }

            // Lưu thể loại
            if (model.GenreIds != null && model.GenreIds.Any())
            {
                foreach (var genreId in model.GenreIds)
                {
                    comic.ComicGenres.Add(new ComicGenre { GenreId = genreId });
                }
            }

            _context.Comics.Add(comic);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Truyện đã được tạo thành công!";
            return RedirectToAction(nameof(Index));
        }


        // GET: Comics/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comic = await _context.Comics.FindAsync(id);
            if (comic == null)
            {
                return NotFound();
            }
            return View(comic);
        }

        // POST: Comics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Title,Description,CoverImage,Author,View,Status,Id,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,DeletedAt")] Comic comic)
        {
            if (id != comic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComicExists(comic.Id))
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
            return View(comic);
        }

        // GET: Comics/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comic = await _context.Comics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comic == null)
            {
                return NotFound();
            }

            return View(comic);
        }

        // POST: Comics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var comic = await _context.Comics.FindAsync(id);
            if (comic != null)
            {
                _context.Comics.Remove(comic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComicExists(Guid id)
        {
            return _context.Comics.Any(e => e.Id == id);
        }
    }

    public class StepModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}

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
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.SignalR;
using DotNetTruyen.Hubs;
using DotNetTruyen.ViewModels.Management;

namespace DotNetTruyen.Controllers.Admin.GenreManagement
{
    public class GenresController : Controller
    {
        private readonly DotNetTruyenDbContext _context;
        private readonly IHubContext<GenreHub> _hubContext;

        public GenresController(DotNetTruyenDbContext context, IHubContext<GenreHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }


        // GET: Genres
        public async Task<IActionResult> Index()
        {
            var comic = _context.Comics.ToList();
            var viewModel = _context.Genres.Select(g => new GenreViewModel
            {
                Id = g.Id,
                GenreName = g.GenreName,
                TotalStories = g.ComicGenres.Count(),
                UpdatedAt = DateTime.Now,

            });
            return View("~/Views/Admin/Genres/Index.cshtml", await viewModel.ToListAsync());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGenreViewModel createdGenre)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ShowModal"] = "true"; // Giữ modal mở nếu có lỗi
                return View("Index", createdGenre); // Quay lại Index với lỗi
            }

            var genre = new Genre
            {
                GenreName = createdGenre.GenreName
            };

            _context.Add(genre);
            await _context.SaveChangesAsync();
            var genreViewModel = new GenreViewModel
            {
                Id = genre.Id,
                GenreName = genre.GenreName,
                TotalStories = 0,
                UpdatedAt = DateTime.Now
            };
            await _hubContext.Clients.All.SendAsync("ReceiveGenreCreated", genreViewModel);
            return RedirectToAction("Index"); // Load lại trang sau khi thêm thành công
        }


        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .Include(g => g.ComicGenres) 
                .ThenInclude(cg => cg.Comic) 
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            // Prepare ViewModel
            var viewModel = new GenreDetailViewModel
            {
                Id = genre.Id,
                GenreName = genre.GenreName,
                SelectedStoryIds = genre.ComicGenres.Select(cg => cg.ComicId).ToList(),
                Comics = await _context.Comics.ToListAsync() // Get all comics
            };

            return View("~/Views/Admin/Genres/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GenreDetailViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var genre = await _context.Genres
                    .Include(g => g.ComicGenres)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (genre == null)
                {
                    return NotFound();
                }

                genre.GenreName = model.GenreName;

                // Remove old relationships
                _context.ComicGenres.RemoveRange(genre.ComicGenres);

                // Add new relationships
                foreach (var comicId in model.SelectedStoryIds)
                {
                    var comicGenre = new ComicGenre
                    {
                        GenreId = genre.Id,
                        ComicId = comicId
                    };
                    _context.ComicGenres.Add(comicGenre);
                }

                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveGenreUpdated", genre.GenreName);
                return RedirectToAction(nameof(Index));
            }

            // Reload comics in case of error
            model.Comics = await _context.Comics.ToListAsync();
            return View(model);
        }


        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _hubContext.Clients.All.SendAsync("ReceiveGenreDeleted", genre.GenreName);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenreExists(Guid id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}

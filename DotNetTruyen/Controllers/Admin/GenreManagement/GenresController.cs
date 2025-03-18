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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Text.Json;

namespace DotNetTruyen.Controllers.Admin.GenreManagement
{
    public class GenresController : Controller
    {
        private readonly DotNetTruyenDbContext _context;
        private readonly IHubContext<GenreHub> _hubContext;
        private readonly ILogger<GenresController> _logger;

        public GenresController(DotNetTruyenDbContext context, IHubContext<GenreHub> hubContext, ILogger<GenresController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }


        


        // GET: Genres


        public async Task<IActionResult> Index(string searchQuery = "", int page =1)
        {
            int pageSize = 1;
            
            var genreQuery = _context.Genres.AsQueryable();
            if(!string.IsNullOrEmpty(searchQuery))
            {
                genreQuery = genreQuery.Where(g => g.GenreName.Contains(searchQuery));
            }
            var totalGenres = await genreQuery.CountAsync();

            var genres = await genreQuery
                .OrderBy(g => g.GenreName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new GenreViewModel
                {
                    Id = g.Id,
                    GenreName = g.GenreName,
                    TotalStories = g.ComicGenres.Count(),
                    UpdatedAt = DateTime.Now,
                })
                .ToListAsync();


            var viewModel = new GenreIndexViewModel
            {
                GenreViewModels = genres,
                SearchQuery = searchQuery,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalGenres / (double)pageSize)
            };
            


            return View("~/Views/Admin/Genres/Index.cshtml", viewModel);
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


        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGenreViewModel createdGenre)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ShowModal"] = "true";
                    _logger.LogWarning("Invalid model state.");
                    return View("Index", createdGenre);
                }

                var genre = new Genre
                {
                    GenreName = createdGenre.GenreName
                };

                _logger.LogWarning("Genre created successfully.");
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
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
       
                _logger.LogError(ex, "Error occurred while creating genre.");

               
                ViewData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
                return View("Index", createdGenre);
            }
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

        [HttpGet]
        public async Task<IActionResult> SearchStories(string query)
        {
            var comicsQuery = _context.Comics.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                comicsQuery = comicsQuery.Where(c => c.Title.ToLower().Contains(query.ToLower()));
            }

            var comics = await comicsQuery
                .Select(c => new { c.Id, c.Title, c.CoverImage })
                .ToListAsync();

            return Json(comics);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditGenreViewModel model)
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
                List<Guid>? selectedStoryIds = string.IsNullOrEmpty(model.SelectedStoryIds)
            ? new List<Guid>()
            : JsonSerializer.Deserialize<List<Guid>>(model.SelectedStoryIds);
                // Remove old relationships
                _context.ComicGenres.RemoveRange(genre.ComicGenres);

                // Add new relationships
                foreach (var comicId in selectedStoryIds)
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

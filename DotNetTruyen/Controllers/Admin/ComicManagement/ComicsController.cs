﻿using System;
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
using DotNetTruyen.ViewModels.Management;

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
            var comics = await _context.Comics
             .Include(c => c.Likes)     
             .Include(c => c.Follows)   
             .ToListAsync();

            return View("~/Views/Admin/Comics/Index.cshtml", comics);
        }

        // GET: Comics/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var comic = _context.Comics
                 .Where(c => c.Id == id)
                 .Select(c => new ComicDetailViewModel
                 {
                     Id = c.Id,
                     Title = c.Title,
                     Description = c.Description,
                     CoverImage = c.CoverImage,
                     Author = c.Author,
                     View = c.View,
                     Status = c.Status,
                     Likes = c.Likes.Count(),
                     Follows = c.Follows.Count(),
                     Genres = c.ComicGenres.Select(g => g.Genre.GenreName).ToList()
                     //RecentChapters = c.Chapters.OrderByDescending(ch => ch.CreatedAt)
                     //                           .Take(5)
                     //                           .Select(ch => new ChapterViewModel
                     //                           {
                     //                               Number = ch.Number,
                     //                               Title = ch.Title,
                     //                               UpdatedAt = ch.UpdatedAt,
                     //                               Views = ch.Views
                     //                           }).ToList(),
                     //Comments = c.Comments.OrderByDescending(cmt => cmt.CreatedAt)
                     //                     .Take(5)
                     //                     .Select(cmt => new CommentViewModel
                     //                     {
                     //                         User = cmt.User.Username,
                     //                         Avatar = cmt.User.AvatarUrl,
                     //                         Content = cmt.Content,
                     //                         Time = cmt.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                     //                     }).ToList()
                 })
                 .FirstOrDefault();

            if (comic == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Comics/Details.cshtml",comic);
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
                    comic.CoverImage = uploadResult;
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

            var comic = await _context.Comics
                .Include(c => c.ComicGenres)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comic == null)
            {
                return NotFound();
            }

            var viewModel = new EditComicViewModel
            {
                Id = comic.Id,
                Title = comic.Title,
                Description = comic.Description,
                Author = comic.Author,
                Status = comic.Status,
                CoverImage = comic.CoverImage,
                Genres = _context.Genres.Select(g => new GenreViewModel
                {
                    Id = g.Id,
                    GenreName = g.GenreName,
                    TotalStories = g.ComicGenres.Count(),
                    UpdatedAt = g.UpdatedAt
                }).ToList(),
                SelectedGenres = comic.ComicGenres.Select(cg => cg.GenreId).ToList()
            };

            return View("~/Views/Admin/Comics/Edit.cshtml", viewModel);
        }

        // POST: Comics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description,Author,Status,SelectedGenres,CoverImageFile,CoverImage")] EditComicViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Error in {state.Key}: {state.Value.Errors[0].ErrorMessage}");
                    }
                }
                // Reload all necessary data for the view
                var comic = await _context.Comics
                    .Include(c => c.ComicGenres)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (comic == null)
                {
                    return NotFound();
                }

                // Keep the user-entered values for these fields
                // but ensure other properties are properly populated
                model.CoverImage = comic.CoverImage; // Keep existing cover image

                // Load all genres and mark selected ones
                model.Genres = await _context.Genres.Select(g => new GenreViewModel
                {
                    Id = g.Id,
                    GenreName = g.GenreName
                }).ToListAsync();

                // If SelectedGenres is null (which could happen during validation failure),
                // reload it from the database
                if (model.SelectedGenres == null)
                {
                    model.SelectedGenres = comic.ComicGenres.Select(cg => cg.GenreId).ToList();
                }

                return View("~/Views/Admin/Comics/Edit.cshtml", model);
            }

            var comicToUpdate = await _context.Comics
                .Include(c => c.ComicGenres)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comicToUpdate == null)
            {
                return NotFound();
            }

            comicToUpdate.Title = model.Title;
            comicToUpdate.Description = model.Description;
            comicToUpdate.Author = model.Author;
            comicToUpdate.Status = model.Status;
            

            // Kiểm tra nếu có ảnh mới được chọn thì mới upload
            if (model.CoverImageFile != null && model.CoverImageFile.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(model.CoverImageFile);
                if (uploadResult != null)
                {
                    comicToUpdate.CoverImage = uploadResult;
                }
            }

            // Cập nhật thể loại
            comicToUpdate.ComicGenres.Clear();
            if (model.SelectedGenres != null && model.SelectedGenres.Any())
            {
                foreach (var genreId in model.SelectedGenres)
                {
                    comicToUpdate.ComicGenres.Add(new ComicGenre { GenreId = genreId, ComicId = id });
                }
            }

            try
            {
                _context.Update(comicToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Comics.Any(e => e.Id == comicToUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["SuccessMessage"] = "Truyện đã được cập nhật thành công!";
            return RedirectToAction(nameof(Index));
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

    
}

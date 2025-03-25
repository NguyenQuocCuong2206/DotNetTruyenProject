using DotNetTruyen.Data;
using DotNetTruyen.Models;
using DotNetTruyen.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetTruyen.Controllers
{
	public class ReadChapterController : Controller
	{
		private readonly DotNetTruyenDbContext _context;

		public ReadChapterController(DotNetTruyenDbContext context)
		{
			_context = context;
		}

		// Get chapter by ID
		public async Task<IActionResult> Index(Guid id)
		{
			// Get the current chapter with its images
			var chapter = await _context.Chapters
				.Include(c => c.Comic)
				.Include(c => c.Images.OrderBy(i => i.ImageNumber))
				.FirstOrDefaultAsync(c => c.Id == id && c.IsPublished && c.DeletedAt == null);

			if (chapter == null)
			{
				return NotFound();
			}

			// Increment view count
			chapter.Views += 1;
			await _context.SaveChangesAsync();

			// Get previous chapter
			var prevChapter = await _context.Chapters
				.Where(c => c.ComicId == chapter.ComicId &&
					   c.ChapterNumber < chapter.ChapterNumber &&
					   c.IsPublished &&
					   c.DeletedAt == null)
				.OrderByDescending(c => c.ChapterNumber)
				.FirstOrDefaultAsync();

			// Get next chapter
			var nextChapter = await _context.Chapters
				.Where(c => c.ComicId == chapter.ComicId &&
					   c.ChapterNumber > chapter.ChapterNumber &&
					   c.IsPublished &&
					   c.DeletedAt == null)
				.OrderBy(c => c.ChapterNumber)
				.FirstOrDefaultAsync();

			// Get all chapters of this comic for the chapter list
			var allChapters = await _context.Chapters
				.Where(c => c.ComicId == chapter.ComicId &&
					   c.IsPublished &&
					   c.DeletedAt == null)
				.OrderByDescending(c => c.ChapterNumber)
				.ToListAsync();

			// Create view model
			var viewModel = new ReadChapterViewModel
			{
				Chapter = chapter,
				PreviousChapter = prevChapter,
				NextChapter = nextChapter,
				AllChapters = allChapters
			};

			return View(viewModel);
		}

		// Read first chapter of a comic
		public async Task<IActionResult> ReadFromBeginning(Guid comicId)
		{
			var firstChapter = await _context.Chapters
				.Where(c => c.ComicId == comicId &&
					   c.IsPublished &&
					   c.DeletedAt == null)
				.OrderBy(c => c.ChapterNumber)
				.FirstOrDefaultAsync();

			if (firstChapter == null)
			{
				TempData["ErrorMessage"] = "Không có chương nào được tìm thấy.";
				return RedirectToAction("Index", "Detail", new { id = comicId });
			}

			return RedirectToAction("Index", new { id = firstChapter.Id });
		}
		public async Task<IActionResult> ReadLastChapter(Guid comicId)
		{
			var lastChapter = await _context.Chapters
				.Where(c => c.ComicId == comicId &&
					   c.IsPublished &&
					   c.DeletedAt == null)
				.OrderByDescending(c => c.ChapterNumber)
				.FirstOrDefaultAsync();

			if (lastChapter == null)
			{
				TempData["ErrorMessage"] = "Không có chương nào được tìm thấy.";
				return RedirectToAction("Index", "Detail", new { id = comicId });
			}

			return RedirectToAction("Index", new { id = lastChapter.Id });
		}
		public async Task<List<ChapterImage>> GetPublishedChapterImagesAsync()
		{
			return await _context.ChapterImages
				.Where(img => img.Chapter.IsPublished)
				.ToListAsync();
		}
	}
}
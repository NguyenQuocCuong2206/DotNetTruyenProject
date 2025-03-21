using System.Diagnostics.CodeAnalysis;

namespace DotNetTruyen.ViewModels.Management
{
    public class EditChapterViewModel
    {
        public Guid Id { get; set; } 
        public Guid ComicId { get; set; } 
        public string? ChapterTitle { get; set; } 
        public int ChapterNumber { get; set; } 
        public DateTime? PublishedDate { get; set; } 
        public List<string>? ExistingImages { get; set; }
       
        public List<IFormFile>? Images { get; set; }
        public string? DeletedImages { get; set; }
    }
}

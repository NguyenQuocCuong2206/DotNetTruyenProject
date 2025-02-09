namespace DotNetTruyen.Models
{
    public class Chapter
    {
        public string ChapterTitle { get; set; }
        public int ChapterNumber { get; set; }
        public Guid ComicId { get; set; }
        public Comic Comic { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ICollection<ChapterImage> Images { get; set; }
    }
}

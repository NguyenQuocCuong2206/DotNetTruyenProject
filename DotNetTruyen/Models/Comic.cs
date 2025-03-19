namespace DotNetTruyen.Models
{
    public class Comic : BaseEnity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? CoverImage { get; set; }
        public string Author  { get; set; }
        public int View {  get; set; }
        public bool Status { get; set; }
        public int Likes { get; set; }
        public ICollection<Chapter> Chapters { get; set; }
        public ICollection<ComicGenre> ComicGenres { get; set; } = new List<ComicGenre>();
        public ICollection<Follow> Follows { get; set; } = new List<Follow>();
    }
}

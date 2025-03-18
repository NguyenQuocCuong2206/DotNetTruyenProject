using DotNetTruyen.Models;

namespace DotNetTruyen.ViewModels
{
    public class SearchViewModel
    {
        public List<Comic> Comics { get; set; }
        public int TotalResults { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }
        public string Status { get; set; }
        public List<Genre> AllGenres { get; set; }
        public int Page { get; set; }
        public string GenreFilter { get; set; }
    }
}

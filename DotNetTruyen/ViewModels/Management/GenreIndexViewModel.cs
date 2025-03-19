using DotNetTruyen.Models;

namespace DotNetTruyen.ViewModels.Management
{
    public class GenreIndexViewModel
    {
        public List<GenreViewModel> GenreViewModels { get; set; }
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

using DotNetTruyen.Models;

namespace DotNetTruyen.ViewModels.Management
{
    public class ComicIndexViewModel
    {
        public List<Comic> Comics { get; set; }
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

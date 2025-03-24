namespace DotNetTruyen.ViewModels.Management
{
    public class DashboardViewModel
    {
        public int TotalComics { get; set; }
        public int TotalChapters { get; set; }
        public int TotalViews { get; set; }
        public int TotalUsers { get; set; }
        public List<RecentChapterViewModel> RecentChapters { get; set; }
        public List<TopGenreViewModel> TopGenres { get; set; }
        public List<string> ViewsByMonthLabels { get; set; }
        public List<int> ViewsByMonthData { get; set; }
        public List<string> PreviousViewsByMonthLabels { get; set; }
        public List<int> PreviousViewsByMonthData { get; set; }
    }
}

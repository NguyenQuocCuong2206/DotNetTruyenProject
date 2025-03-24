namespace DotNetTruyen.ViewModels.Management
{
    public class RankTypeIndexViewModel
    {
        public List<RankTypeViewModel> RankTypes { get; set; }
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRankTypes { get; set; }  
        public int TotalRanks { get; set; }
    }
}

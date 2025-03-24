namespace DotNetTruyen.ViewModels.Management
{
    public class RankIndexViewModel
    {
        public List<RankViewModel> Ranks { get; set; }
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRanks { get; set; }       
        public int TotalUsers { get; set; }       
        public double RankChangePercentage { get; set; }  
        public double UserChangePercentage { get; set; }
    }
}

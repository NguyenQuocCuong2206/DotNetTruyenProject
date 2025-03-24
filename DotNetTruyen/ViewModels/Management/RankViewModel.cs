namespace DotNetTruyen.ViewModels.Management
{
    public class RankViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ExpRequired { get; set; }
        public int Level { get; set; }
        public string RankTypeName { get; set; }
        public int UserCount { get; set; }
    }
}

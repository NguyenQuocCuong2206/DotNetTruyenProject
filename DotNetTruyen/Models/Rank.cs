namespace DotNetTruyen.Models
{
    public class Rank : BaseEnity<Guid>
    {
        public string Name { get; set; }
        public int ExpRequired { get; set; }
        public int Level { get; set; }
        public ICollection<UserRank> UserRanks { get; set; }
        public Guid RankTypeId { get; set; }
        public RankType RankType { get; set; }

    }
}

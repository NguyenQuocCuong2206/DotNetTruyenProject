namespace DotNetTruyen.Models
{
    public class UserRank
    {
        public Guid UserId { get; set; }
        public Guid RankId { get; set; }
        public int Exp {  get; set; }
        public User User { get; set; }
        public Rank Rank { get; set; }
    }
}

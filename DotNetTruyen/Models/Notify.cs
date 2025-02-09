using System.ComponentModel.DataAnnotations;

namespace DotNetTruyen.Models
{
    public class Notify
    {
        [Key]
        public Guid NotifyId { get; set; }
        public Guid ComicId { get; set; }
        public Guid UserId {  get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public User User { get; set; }
        public Comic Comic { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace DotNetTruyen.Models
{
    public class Like
    {
        [Key]
        public Guid Id { get; set; }
        public string UserIpHash { get; set; }
        public Guid ComicId { get; set; }
        public Comic Comic { get; set; }
    }
}

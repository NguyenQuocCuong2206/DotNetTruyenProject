using System.ComponentModel.DataAnnotations;

namespace DotNetTruyen.Models
{
    public class Like
    {
        public string UserIpHash { get; set; }
        public Guid ComicId { get; set; }
        public Comic Comic { get; set; }
    }
}

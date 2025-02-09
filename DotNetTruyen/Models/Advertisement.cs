using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DotNetTruyen.Models
{
    public class Advertisement : BaseEnity<Guid>
    {
        public string Title { get; set; }
        public string LinkTo { get; set; }
        public string ImageUrl { get; set; }
    }
}

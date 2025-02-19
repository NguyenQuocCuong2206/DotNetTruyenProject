using Microsoft.AspNetCore.Identity;

namespace DotNetTruyen.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? NameToDisplay { get; set; }
    }
}

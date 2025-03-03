using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetTruyen.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [HttpGet("/userProfile")]
        public IActionResult UserProfile()
        {
            return View();
        }
    }
}

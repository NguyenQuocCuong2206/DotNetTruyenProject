using DotNetTruyen.Models;
using DotNetTruyen.Service;
using DotNetTruyen.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Security.Claims;

namespace DotNetTruyen.Controllers
{
    [Authorize]
    [Route("/Auths/[action]")]
    public class AuthsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;

        public AuthsController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            EmailService emailService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet("/login/")]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("/login/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {

                var result = await _signInManager.PasswordSignInAsync(model.UserNameOrEmail, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.UserNameOrEmail);
                    if (user != null)
                    {
                        result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
                    }
                }

                if (result.Succeeded)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return LocalRedirect("/DashBoard");
                    }
                    return LocalRedirect("/Home");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không chính xác";
                    return View();
                }
            }
            ViewData["ErrorMessage"] = "Đã xảy ra lỗi khi đăng nhập";
            return View();
        }

        [HttpGet("/loginWithGoogle/")]
        [AllowAnonymous]
        public async Task LoginWithGoogle(string returnUrl = null)
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse", new {returnUrl}),
                    Items = { { "prompt", "select_account" } }
                });
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                ViewData["ErrorMessage"] = "Đăng nhập bằng Google không thành công";
                return LocalRedirect("/login");
            }

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string userName = email.Split("@")[0];
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var newUser = new Models.User { UserName = userName, Email = email };
                newUser.NameToDisplay = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var creatResult = await _userManager.CreateAsync(newUser);
                if (!creatResult.Succeeded)
                {
                    ViewData["ErrorMessage"] = "Đăng nhập bằng Google không thành công";
                    return LocalRedirect("/login");
                }

                //var addRoleResult = await _userManager.AddToRoleAsync(newUser, "Reader");
                //if (!addRoleResult.Succeeded)
                //{
                //    ViewData["ErrorMessage"] = "Có lỗi khi cấp quyền cho tài khoản này";
                //    return LocalRedirect("/login");
                //}

                if (creatResult.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    await _userManager.ConfirmEmailAsync(newUser, code);
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }
            else
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("/login");
        }

        [HttpGet("/register/")]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet("/forgotPassword/")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("/forgotPassword/")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string emailReset)
        {
            await _emailService.SendEmailAsync(emailReset, "Xin chao", "DotNetTruyen đây");
            return View();
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}

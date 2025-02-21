using DotNetTruyen.Models;
using DotNetTruyen.Service;
using DotNetTruyen.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using static System.Net.WebRequestMethods;

namespace DotNetTruyen.Controllers
{
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
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("/login/")]
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
                    ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác";
                    return View();
                }
            }
            ViewBag.ErrorMessage = "Đã xảy ra lỗi khi đăng nhập";
            return View();
        }

        [HttpGet("/loginWithGoogle/")]
        public async Task LoginWithGoogle(string returnUrl = null)
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse", new {returnUrl}),
                    Items = { { "prompt", "select_account" } }
                });
        }

        public async Task<IActionResult> GoogleResponse(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Đăng nhập bằng Google không thành công";
                return View("Login");
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
                    ViewBag.ErrorMessage = "Đăng nhập bằng Google không thành công";
                    return View("Login");
                }

                var addRoleResult = await _userManager.AddToRoleAsync(newUser, "Reader");
                if (!addRoleResult.Succeeded)
                {
                    ViewBag.ErrorMessage = "Có lỗi khi cấp quyền cho tài khoản này";
                    return View("Login");
                }

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
            return LocalRedirect("/login");
        }

        [HttpGet("/register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(user, "Reader");
                    if (!addRoleResult.Succeeded)
                    {
                        ViewBag.ErrorRegisterMessage = "Có lỗi khi cấp quyền cho tài khoản này";
                        return View();
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.ActionLink(
                        action: nameof(ConfirmEmail),
                        values:
                            new
                            {
                                userId = user.Id,
                                code = code
                            },
                        protocol: Request.Scheme);

                    await _emailService.SendEmailAsync(model.Email,
                        "Xác nhận địa chỉ email",
                        @$"Bạn đã đăng ký tài khoản trên DotNetTruyen, 
                           hãy <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a> 
                           để kích hoạt tài khoản.");
                    ViewBag.SuccessRegisterMessage = "Hãy truy cập vào email để xác thực việc đăng ký tài khoản";
                }
                else
                {
                    ViewBag.ErrorRegisterMessage = result.Errors.FirstOrDefault().Description;
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            
            if (userId == null || code == null)
            {
                ViewBag.ErrorRegisterMessage = "Đăng ký không thành công do lỗi xác thực email";
                return View("Register");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorRegisterMessage = "Đăng ký không thành công do lỗi xác thực email";
                return View("Register");
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                ViewBag.SuccessRegisterMessage = "Đăng ký thành công, hãy đăng nhập ngay";
                return View("Login");
            }

            ViewBag.ErrorRegisterMessage = "Đăng ký không thành công do lỗi xác thực email";
            return View("Register");
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(string userId, string code)
        {

            ViewBag.ErrorRegisterMessage = "Đăng ký không thành công do lỗi xác thực email";
            return View("Register");
        }

        [HttpGet("/forgotPassword/")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("/forgotPassword/")]
        public async Task<IActionResult> ForgotPassword(string emailReset)
        {
            await _emailService.SendEmailAsync(emailReset, "Xin chao", "DotNetTruyen đây");
            return View();
        }

        [HttpGet("/logout")]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}

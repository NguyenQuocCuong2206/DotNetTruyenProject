using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetTruyen.Data;
using DotNetTruyen.Models;
using Microsoft.AspNetCore.Authorization;
using DotNetTruyen.ViewModels.Management;
using Microsoft.AspNetCore.Identity;
using DotNetTruyen.Services;

namespace DotNetTruyen.Controllers.Admin.UserManagement
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DotNetTruyenDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;

        public UsersController(
            DotNetTruyenDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            EmailService emailService,
            OtpService otpService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _otpService = otpService;
        }

        [HttpGet("/userManagement")]
        // GET: Users
        public async Task<IActionResult> Index()
        {
            var adminUserIds = _context.UserRoles
                .Where(ur => _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Admin"))
                .Select(ur => ur.UserId)
                .ToHashSet();

            var userRoles = _context.UserRoles
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .GroupBy(ur => ur.UserId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Name).ToList());

            var viewModel = _context.Users
                .Where(u => !adminUserIds.Contains(u.Id))
                .Select(u => new UserViewModel
            {
                Id = u.Id.ToString(),
                ImageUrl = u.ImageUrl,
                NameToDisplay = u.NameToDisplay,
                Email = u.Email,
                Role = userRoles.ContainsKey(u.Id) ? string.Join(", ", userRoles[u.Id]) : "Không có",
                Status = u.LockoutEnd,

            });
            return View("~/Views/Admin/Users/Index.cshtml", await viewModel.ToListAsync());
        }

        [HttpGet("/blockUser")]
        public async Task<IActionResult> BlockUser(string userId)
        {
            TempData["Message"] = "Khóa tài khoản không thành công";
            TempData["TypeMessage"] = "alert alert-danger";

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // Khóa tài khoản vô thời hạn
                user.LockoutEnd = DateTimeOffset.MaxValue;
                await _userManager.UpdateAsync(user);
                TempData["Message"] = "Đã khóa tài khoản";
                TempData["TypeMessage"] = "alert alert-success";
            }  
            return RedirectToAction("Index");
        }

        [HttpGet("/unblockUser")]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            TempData["TypeMessage"] = "alert alert-danger";
            TempData["Message"] = "Mở khóa tài khoản không thành công";

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Mở khóa user
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
                TempData["TypeMessage"] = "alert alert-success";
                TempData["Message"] = "Mở khóa tài khoản thành công";
            }   
            return RedirectToAction("Index");
        }

    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SJBRAssociatesWeb.Models;
using SJBRAssociatesWeb.Services;
using System.Security.Claims;

namespace SJBRAssociatesWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminAuthSettings _settings;
        private readonly IJsonDataService _dataService;
        private readonly PasswordHasher<object> _hasher = new();

        public AdminController(IOptions<AdminAuthSettings> settings, IJsonDataService dataService)
        {
            _settings = settings.Value;
            _dataService = dataService;
        }

        [HttpGet]
        [Route("Admin")]
        [AllowAnonymous]
        public IActionResult Login() => View(new AdminLoginViewModel());

        [HttpPost]
        [Route("Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var admin = await _dataService.GetAdminUserAsync(model.Username);
            //string password=_hasher.HashPassword(model,model.Password); default pw 1234
            var validUsername = admin != null;
            var validPassword = validUsername &&
                _hasher.VerifyHashedPassword(null!, admin!.PasswordHash, model.Password) != PasswordVerificationResult.Failed;

            if (!validPassword)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(model);
            }

            var claims = new List<Claim> { new(ClaimTypes.Name, model.Username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction(nameof(Enquiries));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        public async Task<IActionResult> Enquiries()
        {
            var enquiries = await _dataService.GetEnquiriesAsync();
            return View(enquiries.OrderByDescending(e => e.SubmittedAtUtc).ToList());
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var username = User.Identity?.Name ?? string.Empty;
            var admin = await _dataService.GetAdminUserAsync(username);

            if (admin == null ||
                _hasher.VerifyHashedPassword(null!, admin.PasswordHash, model.CurrentPassword) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Current password is incorrect");
                return View(model);
            }

            admin.PasswordHash = _hasher.HashPassword(null!, model.NewPassword);
            admin.UpdatedAtUtc = DateTime.UtcNow;
            await _dataService.UpdateAdminUserAsync(admin);

            // Force re-login with the new password.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["PasswordChanged"] = true;
            return RedirectToAction(nameof(Login));
        }
    }
}

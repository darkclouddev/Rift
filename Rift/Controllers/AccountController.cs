using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using Rift.Middleware.Discord;

namespace Rift.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Challenge(DiscordSettings.AuthenticationScheme);

            return RedirectToPage("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            return new SignOutResult(new[]
            {
                DiscordSettings.AuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme
            });
        }
    }
}

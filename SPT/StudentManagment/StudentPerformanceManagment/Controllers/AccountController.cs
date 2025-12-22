using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPerformanceManagment.Models;

namespace IdentityDemo.Controllers
{
        public class AccountController : Controller
        {
            private readonly SignInManager<AppUser> _signInManager;
            private readonly UserManager<AppUser> _userManager;

            public AccountController(SignInManager<AppUser> signInManager,
                                     UserManager<AppUser> userManager)
            {
                _signInManager = signInManager;
                _userManager = userManager;
            }

            // LOGIN PAGE
            [HttpGet]
            public IActionResult Login()
            {
                return View();
            }

            // LOGIN POST
            [HttpPost]
            public async Task<IActionResult> Login(string email, string password)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    email, password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard");
                }

                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // ROLE BASED DASHBOARD
            public async Task<IActionResult> Dashboard()
            {
                var user = await _userManager.GetUserAsync(User);

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return View("AdminDashboard");

                if (await _userManager.IsInRoleAsync(user, "Staff"))
                    return View("StaffDashboard");

                return View("StudentDashboard");
            }

            // LOGOUT
            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login");
            }
        }
    }




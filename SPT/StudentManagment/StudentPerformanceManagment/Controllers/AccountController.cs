using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPerformanceManagment.Models;
using StudentPerformanceManagment.Models.ViewModel;


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

        [HttpGet]
        public IActionResult Login()
        {
            // Redirect authenticated users away from the login page
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
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
<<<<<<< HEAD
                return RedirectToAction("Dashboard", "Admin"); 
=======

                return RedirectToAction("Dashboard", "Admin"); ;
>>>>>>> d76ab170f51a2d1ffd356b57cdc2dfaab74a1799

            if (await _userManager.IsInRoleAsync(user, "Staff"))
                return RedirectToAction("Dashboard", "Staff");

               return RedirectToAction("Dashboard", "Student"); 



        }

        // LOGOUT
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}




    


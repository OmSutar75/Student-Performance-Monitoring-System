using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPerformanceManagment;
using StudentPerformanceManagment.Models;



namespace StudentPerformanceManagement.Controllers
{
    public class StudentController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StudentController(UserManager<AppUser> userManager,
        ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            string role = "Student";
            if (await _userManager.IsInRoleAsync(user, "Admin"))
                role = "Admin";
            else if (await _userManager.IsInRoleAsync(user, "Staff"))
                role = "Staff";

            var vm = new LayoutUserViewModel
            {
                FullName = user?.FullName ?? "User",
                Role = role
            };

            return View($"~/Views/{role}/Dashboard.cshtml", vm);

        }
    }
}

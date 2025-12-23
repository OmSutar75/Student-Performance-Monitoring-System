using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPerformanceManagment.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentPerformanceManagment.Controllers
{


    public class StaffController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;


        public StaffController(UserManager<AppUser>  userManager,
                                  ApplicationDbContext context)

        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StaffDashboard()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myTasks =await _context.Tasks
           .Include(t => t.Course)
           .Include(t => t.Subject)
           .Include(t => t.CourseGroup)
           .Where(t => t.Staff.AppUserId == userId)
           .ToListAsync();

            return View(myTasks);
        }
    }
}

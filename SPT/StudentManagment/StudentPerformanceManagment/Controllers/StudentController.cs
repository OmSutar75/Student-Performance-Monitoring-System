//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using StudentPerformanceManagment;
//using StudentPerformanceManagment.Models;
//using StudentPerformanceManagment.Models.ViewModel;
//using System.Security.Claims;



//namespace StudentPerformanceManagement.Controllers
//{
//    public class StudentController : Controller
//    {

//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<AppUser> _userManager;

//        public StudentController(UserManager<AppUser> userManager,
//        ApplicationDbContext context)
//        {
//            _userManager = userManager;
//            _context = context;
//        }

//        public async Task<IActionResult> MyTasks()
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            // 1. Fetch the user for the Layout data
//            var user = await _userManager.FindByIdAsync(userId);

//            // 2. Fetch the tasks
//            var myTasks = await _context.Tasks
//                .Include(t => t.Course)
//                .Include(t => t.Subject)
//                .Include(t => t.CourseGroup)
//                .Where(t => t.Staff.AppUserId == userId)
//                .ToListAsync();

//            // 3. Map everything to the ViewModel
//            var viewModel = new StaffDashViewModel
//            {
//                // Layout Properties (Inherited from LayoutUserViewModel)
               
//                // ProfilePictureUrl = user?.ProfilePictureUrl,

//                // Page Properties
//                StaffId = userId,
//                StaffName = user?.UserName, // Or user.FullName
//                TaskCount = myTasks.Count,
//                Tasks = myTasks // This is your List<Tasks>
//            };

//            return View(viewModel);
//        }

//    }
//}

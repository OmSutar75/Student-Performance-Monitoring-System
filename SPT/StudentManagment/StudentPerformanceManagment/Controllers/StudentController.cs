using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment;
using StudentPerformanceManagment.Models;
using StudentPerformanceManagment.Models.ViewModel;
using System.Security.Claims;



namespace StudentPerformanceManagement.Controllers
{
    [Authorize(Roles = "Student")]
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

        private async Task<StudentViewModel> GetData()
        {
            var userId = _userManager.GetUserId(User);

            // 1. Single Query with Joins: Student, Course, aur Group ko ek saath fetch karein
            var student = await _context.Students
                .Include(s => s.Course)
                .Include(s => s.CourseGroup)
                .Where(s => s.AppUserId == userId)
                .FirstOrDefaultAsync();

            if (student == null) return new StudentViewModel();

            // 2. Optimized Count: Subject count ke liye alag query
            int subjectCount = 0;
            if (student.CourseId != null)
            {
                subjectCount = await _context.Subjects
                    .CountAsync(s => s.CourseId == student.CourseId);
            }

            // 3. Mapping to ViewModel
            var stud = new StudentViewModel()
            {
                StudentId = student.StudentId,
                PRN = student.PRN,
                Name = student.Name,
                Email = User.Identity?.Name, // Identity se email lena fast hai
                CourseName = student.Course?.CourseName ?? "N/A",
                SubjectCount = subjectCount,
                CourseGroupName = student.CourseGroup?.GroupName ?? "N/A",
                MobileNo = student.MobileNo
            };

            return stud;
        }


        public async Task<IActionResult> Dashboard()
        {

            var stud = await GetData();
            return View(stud);

        }

   [HttpGet]
        public async Task<IActionResult> EditProfile() 
        {
            var stud = await GetData(); 
            return View(stud);
        }

        [HttpPost]
        public async Task<IActionResult> AfterEditProfile(StudentViewModel model)
        {
        

            var userId = _userManager.GetUserId(User);
            var appUser = await _userManager.FindByIdAsync(userId);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.AppUserId == userId);

            if (student == null) return NotFound();

            // 2. Profile Data Update (Name & Mobile)
            student.Name = model.Name;
            student.MobileNo = model.MobileNo;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();



            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Dashboard");
        }



        public IActionResult StudentPerformance()
        {
            return View();
        }
    }
}

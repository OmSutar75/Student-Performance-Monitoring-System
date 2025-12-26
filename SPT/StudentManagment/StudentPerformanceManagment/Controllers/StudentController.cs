using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment;
using StudentPerformanceManagment.Models;
using StudentPerformanceManagment.Models.ViewModel;
using System.Collections.Immutable;
using System.Security.Claims;



namespace StudentPerformanceManagement.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {

        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StudentController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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


     
        public IActionResult ChangePassword()
        {
            
            return View(new PasswordViewModel());
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(PasswordViewModel model)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

           
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password updated successfully!";
                return RedirectToAction("Dashboard", "Student");
            }
        
            foreach (var error in result.Errors)
            {
              
                if (error.Code.Contains("PasswordMismatch"))
                {
                    ModelState.AddModelError("CurrentPassword", "The current password you entered is incorrect.");
                }
                else
                {
                 
                    ModelState.AddModelError("NewPassword", error.Description);
                }
            }

            return View("ChangePassword", model);
        }

        public IActionResult StudentPerformance()
        {
            return View();
        }

        public IActionResult Ranking()
        {
            List<Mark> Marks = _context.Marks.ToList();

            //Marks = Marks.OrderBy(m => m.TotalObtained());
            //return View(SortedMarks);
            return View();
        }
    }
}

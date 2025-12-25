using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var student = await _context.Students.Where(s => s.AppUserId == userId).FirstOrDefaultAsync();
              
            string courseName = student.CourseId != null ? 
                (await _context.Courses.Where(c => c.CourseId == student.CourseId).FirstOrDefaultAsync())?.CourseName 
                : "N/A";

            int subjectcount=student.CourseId != null ? 
                await _context.Subjects.Where(s => s.CourseId == student.CourseId).CountAsync() 
                : 0;

            string courseGroupName = student.CourseGroupId != null ? 
                (await _context.CourseGroups.Where(cg => cg.CourseGroupId == student.CourseGroupId).FirstOrDefaultAsync())?.GroupName 
                : "N/A";

            var stud = new StudentViewModel()
            {
                PRN = student.PRN,
                Name=student.Name,
                Email=user.Email,
                CourseName=courseName,
                SubjectCount=subjectcount,
                CourseGroupName=courseGroupName,
                MobileNo =student.MobileNo,

                
            };



            return View(stud);

        }
        public IActionResult EditProfile() {
            return View();
        }
        public IActionResult StudentPerformance()
        {
            return View();
        }
    }
}

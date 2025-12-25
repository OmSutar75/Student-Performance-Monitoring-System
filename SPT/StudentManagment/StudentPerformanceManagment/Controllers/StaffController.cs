using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment.Models;
using StudentPerformanceManagment.Models.ViewModel;
using System.Security.Claims;

namespace StudentPerformanceManagment.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StaffController(UserManager<AppUser> userManager,
        ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var myTasks = await _context.Tasks
                .Include(t => t.Course)
                .Include(t => t.Subject)
                .Include(t => t.CourseGroup)
                .Where(t => t.Staff.AppUserId == userId)
                .ToListAsync();

            var vm = new StaffDashViewModel
            {
                // base (LayoutUserViewModel) properties
                FullName = user?.FullName ?? "User",
                Role = "Staff",

                // StaffDashViewModel properties
                StaffId = userId,
                StaffName = user?.UserName,
                TaskCount = myTasks.Count,
                Tasks = myTasks
            };

            return View("Dashboard", vm);   // YAHAN StaffDashViewModel hi return karo
        }




        public IActionResult AddMark(int subjectId)
        {
            var viewModel = new MarkViewModel();

            // 1. Fetch students (you might filter by class or department here)
            viewModel.students = _context.Students.ToList();

            // 2. Pass the SubjectId to the view using ViewBag
            ViewBag.SubjectId = 1;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveMark(int StudentId, int SubjectId, int TheoryMarks, int LabMarks, int InternalMarks)
        {
            // 1. Check if marks already exist for this student in this subject
            var existingMark = _context.Marks
                .FirstOrDefault(m => m.StudentId == StudentId && m.SubjectId == SubjectId);

            if (existingMark != null)
            {
                // Update
                existingMark.TheoryMarks = TheoryMarks;
                existingMark.LabMarks = LabMarks;
                existingMark.InternalMarks = InternalMarks;
                _context.Marks.Update(existingMark);
            }
            else
            {
                // Insert
                var newMark = new Mark
                {
                    StudentId = StudentId,
                    SubjectId = SubjectId,
                    TheoryMarks = TheoryMarks,
                    LabMarks = LabMarks,
                    InternalMarks = InternalMarks
                };
                _context.Marks.Add(newMark);
            }

            _context.SaveChanges();



              
            return RedirectToAction("AddMark",new {subjectId=SubjectId});
        }

        public async Task<IActionResult> MyTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var myTasks = await _context.Tasks
                .Include(t => t.Course)
                .Include(t => t.Subject)
                .Include(t => t.CourseGroup)
                .Where(t => t.Staff.AppUserId == userId)
                .ToListAsync();

            var vm = new StaffDashViewModel
            {
                // base properties (LayoutUserViewModel)
                FullName = user?.FullName ?? "User",
                Role = "Staff",

                // StaffDashViewModel properties
                StaffId = userId,
                StaffName = user?.UserName,
                TaskCount = myTasks.Count,
                Tasks = myTasks
            };

            return View("MyTasks", vm);  // ya sirf return View(vm);
        }

    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using StudentPerformanceManagement.Models;
=======

>>>>>>> main
using StudentPerformanceManagment.Models;
using StudentPerformanceManagment.Models.ViewModel;
using System.Security.Claims;

namespace StudentPerformanceManagment.Controllers
{
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

        // ROLE BASED DASHBOARD - create and pass vm so partials that expect LayoutUserViewModel work
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

<<<<<<< HEAD

              
            return RedirectToAction("AddMark",new {subjectId=SubjectId});
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

=======
            return RedirectToAction("AddMark", new { subjectId = SubjectId });
>>>>>>> main
        }
    }
}

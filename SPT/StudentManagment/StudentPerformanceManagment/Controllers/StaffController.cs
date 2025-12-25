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

                // StaffDashViewModel properties
                StaffId = userId,
                StaffName = user?.UserName,
                TaskCount = myTasks.Count,
                Tasks = myTasks
            };
            return View("Dashboard", vm);   // YAHAN StaffDashViewModel hi return karo
        }




        public IActionResult AddMark(int id)

        {

            // id = 3;
            var task = _context.Tasks.Include(c => c.Course)
                .Include(cg => cg.CourseGroup)
                .Include(s=>s.Subject)
                .Where(t => t.TasksId == id).FirstOrDefault();


            var students = _context.Students.Where(s => s.CourseGroupId == task.CourseGroupId)
                .Select(s => new MarkViewModel
                {

                    StudentId = s.StudentId,
                    SubjectId = task.SubjectId,
                    CourseGroupId = task.CourseGroupId,
                   // CourseId = task.CourseId,
                    PRN = s.PRN,
                    Name = s.Name,
                    TaskId = task.TasksId,
                    TheoryMarks = _context.Marks.Where(m => m.TasksId == task.TasksId && m.StudentId == s.StudentId)
                                    .Select(m => m.TheoryMarks).FirstOrDefault(),


                    LabMarks = _context.Marks.Where(m => m.TasksId == task.TasksId && m.StudentId == s.StudentId)
                                    .Select(m => m.LabMarks).FirstOrDefault(),

                    InternalMarks = _context.Marks.Where(m => m.TasksId == task.TasksId && m.StudentId == s.StudentId)
                                    .Select(m => m.InternalMarks).FirstOrDefault(),
                }).ToList();







            return View(students);
        }


        [HttpPost]
        public IActionResult SaveMark(UpdateStudentViewModel markviewmodel)
        {
            var existingMark = _context.Marks
                .FirstOrDefault(m => m.StudentId == markviewmodel.StudentId && m.TasksId == markviewmodel.TaskId);

            if (existingMark != null)
            {
                // Update
                existingMark.TheoryMarks = markviewmodel.TheoryMarks;
                existingMark.LabMarks = markviewmodel.LabMarks;
                existingMark.InternalMarks = markviewmodel.InternalMarks;
               
            }
            else
            {
                // Insert
                var newMark = new Mark
                {
                    TasksId = markviewmodel.TaskId,
                    StudentId = markviewmodel.StudentId,
                    SubjectId = markviewmodel.SubjectId,
                    TheoryMarks = markviewmodel.TheoryMarks,
                    LabMarks = markviewmodel.LabMarks,
                    InternalMarks = markviewmodel.InternalMarks
                };
                _context.Marks.Add(newMark);
               
            }



            _context.SaveChanges();

            return RedirectToAction("AddMark",new {id= markviewmodel.TaskId});

        }

        public IActionResult CompleteTask(int taskId)
        {
            var task = _context.Tasks.Find(taskId);
            if (task != null)
            {
                task.Status = Status.Completed;
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("AddMark",new { taskId });
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

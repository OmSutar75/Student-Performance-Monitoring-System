
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using StudentPerformanceManagement.Models;
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

        public IActionResult Dashboard()
        {
            StaffDashViewModel staffDashViewModel = new StaffDashViewModel();
            Staff staff = (from s in _context.Staffs
                           where s.Email == _userManager.GetUserName(User)
                           select s).FirstOrDefault();

            staffDashViewModel.StaffId = staff.StaffId;
            staffDashViewModel.StaffName = staff.Name;

            List < Tasks > tasks = _context.Tasks.ToList();
            staffDashViewModel.Tasks = (from tsk in tasks
                                     where tsk.StaffId == staffDashViewModel.StaffId
                                     select tsk ).ToList();

            staffDashViewModel.TotalTask = tasks.Count();

            //COUNT PENDING TASKS
            foreach (var task in staffDashViewModel.Tasks)
            {
                if (task.Status == Status.Pending)
                    staffDashViewModel.PendingTask++;
            }

            //COUNT COMPLETED TASKS
            foreach (var task in staffDashViewModel.Tasks)
            {
                if (task.Status == Status.Completed)
                    staffDashViewModel.CompletedTasks++;
            }

            return View(staffDashViewModel);
        }

        public IActionResult AddMark(int subjectId)
        {
            var viewModel = new MarkViewModel();

            // 1. Fetch students (you might filter by class or department here)
            viewModel.Students = _context.Students.ToList();

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

            return RedirectToAction("AddMark", new {subjectId = SubjectId});
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

        public IActionResult PerformTask()
        {

            int taskid = 2;
            var task = _context.Tasks.Include(c => c.Course).Include(cg => cg.CourseGroup).Where(t => t.TasksId == taskid).FirstOrDefault();


            var students = _context.Students.Where(s => s.CourseGroupId == task.CourseGroupId)
                .Select(s => new MarkViewModel
                {
                    SubjectId = task.SubjectId,
                    CourseGroupId = task.CourseGroupId,
                    CourseId = task.CourseId,
                    PRN = s.PRN,
                    Name = s.Name,                   TaskId = task.TasksId,
                    TheoryMarks = _context.Marks.Where(m => m.TasksId == task.TasksId && m.StudentId == s.StudentId)
                                    .Select(m => m.TheoryMarks).FirstOrDefault(),


                    LabMarks = _context.Marks.Where(m => m.TasksId == task.TasksId && m.StudentId == s.StudentId)
                                    .Select(m => m.LabMarks).FirstOrDefault(),

                    InternalMarks = _context.Marks.Where(m => m.TasksId == task.TasksId && m.StudentId == s.StudentId)
                                    .Select(m => m.InternalMarks).FirstOrDefault(),
                }).ToList();







            return View(students);
        }

    }
}

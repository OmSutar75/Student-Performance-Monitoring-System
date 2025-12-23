
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment.Models;
using StudentPerformanceManagment.Models.ViewModel;


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

            // Redirect back to the list so the staff can continue with the next student
            return RedirectToAction("AddMark", new { subjectId = SubjectId });
        }
    }
}

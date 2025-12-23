
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment.Models;
using System.Security.Claims;

namespace StudentPerformanceManagment.Controllers
{
    public class StaffController : Controller
    {

        public IActionResult Dashboard()
        {
            return View();
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StaffController(UserManager<AppUser> userManager,
        ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult AddMark()
       {
            //var model= _context.Students.Select(s => new MarkViewModel { Prn = s.PRN, Name = s.Name });

            var model = new MarkViewModel
            {
                students = _context.Students.ToList(),

            };

              
            return View(model);
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

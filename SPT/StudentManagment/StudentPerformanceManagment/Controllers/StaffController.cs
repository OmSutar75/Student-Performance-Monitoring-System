
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Dashboard()
        {
            StaffDashViewModel staffDashViewModel = new StaffDashViewModel();
            List<Tasks> tasks = _context.Tasks.ToList();
            staffDashViewModel.Tasks = tasks;

            staffDashViewModel.TotalTask = tasks.Count();

            staffDashViewModel.StaffName = _userManager.GetUserName(User);
            staffDashViewModel.StaffId = _userManager.GetUserId(User);
            

            return View(staffDashViewModel);
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
    }
}

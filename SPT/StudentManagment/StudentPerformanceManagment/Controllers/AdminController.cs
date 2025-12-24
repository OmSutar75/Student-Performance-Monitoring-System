using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using StudentPerformanceManagment.Models;
using StudentPerformanceManagment;
using StudentPerformanceManagement.Models;

namespace IdentityDemo.Controllers
{
    

   
        [Authorize(Roles = "Admin")]
        public class AdminController : Controller
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly ApplicationDbContext _context;

            public AdminController(UserManager<AppUser> userManager,
                                   ApplicationDbContext context)
            {
                _userManager = userManager;
                _context = context;
            }
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

        // ADD STAFF (GET)
        [HttpGet]
            public IActionResult AddStaff()
            {
                return View();
            }

            // ADD STAFF (POST)
            [HttpPost]
            public async Task<IActionResult> AddStaff(string name, string email, string password,string mobile)
            {
            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = name,
                EmailConfirmed = true,
                //Mobile = mobile

            };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Staff");

                    var staff = new Staff
                    {
                        Name = name,
                        Email = email,
                        AppUserId = user.Id,
                        Mobile = mobile
                    };

                    _context.Staffs.Add(staff);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Dashboard", "Account");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View();
            }

            // ENROLL STUDENT (GET)
            [HttpGet]
            public IActionResult EnrollStudent()
            {
                return View();
            }

            // ENROLL STUDENT (POST)
            [HttpPost]
            public async Task<IActionResult> EnrollStudent(string name, string email,string mobile,int course,int groupid)
            {
                string defaultPassword = "Student@123";

                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FullName = name,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");

                    var student = new Student
                    {
                        Name = name,
                        Email = email,
                        AppUserId = user.Id,
                        MobileNo = mobile,
                        CourseId = course,
                        CourseGroupId= groupid

                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Dashboard", "Account");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View();
            }
        }
    }




using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NuGet.Versioning;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment;
using StudentPerformanceManagment.Models;
using StudentPerformanceManagment.Models.ViewModel;

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

 

            return View();

        }

        // ADD STAFF (GET)
        [HttpGet]
        public IActionResult AddStaff()
        {
            return View();
        }

        // ADD STAFF (POST)
        [HttpPost]
        public async Task<IActionResult> AddStaff(string name, string email, string password, string mobile)
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
        public async Task<IActionResult> EnrollStudent(string name, string email, string mobile, int course, int groupid)
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
                    CourseGroupId = groupid

                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                return RedirectToAction("Dashboard", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }



        #region Om 


        public IActionResult Courses()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }
        public IActionResult AddCourses()
        {
            
            return View();
        }


        // ADD COURSE (POST)
        [HttpPost]
      
        public IActionResult AddCourse(Course course)
        {
            if (!ModelState.IsValid)
                return View(course);

            Course course1 = _context.Courses.Where(c=>c.CourseName == course.CourseName).FirstOrDefault();
            if (course1 == null)
            {
                _context.Courses.Add(course);
                _context.SaveChanges();
                TempData["Success"] = "Course Add Successfully";
                return RedirectToAction("Dashboard");
            }
            TempData["Error"] = "Can't Add Course , Course Already Present ";
            return RedirectToAction("AddCourses");
        }

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        
        public IActionResult EditCourse(Course model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var course = _context.Courses.FirstOrDefault(c => c.CourseId == model.CourseId);

            if (course == null)
                return NotFound();

            course.CourseName = model.CourseName;
            course.Description = model.Description;
            course.Duration = model.Duration;
            course.Fees = model.Fees;

            _context.SaveChanges();

            return RedirectToAction("Courses");
        }




        public IActionResult CourseStudents(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
                return NotFound();

            var vm = new CourseStudentsVM
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                Students = _context.Students
                    .Where(s => s.CourseId == id)
                    .Select(s => new CourseStudentItemVM
                    {
                        PRN = s.PRN,
                        Name = s.Name,
                        Email = s.Email,
                        MobileNo = s.MobileNo,
                        CourseGroupName = s.CourseGroup.GroupName
                    })
                    .ToList()
            };

            return View(vm);
        }


        public IActionResult CourseGroups()
        {
            var groups = _context.CourseGroups.Include(c => c.Course).ToList();

            return View(groups);
        }
        public IActionResult AddCourseGroup()
        {
            AddCourseGroupMV mv = new AddCourseGroupMV()
            {
                Courses = _context.Courses.Select(
                    c => new SelectListItem()
                    {
                        Text = c.CourseName,
                        Value = c.CourseId.ToString()
                    }).ToList()


            };
            return View(mv);


        }
            [HttpPost]
            public IActionResult AddCourseGroup(AddCourseGroupMV mv)
            {

            var groups = new CourseGroup()
            {
                CourseId = mv.CourseId,
                GroupName = mv.CourseGroupName
            };

            _context.CourseGroups.Add(groups);
            _context.SaveChanges();
            return RedirectToAction("CourseGroups");
            }


        [HttpGet]
        public IActionResult EditCourseGroup(int id)
        {
            var group = _context.CourseGroups.Find(id);
            if (group == null)
                return NotFound();

            ViewBag.Courses = _context.Courses
                .Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.CourseName
                }).ToList();

            return View(group);
        }

        // EDIT (POST)
        [HttpPost]
        public IActionResult EditCourseGroup(CourseGroup model)
        {

            var group = _context.CourseGroups.Find(model.CourseGroupId);
            if (group == null)
                return NotFound();

            group.GroupName = model.GroupName;
            group.CourseId = model.CourseId;

            _context.SaveChanges();

            return RedirectToAction("CourseGroups");
        }

        #endregion

    }
}




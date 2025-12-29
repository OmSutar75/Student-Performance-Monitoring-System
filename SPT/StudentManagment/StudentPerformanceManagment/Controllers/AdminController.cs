using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        private async Task<AllModelCount> GetAllModelsCount()
        {
            var model = new AllModelCount
            {
                CourseCount = await _context.Courses.CountAsync(),
                StudentCount = await _context.Students.CountAsync(),
                SubjectCount = await _context.Subjects.CountAsync(),
                StaffCount = await _context.Staffs.CountAsync(),
                TotalTasks = await _context.Tasks.CountAsync(),
                PendingTasks = await _context.Tasks.CountAsync(t => t.Status == Status.Pending),
                CompletedTasks = await _context.Tasks.CountAsync(t => t.Status == Status.Completed)
            };
            return model; 
        }



        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var stats=await GetAllModelsCount();
            return View(stats);

        }



      #region //Staff all operations
        //List of staff
        public IActionResult Staff()
        {
            var staffs = _context.Staffs.ToList();
            return View(staffs);
        }

        // ADD STAFF (GET)
        [HttpGet]
        public IActionResult AddStaff()
        {
            return View();
        }

        // ADD STAFF (POST)
        [HttpPost]
        public async Task<IActionResult> AddStaff(string name, string email,  string mobile)
        {
            var tempPassword = "Temp@123";


            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = name,
                EmailConfirmed = true,
                //Mobile = mobile

            };

            var result = await _userManager.CreateAsync(user, tempPassword);

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


                string finalPassword = $"{staff.StaffId}@Sunbeam";


                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, finalPassword);

                TempData["Success"] = $"Staff added. Default Password: {finalPassword}";

                return RedirectToAction("Dashboard", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }

        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            var staff = _context.Staffs
                .Include(s => s.Tasks)
                .FirstOrDefault(s => s.StaffId == id);

            if (staff == null)
                return NotFound();

            return View(staff);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStaff(Staff model)
        {
            var staff = _context.Staffs
                .Include(s => s.Tasks)
                .FirstOrDefault(s => s.StaffId == model.StaffId);

            if (staff == null)
                return NotFound();

            staff.Name = model.Name;
            staff.Mobile = model.Mobile;

            _context.SaveChanges();

            return RedirectToAction("Staff");
        }

        public IActionResult ViewStaffTasks(int id)
        {
            var staff = _context.Staffs
                .Include(s => s.Tasks)
                    .ThenInclude(t => t.Course)
                .Include(s => s.Tasks)
                    .ThenInclude(t => t.CourseGroup)
                .Include(s => s.Tasks)
                    .ThenInclude(t => t.Subject)
                .FirstOrDefault(s => s.StaffId == id);

            if (staff == null) return NotFound();

            var vm = new StaffDashViewModel
            {
                StaffId = staff.StaffId,
                StaffName = staff.Name,
                TotalTasks = staff.Tasks.Count,
                PendingTasks = staff.Tasks.Count(t => t.Status == Status.Pending),
                CompletedTasks = staff.Tasks.Count(t => t.Status == Status.Completed)
            };

            vm.Tasks = staff.Tasks.Select(t => new TasksViewModel
            {
                TasksTitle = t.TasksTitle,
                TasksDescription = t.TasksDescription,
                CourseName = t.Course.CourseName,
                GroupName = t.CourseGroup.GroupName,
                SubjectName = t.Subject.SubjectName,
                ValidFrom = t.ValidFrom,
                ValidTo = t.ValidTo,
                Status = t.Status
            }).ToList();

            return View(vm);
        }

        #endregion
        public IActionResult Tasks()
        {
            var tasks = _context.Tasks
                .Include(t => t.Course)
                .Include(t => t.CourseGroup)
                .Include(t => t.Subject)
                .Include(t => t.Staff)
                .Select(t => new TasksViewModel
                {
                    TasksId = t.TasksId,
                    TasksTitle = t.TasksTitle,
                    TasksDescription = t.TasksDescription,
                    CourseName = t.Course.CourseName,
                    GroupName = t.CourseGroup.GroupName,
                    SubjectName = t.Subject.SubjectName,
                    StaffName = t.Staff.Name,
                    ValidFrom = t.ValidFrom,
                    ValidTo = t.ValidTo,
                    Status = t.Status
                })
                .ToList();

            return View(tasks);
        }



        [HttpGet]
        public IActionResult AddTask()
        {
            var vm = new TasksViewModel
            {
                Courses = _context.Courses.Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.CourseName
                }).ToList(),

                CourseGroups = _context.CourseGroups.Select(g => new SelectListItem
                {
                    Value = g.CourseGroupId.ToString(),
                    Text = g.GroupName
                }).ToList(),

                Subjects = _context.Subjects.Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = s.SubjectName
                }).ToList(),

                Staffs = _context.Staffs.Select(st => new SelectListItem
                {
                    Value = st.StaffId.ToString(),
                    Text = st.Name
                }).ToList()
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> AddTask(TasksViewModel vm)
        {
            var task = new Tasks
            {
                TasksTitle = vm.TasksTitle,
                TasksDescription = vm.TasksDescription,
                CourseId = vm.CourseId,
                CourseGroupId = vm.CourseGroupId,
                SubjectId = vm.SubjectId,
                StaffId = vm.StaffId,
                ValidFrom = vm.ValidFrom,
                ValidTo = vm.ValidTo,
                Status = Status.Pending
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Tasks");
        }

        public IActionResult GetSubjectsByCourse(int courseId)
        {
            var subjects = _context.Subjects
        .Where(s => s.CourseId == courseId)
        .Select(s => new
        {
            subjectId = s.SubjectId,
            subjectName = s.SubjectName
        })
        .ToList();

            return Json(subjects);
        }

        public IActionResult GetGroupsByCourse(int courseId)
        {
            var subjects = _context.CourseGroups
        .Where(s => s.CourseId == courseId)
        .Select(s => new
        {
            courseGroupId = s.CourseGroupId,
            groupName = s.GroupName
        })
        .ToList();

            return Json(subjects);
        }

        [HttpGet]
        public JsonResult GetSubjectsByCourses(int courseId)
        {
            var subjects = _context.Subjects
                .Where(s => s.CourseId == courseId)
                .Select(s => new
                {
                    s.SubjectId,
                    s.SubjectName
                }).ToList();

            return Json(subjects);
        }

        public IActionResult EditTask(int id)
        {
            var t = _context.Tasks.FirstOrDefault(x => x.TasksId == id);
            if (t == null) return NotFound();

            var vm = new TasksViewModel
            {
                TasksId = t.TasksId,
                TasksTitle = t.TasksTitle,
                TasksDescription = t.TasksDescription,
                CourseId = t.CourseId,
                CourseGroupId = t.CourseGroupId,
                SubjectId = t.SubjectId,
                StaffId = t.StaffId,
                ValidFrom = t.ValidFrom,
                ValidTo = t.ValidTo,

                Courses = _context.Courses.Select(c =>
                    new SelectListItem(c.CourseName, c.CourseId.ToString())).ToList(),

                CourseGroups = _context.CourseGroups.Select(g =>
                    new SelectListItem(g.GroupName, g.CourseGroupId.ToString())).ToList(),

                Subjects = _context.Subjects.Select(s =>
                    new SelectListItem(s.SubjectName, s.SubjectId.ToString())).ToList(),

                Staffs = _context.Staffs.Select(s =>
                    new SelectListItem(s.Name, s.StaffId.ToString())).ToList()
            };

            return View(vm);
        }




        [HttpPost]
        public async Task<IActionResult> EditTask(TasksViewModel vm)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.TasksId == vm.TasksId);
            if (task == null) return NotFound();

            task.TasksTitle = vm.TasksTitle;
            task.TasksDescription = vm.TasksDescription;
            task.CourseId = vm.CourseId;
            task.CourseGroupId = vm.CourseGroupId;
            task.SubjectId = vm.SubjectId;
            task.StaffId = vm.StaffId;
            task.ValidFrom = vm.ValidFrom;
            task.ValidTo = vm.ValidTo;

            await _context.SaveChangesAsync();
            return RedirectToAction("Tasks");
        }



        public IActionResult DeleteTask(int id)
        {
            var t = _context.Tasks.FirstOrDefault(x => x.TasksId == id);
            if (t == null) return NotFound();

            _context.Tasks.Remove(t);
            _context.SaveChanges();

            return RedirectToAction("Tasks");
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

            string defaultprofileimage = "/uploads/StudProfile.jpg";

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
                    CourseGroupId = groupid,
                    ProfileImagePath= defaultprofileimage

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




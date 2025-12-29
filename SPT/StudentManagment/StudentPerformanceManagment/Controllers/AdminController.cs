using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagement.ViewModel;
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


        #region Student

        public IActionResult Students()
        {
            var students = _context.Students
                .Include(s => s.Course)
                .Include(s => s.CourseGroup)
                .ToList();

            return View(students);
        }



        [HttpGet]
        public IActionResult EditStudent(int id)
        {

            var vm = new TasksViewModel
            {
                Courses = _context.Courses.Select(c => new SelectListItem
                {
                    Value = c.CourseId.ToString(),
                    Text = c.CourseName
                }).ToList(),

                // Empty initially - populated via AJAX
                CourseGroups = new List<SelectListItem>(),
                Subjects = new List<SelectListItem>(),

                Staffs = _context.Staffs.Select(st => new SelectListItem
                {
                    Value = st.StaffId.ToString(),
                    Text = st.Name
                }).ToList()
            };

            var student = _context.Students.FirstOrDefault(s => s.StudentId == id);

            if (student == null)
                return NotFound();


            return View(student);
        }


        [HttpPost]
        public async Task<IActionResult> EditStudent(Student model)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentId == model.StudentId);

            if (student == null)
                return NotFound();


            student.Name = model.Name;
            student.Email = model.Email;
            student.MobileNo = model.MobileNo;
            student.CourseId = model.CourseId;
            student.CourseGroupId = model.CourseGroupId;


            var user = await _userManager.FindByIdAsync(student.AppUserId);
            if (user != null)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);
            }

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("Students");
        }


        // DELETE STUDENT (POST)
        [HttpPost]


        [HttpGet]
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
                Status = t.Status,

                // All courses
                Courses = _context.Courses.Select(c =>
                    new SelectListItem(c.CourseName, c.CourseId.ToString())).ToList(),

                // Filter CourseGroups by task's CourseId
                CourseGroups = _context.CourseGroups
                    .Where(g => g.CourseId == t.CourseId)
                    .Select(g => new SelectListItem(g.GroupName, g.CourseGroupId.ToString()))
                    .ToList(),

                // Filter Subjects by task's CourseId
                Subjects = _context.Subjects
                    .Where(s => s.CourseId == t.CourseId)
                    .Select(s => new SelectListItem(s.SubjectName, s.SubjectId.ToString()))
                    .ToList(),

                // All staff
                Staffs = _context.Staffs.Select(s =>
                    new SelectListItem(s.Name, s.StaffId.ToString())).ToList()
            };

        public async Task<IActionResult> DeleteStudent(int id)
        {


            var student = _context.Students.FirstOrDefault(s => s.StudentId == id);
            if (student == null)
                return NotFound();


        [HttpPost]
        public async Task<IActionResult> EditTask(TasksViewModel vm)
        {
            var task = await _context.Tasks.FindAsync(vm.TasksId);
            if (task == null) return NotFound();

            task.TasksTitle = vm.TasksTitle;
            task.TasksDescription = vm.TasksDescription;
            task.CourseId = vm.CourseId;
            task.CourseGroupId = vm.CourseGroupId;
            task.SubjectId = vm.SubjectId;
            task.StaffId = vm.StaffId;
            task.ValidFrom = vm.ValidFrom;
            task.ValidTo = vm.ValidTo;
            task.Status = vm.Status;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Tasks");


            var user = await _userManager.FindByIdAsync(student.AppUserId);

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            if (user != null)
                await _userManager.DeleteAsync(user);

            return RedirectToAction("Students");

        }





        public string GeneratePRN()
        {
            //int year = DateTime.Now.Year;
            int year = DateTime.Now.Year;
            string basePart = year + "1000";

            var lastPRN = _context.Students
                            .OrderByDescending(s => s.PRN)
                            .Select(s => s.PRN)
                            .FirstOrDefault();

            if (lastPRN == null)
            {
                return basePart + "0001";
            }
            else
            {
                string last = lastPRN.Substring(basePart.Length);
                int next = int.Parse(last) + 1;

                return basePart + next.ToString("D4");
            }
        }

        [HttpGet]
        public IActionResult EnrollStudent()
        {
            var model = new StudentEnrollmentViewModel
            {
                Courses = _context.Courses
                    .Select(c => new SelectListItem
                    {
                        Text = c.CourseName.ToString(),
                        Value = c.CourseId.ToString(),
                    }).ToList(),

                CourseGroups = _context.CourseGroups
                    .Select(g => new SelectListItem
                    {
                        Text = g.GroupName.ToString(),
                        Value = g.CourseGroupId.ToString()
                    }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EnrollStudent(StudentEnrollmentViewModel model,
                                                 StudentEnrollmentViewModel1 model1)
        {
            if (!ModelState.IsValid)
            {
                model.Courses = _context.Courses.Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.CourseId.ToString()
                }).ToList();

                model.CourseGroups = _context.CourseGroups.Select(g => new SelectListItem
                {
                    Text = g.GroupName,
                    Value = g.CourseGroupId.ToString()
                }).ToList();

                return View(model);
            }

            string defaultPassword = "Student@123";

            var user = new AppUser
            {
                UserName = model1.Email,
                Email = model1.Email,
                FullName = model1.Name,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, defaultPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");

                var student = new Student
                {
                    PRN = GeneratePRN(),
                    Name = model1.Name,
                    Email = model1.Email,
                    AppUserId = user.Id,
                    MobileNo = model1.MobileNo,
                    CourseId = model.CourseId,
                    CourseGroupId = model.CourseGroupId,
                    ProfileImagePath = model1.ProfileImagePath
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                return RedirectToAction("Dashboard", "Account");
            }

            return View(model);
        }

        


        #endregion

        #region


        [HttpGet]
        public IActionResult SubjectWiseReport()
        {
            var model = new SubjectWiseReportVM
            {
                Courses = _context.Courses.Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.CourseId.ToString()
                }).ToList(),

                Subjects = new List<SelectListItem>()
            };

            return View(model);
        }

        [HttpGet]
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




        [HttpPost]
        public IActionResult SubjectWiseReport(SubjectWiseReportVM model)
        {
            model.Courses = _context.Courses.Select(c => new SelectListItem
            {
                Text = c.CourseName,
                Value = c.CourseId.ToString()
            }).ToList();

            model.ReportRows = _context.Marks
                .Include(m => m.Student)
                .Where(m => m.SubjectId == model.SubjectId)
                .Select(m => new StudentMarksRowVM
                {
                    PRN = m.Student.PRN,
                    StudentName = m.Student.Name,
                    TheoryMarks = m.TheoryMarks,
                    LabMarks = m.LabMarks,
                    InternalMarks = m.InternalMarks,
                    TotalMarks = 100,
                    ObtainedMarks = m.TheoryMarks + m.LabMarks + m.InternalMarks,
                    ResultStatus = (m.TheoryMarks + m.LabMarks + m.InternalMarks) >= 40 ? "Pass" : "Fail"
                }).ToList();

            return View(model);
        }




        [HttpGet]
        public IActionResult CourseWiseReport()
        {
            var model = new CourseWiseReportVM
            {
                Courses = _context.Courses.Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.CourseId.ToString()
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult CourseWiseReport(CourseWiseReportVM model)
        {

            model.Courses = _context.Courses.Select(c => new SelectListItem
            {
                Text = c.CourseName,
                Value = c.CourseId.ToString()
            }).ToList();

            int subjectCount = _context.Subjects.Count(s => s.CourseId == model.CourseId);
            int maxMarks = subjectCount * 100;

            var students = _context.Students
                .Where(s => s.CourseId == model.CourseId)
                .Select(s => new
                {
                    s.PRN,
                    s.Name,
                    Marks = s.Marks.Select(m => new
                    {
                        m.TheoryMarks,
                        m.LabMarks,
                        m.InternalMarks
                    }).ToList()
                }).ToList();

            var resultList = students.Select(s =>
            {
                int total = s.Marks.Sum(m => m.TheoryMarks + m.LabMarks + m.InternalMarks);

                bool isPass = s.Marks.All(m =>
                    m.TheoryMarks >= 16 &&
                    m.LabMarks >= 16 &&
                    m.InternalMarks >= 8);

                return new StudentRankingRowVM
                {
                    PRN = s.PRN,
                    StudentName = s.Name,
                    TotalMarks = total,
                    Percentage = Math.Round((double)total / maxMarks * 100, 2),
                    ResultStatus = isPass ? "PASS" : "FAIL"
                };
            })
            .OrderByDescending(x => x.TotalMarks)
            .ToList();

            int rank = 1;
            foreach (var item in resultList)
            {
                item.Rank = rank++;
            }

            model.RankingRows = resultList;
            return View(model);
        }
        #endregion

    }
}




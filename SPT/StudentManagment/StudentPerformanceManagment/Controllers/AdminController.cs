using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        public AdminController(UserManager<AppUser> userManager,
                               ApplicationDbContext context,IEmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;

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
        public async Task<IActionResult> AddStaff(string name, string email, string mobile)
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

                var subject = "Welcome! Your Staff registraion is Complete";

                var body = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                    <h2 style='color: #2c3e50;'>Welcome to the Portal, {staff.Name}!</h2>
                    <p>Congratulations, your registraion has been processed successfully. You can now access your staff dashboard using the credentials below:</p>
    
                    <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; border-left: 4px solid #007bff; margin: 20px 0;'>
                        <p style='margin: 5px 0;'><strong>Username :</strong> {staff.Email}</p>
                        <p style='margin: 5px 0;'><strong>Temporary Password:</strong> <code style='background: #eee; padding: 2px 5px;'>{finalPassword}</code></p>
                    </div>


                    <div style='text-align: center; margin-top: 30px;'>
                        <a href='yourportal.com' style='background-color: #007bff; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Login to Dashboard</a>
                    </div>

                    <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;' />
                    <p style='font-size: 0.8em; color: #777;'>
                        Regards,<br/>
                        <strong>Admin Team</strong><br/>
                        Student Performance Management System
                    </p>
              </div>";


                await _emailSender.SendEmailAsync(staff.Email, subject, body);
                return RedirectToAction("Dashboard", "Account");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }



        #endregion
        public IActionResult Tasks(int? taskStatus)
        {
            var query = _context.Tasks
            .Include(t => t.Course)
            .Include(t => t.CourseGroup)
            .Include(t => t.Subject)
            .Include(t => t.Staff)
            .AsQueryable();

            // Filtering Logic
            if (taskStatus.HasValue)
            {
                // Assuming your Status enum matches: 0=Pending, 1=Completed, 2=Overdue
                query = query.Where(t => (int)t.Status == taskStatus.Value);
            }

            var tasks = query.Select(t => new TasksViewModel
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

            // Pass the selected status back to the view to keep the dropdown selection
            ViewBag.SelectedStatus = taskStatus;

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

                // Empty initially - populated via AJAX
                CourseGroups = new List<SelectListItem>(),
                Subjects = new List<SelectListItem>(),

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

            Tasks tasks = _context.Tasks.Where(t => t.SubjectId == vm.SubjectId && t.CourseGroupId == vm.CourseGroupId).FirstOrDefault();
            if (tasks != null)
            {
                TempData["Error"] = "Task Is already Assign to some staff ";
                return RedirectToAction("AddTask");
            }
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

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(TasksViewModel vm)
        {
            Tasks tasks = _context.Tasks.Where(t => t.SubjectId == vm.SubjectId && t.CourseGroupId == vm.CourseGroupId).FirstOrDefault();
            if (tasks != null)
            {
                TempData["Error"] = "Task Is already Assign to some staff ";
                return RedirectToAction("AddTask");
            }
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
        }




        public IActionResult DeleteTask(int id)
        {
            var t = _context.Tasks.FirstOrDefault(x => x.TasksId == id);
            if (t == null) return NotFound();

            _context.Tasks.Remove(t);
            _context.SaveChanges();

            return RedirectToAction("Tasks");
        }


        

        #region Anurag
        public string GeneratePRN()
        {
            int year = DateTime.Now.Year;
            string basePart = year + "1000";

            var lastPRN = _context.Students
                .Where(s => s.PRN.StartsWith(basePart)) // ✅ filter current year
                .OrderByDescending(s => s.PRN)
                .Select(s => s.PRN)
                .FirstOrDefault();

            if (lastPRN == null)
            {
                return basePart + "0001";
            }

            string last = lastPRN.Substring(basePart.Length);
            int next = int.Parse(last) + 1;

            return basePart + next.ToString("D4");
        }

        // ENROLL STUDENT (GET)
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
        public async Task<IActionResult> EnrollStudent(StudentEnrollmentViewModel model)
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
            string profileimage = "/uploads/StudProfile.jpg";

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.Name,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, defaultPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");

                var student = new Student
                {
                    PRN = GeneratePRN(),
                    Name = model.Name,
                    Email = model.Email,
                    AppUserId = user.Id,
                    MobileNo = model.MobileNo,
                    CourseId = model.CourseId,
                    CourseGroupId = model.CourseGroupId,
                    ProfileImagePath = profileimage
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                //return RedirectToAction("Dashboard", "Account");
                var subject = "Welcome! Your Student Enrollment is Complete";

                var body = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                            <h2 style='color: #2c3e50;'>Welcome to the Portal, {student.Name}!</h2>
                            <p>Congratulations, your enrollment has been processed successfully. You can now access your student dashboard using the credentials below:</p>

                            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; border-left: 4px solid #007bff; margin: 20px 0;'>
                                <p style='margin: 5px 0;'><strong>Username (PRN):</strong> {student.PRN}</p>
                                <p style='margin: 5px 0;'><strong>Temporary Password:</strong> <code style='background: #eee; padding: 2px 5px;'>{defaultPassword}</code></p>
                            </div>

                            <p style='color: #e67e22;'><strong>Note:</strong> For security reasons, please change your password immediately after your first login.</p>

                            <div style='text-align: center; margin-top: 30px;'>
                                <a href='yourportal.com' style='background-color: #007bff; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Login to Dashboard</a>
                            </div>

                            <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;' />
                            <p style='font-size: 0.8em; color: #777;'>
                                Regards,<br/>
                                <strong>Admin Team</strong><br/>
                                Student Performance Management System
                            </p>
                      </div>";


                await _emailSender.SendEmailAsync(student.Email, subject, body);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return RedirectToAction("Dashboard");

        }


        #endregion

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
                return RedirectToAction("Courses");
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
            for (int i = 0; i < mv.GroupCount; i++)
            {
                var groups = new CourseGroup()
                {
                    CourseId = mv.CourseId,
                    GroupName = mv.GroupPrefix+ (i+1).ToString()
                };

                _context.CourseGroups.Add(groups);
            }
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







        public IActionResult Subjects()
        {
            var subject = _context.Subjects.Include(s=>s.Course).ToList();
            return View(subject);
        }
        public IActionResult AddSubjects()
        {
            SubjectViewModel mv = new SubjectViewModel()
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


        // ADD COURSE (POST)
        [HttpPost]

        public IActionResult AddSubjects(Subject subject)
        {
            

            Subject sub= _context.Subjects.Where(s => s.SubjectName == subject.SubjectName ).FirstOrDefault();

            
            if (sub == null)
            {
                subject.Course = _context.Courses.Find(subject.CourseId);
                _context.Subjects.Add(subject);
                _context.SaveChanges();
                TempData["Success"] = "Subject Add Successfully";
                return RedirectToAction("Subjects");
            }
            TempData["Error"] = "Can't Add Course , Course Already Present ";
            return RedirectToAction("AddSubjects");
        }

        [HttpGet]
        public IActionResult EditSubject(int id)
        {
            
            var subjects = _context.Subjects.Include(c=>c.Course).Where(s => s.SubjectId == id)
                .Select(s => new SubjectViewModel
                {

                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName,
                    CourseId = s.CourseId,
                    Courses = _context.Courses.Select(
                    c => new SelectListItem()
                    {
                        Text = c.CourseName,
                        Value = c.CourseId.ToString()
                    }).ToList(),

                    MaxTheoryMarks = s.MaxTheoryMarks,
                    MaxLabMarks = s.MaxLabMarks,
                    MaxInternalMarks = s.MaxInternalMarks,
                    PassingPercentEachComponent = s.PassingPercentEachComponent,
                    PassingPercentTotal = s.PassingPercentTotal


                }).FirstOrDefault();



            if (subjects == null)
                return NotFound();

            return View(subjects);
        }

        [HttpPost]

        public IActionResult EditSubject(SubjectViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var subject = _context.Subjects.Include(c=>c.Course).FirstOrDefault(s=>s.SubjectId == model.SubjectId);

            if (subject == null)
                return NotFound();

            subject.SubjectName= model.SubjectName;
            subject.MaxTheoryMarks = model.MaxTheoryMarks;
            subject.MaxLabMarks = model.MaxLabMarks;
            subject.MaxInternalMarks = model.MaxInternalMarks;
            subject.PassingPercentEachComponent = model.PassingPercentEachComponent;
            subject.PassingPercentTotal = model.PassingPercentTotal;
            subject.Course = _context.Courses.Find(model.CourseId);

            _context.SaveChanges();

            return RedirectToAction("Subjects");
        }




        #endregion

        #region report
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

                    ResultStatus =
                        (
                (m.TheoryMarks > 16 ? 1 : 0) + (m.LabMarks > 16 ? 1 : 0) + (m.InternalMarks > 8 ? 1 : 0)) >= 2 ? "PASS" : "FAIL"
                })
                .ToList();

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
            model.Courses = _context.Courses
                .Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.CourseId.ToString()
                })
                .ToList();

            if (model.CourseId == 0)
            {
                model.RankingRows = new List<StudentRankingRowVM>();
                return View(model);
            }

            const int MAX_SUBJECT_MARKS = 100;

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
                })
                .ToList();

            var resultList = students.Select(s =>
            {
                // ❗ If no marks entered → FAIL
                if (!s.Marks.Any())
                {
                    return new StudentRankingRowVM
                    {
                        PRN = s.PRN,
                        StudentName = s.Name,
                        TotalMarks = 0,
                        Percentage = 0,
                        ResultStatus = "FAIL",
                        Rank = 0
                    };
                }

                int totalMarks = s.Marks.Sum(m =>
                    m.TheoryMarks + m.LabMarks + m.InternalMarks);

                bool isPass = s.Marks.All(m =>
                {
                    return m.TheoryMarks >= 16
                        && m.LabMarks >= 16
                        && m.InternalMarks >= 8;
                });

                return new StudentRankingRowVM
                {
                    PRN = s.PRN,
                    StudentName = s.Name,
                    TotalMarks = totalMarks,

                    // ✅ CORRECT PERCENTAGE
                    Percentage = Math.Round(
                        (double)totalMarks / (s.Marks.Count * MAX_SUBJECT_MARKS) * 100, 2),

                    ResultStatus = isPass ? "PASS" : "FAIL"
                };
            })
            .OrderByDescending(x => x.TotalMarks)
            .ToList();

            // Rank only PASS students
            int rank = 1;
            foreach (var item in resultList.Where(r => r.ResultStatus == "PASS"))
            {
                item.Rank = rank++;
            }

            foreach (var item in resultList.Where(r => r.ResultStatus == "FAIL"))
            {
                item.Rank = 0;
            }

            model.RankingRows = resultList;
            return View(model);
        }


        #endregion

    }
}




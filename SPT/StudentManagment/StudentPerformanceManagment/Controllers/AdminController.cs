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
using Microsoft.EntityFrameworkCore;

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

        //// ENROLL STUDENT (GET)
        //[HttpGet]
        //public IActionResult EnrollStudent()
        //{
        //    return View();
        //}

        //// ENROLL STUDENT (POST)
        //[HttpPost]
        //public async Task<IActionResult> EnrollStudent(string name, string email, string mobile, int course, int groupid)
        //{
        //    string defaultPassword = "Student@123";

        //    var user = new AppUser
        //    {
        //        UserName = email,
        //        Email = email,
        //        FullName = name,
        //        EmailConfirmed = true
        //    };

        //    var result = await _userManager.CreateAsync(user, defaultPassword);

        //    if (result.Succeeded)
        //    {
        //        await _userManager.AddToRoleAsync(user, "Student");

        //        var student = new Student
        //        {
        //            Name = name,
        //            Email = email,
        //            AppUserId = user.Id,
        //            MobileNo = mobile,
        //            CourseId = course,
        //            CourseGroupId = groupid

        //        };

        //        _context.Students.Add(student);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction("Dashboard", "Account");
        //    }

        //    foreach (var error in result.Errors)
        //        ModelState.AddModelError("", error.Description);

        //    return View();
        //}



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

        public async Task<IActionResult> DeleteStudent(int id)
        {

            var student = _context.Students.FirstOrDefault(s => s.StudentId == id);
            if (student == null)
                return NotFound();


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
            int year = 2026;
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

        //[HttpPost]
        //public async Task<IActionResult> EnrollStudent(StudentEnrollmentViewModel model, StudentEnrollmentViewModel1 model1)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Ensure all courses and groups are populated in the model if validation fails
        //        model.Courses = _context.Courses.Select(c => new SelectListItem
        //        {
        //            Text = c.CourseName,
        //            Value = c.CourseId.ToString()
        //        }).ToList();

        //        model.CourseGroups = _context.CourseGroups.Select(g => new SelectListItem
        //        {
        //            Text = g.GroupName,
        //            Value = g.CourseGroupId.ToString()
        //        }).ToList();

        //        return View(model);
        //    }

        //    // Log to see model1 values
        //    Debug.WriteLine($"Name: {model1.Name}, Email: {model1.Email}, Mobile: {model1.MobileNo}");

        //    string defaultPassword = "Student@123";

        //    var user = new AppUser
        //    {
        //        UserName = model1.Email,
        //        Email = model1.Email,
        //        FullName = model1.Name,
        //        EmailConfirmed = true
        //    };

        //    var result = await _userManager.CreateAsync(user, defaultPassword);
        //    if (!result.Succeeded)
        //    {
        //        // Log errors if user creation fails
        //        foreach (var error in result.Errors)
        //            ModelState.AddModelError("", error.Description);
        //        return View(model);
        //    }

        //    await _userManager.AddToRoleAsync(user, "Student");

        //    var student = new Student
        //    {
        //        PRN = GeneratePRN(),  // Ensure PRN generation is correct
        //        Name = model1.Name,
        //        Email = model1.Email,
        //        AppUserId = user.Id,
        //        MobileNo = model1.MobileNo,
        //        CourseId = model.CourseId,
        //        CourseGroupId = model.CourseGroupId,
        //        ProfileImagePath = model1.ProfileImagePath
        //    };

        //    _context.Students.Add(student);

        //    try
        //    {
        //        await _context.SaveChangesAsync();  // Ensure changes are committed to DB
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle exceptions during SaveChangesAsync
        //        Debug.WriteLine("Error saving student: " + ex.Message);
        //    }

        //    return RedirectToAction("Dashboard", "Account");
        //}



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
                    subjectId = s.SubjectId,      // 🔑 must match JS
                    subjectName = s.SubjectName   // 🔑 must match JS
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




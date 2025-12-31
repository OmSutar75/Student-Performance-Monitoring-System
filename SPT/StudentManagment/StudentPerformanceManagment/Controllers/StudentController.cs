using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment;
using StudentPerformanceManagment.Models;
using StudentPerformanceManagment.Models.ViewModel;
using System.Collections.Immutable;
using System.Security.Claims;



namespace StudentPerformanceManagement.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {

        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StudentController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        private async Task<StudentViewModel> GetData()
        {
            var userId = _userManager.GetUserId(User);

            // 1. Single Query with Joins: Student, Course, aur Group ko ek saath fetch karein
            var student = await _context.Students
                .Include(s => s.Course)
                .Include(s => s.CourseGroup)
                .Where(s => s.AppUserId == userId)
                .FirstOrDefaultAsync();

            if (student == null) return new StudentViewModel();

            // 2. Optimized Count: Subject count ke liye alag query
            int subjectCount = 0;
            if (student.CourseId != null)
            {
                subjectCount = await _context.Subjects
                    .CountAsync(s => s.CourseId == student.CourseId);
            }

            //Finding the Rank 

            var std = _context.Students
                          .FirstOrDefault(s => s.Email == _userManager.GetUserName(User));

            //var student = _context.Students
            //    .Include(s => s.Course)
            //    .Include(s => s.CourseGroup)
            //    .Include(s => s.Marks)
            //    .ThenInclude(m => m.Subject)
            //    .FirstOrDefault(s => s.StudentId == std.StudentId);
           
            int rank = GetStudentRank(std.StudentId, student.CourseId);

            // 3. Mapping to ViewModel
            var stud = new StudentViewModel()
            {
                StudentId = student.StudentId,
                PRN = student.PRN,
                Name = student.Name,
                Email = User.Identity?.Name, // Identity se email lena fast hai
                CourseName = student.Course?.CourseName ?? "N/A",
                SubjectCount = subjectCount,
                CourseGroupName = student.CourseGroup?.GroupName ?? "N/A",
                MobileNo = student.MobileNo,
                ProfileImage = student.ProfileImagePath,
                Rank = rank
            };

            return stud;
        }


        public async Task<IActionResult> Dashboard()
        {

            var stud = await GetData();
            return View(stud);

        }

   [HttpGet]
        public async Task<IActionResult> EditProfile() 
        {
            var stud = await GetData(); 
            return View(stud);
        }

        [HttpPost]
        public async Task<IActionResult> AfterEditProfile(StudentViewModel model)
        {
        

            var userId = _userManager.GetUserId(User);
            var appUser = await _userManager.FindByIdAsync(userId);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.AppUserId == userId);

            if (student == null) return NotFound();

            // 2. Profile Data Update (Name & Mobile)
            student.Name = model.Name;
            student.MobileNo = model.MobileNo;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();



            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Dashboard");
        }

        private async Task<string> SaveProfileImageAsync(IFormFile? profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
                return string.Empty;

            // Generate unique file name
            var fileName = $"{Guid.NewGuid()}_{profileImage.FileName}";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            // Ensure uploads folder exists
            Directory.CreateDirectory(Path.GetDirectoryName(uploadPath)!);

            // Save file
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            // Return relative path to store in DB
            return $"/uploads/{fileName}";

        }

        public async Task<IActionResult> ProfileImage()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfileImage(IFormFile profileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

   
            if (profileImage != null && profileImage.Length > 0)
            {

                string imagePath = await SaveProfileImageAsync(profileImage);

                var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == user.Email);
                if (student != null)
                {
                    student.ProfileImagePath = imagePath;
                    _context.Update(student);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Profile image updated successfully!";
                    return RedirectToAction("Dashboard");
                }
            }

            ModelState.AddModelError("", "Please select a valid image file.");
            return View();
        }


    
        public IActionResult ChangePassword()
        {
            
            return View(new PasswordViewModel());
        }


    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(PasswordViewModel model)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

           
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password updated successfully!";
                return RedirectToAction("Dashboard", "Student");
            }
        
            foreach (var error in result.Errors)
            {
              
                if (error.Code.Contains("PasswordMismatch"))
                {
                    ModelState.AddModelError("CurrentPassword", "The current password you entered is incorrect.");
                }
                else
                {
                 
                    ModelState.AddModelError("NewPassword", error.Description);
                }
            }

            return View("ChangePassword", model);
        }


        #region Performance Card

        public int GetStudentRank(int studentId, int courseId)
        {
            var students = _context.Students.Where(s => s.CourseId == courseId).Include(s => s.Marks).ToList();
            var markList = students.Select(s => new StudentMarks
            {
                StudentId = s.StudentId,
                TotalMarks = s.Marks.Sum(m => m.TotalObtained)
            }
            ).OrderByDescending(sm => sm.TotalMarks);

            int rank = 0;
            int prevMarks = -1;

            foreach (var item in markList)
            {
                if (item.TotalMarks != prevMarks)
                {
                    rank++;
                    prevMarks = item.TotalMarks;
                }

                if (item.StudentId == studentId)
                    return rank;
            }
            return 0;// student not found
        }

        public IActionResult PerformanceCard()
        {

            var std = _context.Students
                          .FirstOrDefault(s => s.Email == _userManager.GetUserName(User));

            var student = _context.Students
                .Include(s => s.Course)
                .Include(s => s.CourseGroup)
                .Include(s => s.Marks)
                .ThenInclude(m => m.Subject)
                .FirstOrDefault(s => s.StudentId == std.StudentId);
            /*var subjects = _db.Students
                .Include(s => s.Marks)
                    .ThenInclude(m => m.Subject).ToList();*/
            int rank = GetStudentRank(std.StudentId, student.CourseId);
            if (student == null)
                return NotFound();

            var vm = new PerformanceCard
            {
                Rank = rank,
                StudentPRN = student.PRN,
                StudentName = student.Name,
                CourseName = student.Course.CourseName,
                Subjects = student.Marks.Select(m => new SubjectMarksViewModel
                {
                    SubjectName = m.Subject.SubjectName,
                    Theory = m.TheoryMarks,
                    Lab = m.LabMarks,
                    Internal = m.InternalMarks,
                    Total = m.TotalObtained,
                    Status = m.ResultStatus == "Pass" ? true : false,
                    MaxMarks = m.Subject.MaxLabMarks + m.Subject.MaxLabMarks + m.Subject.MaxInternalMarks,
                    FailedIn = m.FailedIn(),
                }).ToList()
            };

            return View(vm);
        }

        #endregion

        #region report
        [HttpGet]
        public JsonResult GetSubjectByCourse(int courseId)
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
        public IActionResult SubjectWiseReport()
        {
            var model = new SubjectWiseReportVM
            {
                Courses = _context.Courses.Include(c => c.Subjects).Select(c => new SelectListItem
                {
                    Text = c.CourseName,
                    Value = c.CourseId.ToString()
                }).ToList()
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

            var list = _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Where(m => m.SubjectId == model.SubjectId)
                .ToList()
                .Select(m =>
                {
                    bool isPass = false;
                    string failed = "";

                    bool marksEntered = !(m.TheoryMarks == 0 && m.LabMarks == 0 && m.InternalMarks == 0);

                    if (marksEntered)
                    {
                        if (m.TheoryMarks < (m.Subject.MaxTheoryMarks * 0.4)) failed += "T";
                        if (m.LabMarks < (m.Subject.MaxLabMarks * 0.4)) failed += "L";
                        if (m.InternalMarks < (m.Subject.MaxInternalMarks * 0.4)) failed += "I";

                        isPass = failed.Length < 2;
                    }
                    else
                    {
                        failed = "NA";
                        isPass = false;
                    }

                    return new StudentMarksRowVM
                    {
                        PRN = m.Student.PRN,
                        StudentName = m.Student.Name,
                        TheoryMarks = m.TheoryMarks,
                        LabMarks = m.LabMarks,
                        InternalMarks = m.InternalMarks,
                        TotalMarks = 100,
                        ObtainedMarks = m.TheoryMarks + m.LabMarks + m.InternalMarks,
                        FailedIn = failed,
                        ResultStatus = marksEntered ? (isPass ? "Pass" : "Fail") : "Not Entered"
                    };
                })
                .OrderByDescending(x => x.ObtainedMarks)
                .ToList();




            int rank = 1;
            int prevMarks = -1;
            int skip = 0;

            foreach (var item in list)
            {
                if (item.ObtainedMarks == prevMarks)
                {
                    item.Rank = rank;
                    skip++;
                }
                else
                {
                    rank += skip;
                    item.Rank = rank;
                    skip = 1;
                    prevMarks = item.ObtainedMarks;
                }
            }

            model.ReportRows = list;
            return View(model);
        }

        #endregion

        #region Student Course-Wise Ranking Report 
        [Authorize(Roles = "Student")]

        public async Task<IActionResult> CourseWiseReport()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var currentStudent = await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.AppUserId == user.Id);

            if (currentStudent == null)
                return Unauthorized();

            var students = await _context.Students
                .Include(s => s.Marks)
                    .ThenInclude(m => m.Subject)
                .Where(s => s.CourseId == currentStudent.CourseId)
                .ToListAsync();

            var subjectNames = await _context.Subjects
                .Where(s => s.CourseId == currentStudent.CourseId)
                .Select(s => s.SubjectName)
                .ToListAsync();

            var rankingList = students.Select(s =>
            {
                int total = 0;
                bool courseFail = false;
                var marksList = new List<SubjectMarksVM>();

                foreach (var subjectName in subjectNames)
                {
                    var m = s.Marks.FirstOrDefault(x => x.Subject.SubjectName == subjectName);

                    // No subject record
                    if (m == null)
                    {
                        courseFail = true;

                        marksList.Add(new SubjectMarksVM
                        {
                            SubjectName = subjectName,
                            Theory = 0,
                            Lab = 0,
                            Internal = 0,
                            FailedIn = "N"
                        });

                        continue;
                    }

                    string failed = "";

                    bool notEntered = m.TheoryMarks == 0 && m.LabMarks == 0 && m.InternalMarks == 0;

                    if (notEntered)
                    {
                        courseFail = true;

                        marksList.Add(new SubjectMarksVM
                        {
                            SubjectName = m.Subject.SubjectName,
                            Theory = m.TheoryMarks,
                            Lab = m.LabMarks,
                            Internal = m.InternalMarks,
                            FailedIn = "N"
                        });

                        continue;
                    }

                    bool isTheoryPass = m.TheoryMarks >= 16;
                    bool isLabPass = m.LabMarks >= 16;
                    bool isInternalPass = m.InternalMarks >= 8;

                    int passCount = 0;
                    if (isTheoryPass) passCount++;
                    if (isLabPass) passCount++;
                    if (isInternalPass) passCount++;

                    if (!isTheoryPass) failed += "T";
                    if (!isLabPass) failed += "L";
                    if (!isInternalPass) failed += "I";

                    if (passCount < 2)
                        courseFail = true;

                    marksList.Add(new SubjectMarksVM
                    {
                        SubjectName = m.Subject.SubjectName,
                        Theory = m.TheoryMarks,
                        Lab = m.LabMarks,
                        Internal = m.InternalMarks,
                        FailedIn = failed
                    });

                    total += m.TheoryMarks + m.LabMarks + m.InternalMarks;
                }

                return new StudentRankingRowVM
                {
                    PRN = s.PRN,
                    StudentName = s.Name,
                    SubjectMarks = marksList,
                    TotalMarks = total,
                    Percentage = subjectNames.Count == 0 ? 0 :
                        Math.Round((double)total / (subjectNames.Count * 100) * 100, 2),
                    ResultStatus = courseFail ? "FAIL" : "PASS"
                };
            })
            .OrderByDescending(x => x.TotalMarks)
            .ThenBy(x => x.StudentName)
            .ToList();

            int rank = 0, prev = -1;
            foreach (var item in rankingList)
            {
                if (item.TotalMarks == prev)
                    item.Rank = rank;
                else
                {
                    rank++;
                    item.Rank = rank;
                    prev = item.TotalMarks;
                }
            }

            var model = new CourseWiseReportVM
            {
                SubjectNames = subjectNames,
                RankingRows = rankingList,
            };

            return View(model);
        }







        //public async Task<IActionResult> CourseWiseReport()
        //{
        //    // 1. Get logged-in user
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //        return Unauthorized();

        //    // 2. Get logged-in student's record
        //    var currentStudent = await _context.Students
        //        .Include(s => s.Course)
        //        .FirstOrDefaultAsync(s => s.AppUserId == user.Id);

        //    if (currentStudent == null)
        //        return Unauthorized();

        //    // 3. Get all students from the same course with marks
        //    var students = await _context.Students
        //        .Include(s => s.Marks)
        //            .ThenInclude(m => m.Subject)
        //        .Where(s => s.CourseId == currentStudent.CourseId)
        //        .ToListAsync();

        //    // 4. Get subject names
        //    var subjectNames = await _context.Subjects
        //        .Where(s => s.CourseId == currentStudent.CourseId)
        //        .Select(s => s.SubjectName)
        //        .ToListAsync();

        //    // 5. Build ranking rows
        //    var rankingList = students.Select(s =>
        //    {
        //        int total = 0;
        //        bool courseFail = false;
        //        var marksList = new List<SubjectMarksVM>();

        //        foreach (var m in s.Marks)
        //        {
        //            string failed = "";

        //            bool isTheoryPass = m.TheoryMarks >= 16;
        //            bool isLabPass = m.LabMarks >= 16;
        //            bool isInternalPass = m.InternalMarks >= 8;

        //            int passCount = 0;
        //            if (isTheoryPass) passCount++;
        //            if (isLabPass) passCount++;
        //            if (isInternalPass) passCount++;

        //            if (!isTheoryPass) failed += "T";
        //            if (!isLabPass) failed += "L";
        //            if (!isInternalPass) failed += "I";

        //            // Subject fails only if less than 2 sections pass
        //            if (passCount < 2)
        //                courseFail = true;

        //            marksList.Add(new SubjectMarksVM
        //            {
        //                SubjectName = m.Subject.SubjectName,
        //                Theory = m.TheoryMarks,
        //                Lab = m.LabMarks,
        //                Internal = m.InternalMarks,
        //                FailedIn = failed
        //            });

        //            total += m.TheoryMarks + m.LabMarks + m.InternalMarks;
        //        }

        //        return new StudentRankingRowVM
        //        {
        //            PRN = s.PRN,
        //            StudentName = s.Name,
        //            SubjectMarks = marksList,
        //            TotalMarks = total,
        //            Percentage = Math.Round((double)total / (subjectNames.Count * 100) * 100, 2),
        //            ResultStatus = courseFail ? "FAIL" : "PASS"
        //        };
        //    })
        //    .OrderByDescending(x => x.TotalMarks)
        //    .ThenBy(x => x.StudentName)
        //    .ToList();

        //    // 6. Assign ranks with ties
        //    int rank = 0, prev = -1;
        //    foreach (var item in rankingList)
        //    {
        //        if (item.TotalMarks == prev)
        //        {
        //            item.Rank = rank;
        //        }
        //        else
        //        {
        //            rank++;
        //            item.Rank = rank;
        //            prev = item.TotalMarks;
        //        }
        //    }

        //    // 7. Build ViewModel
        //    var model = new CourseWiseReportVM
        //    {
        //        SubjectNames = subjectNames,
        //        RankingRows = rankingList,
        //    };

        //    return View(model);
        //}

        #endregion



    }
}

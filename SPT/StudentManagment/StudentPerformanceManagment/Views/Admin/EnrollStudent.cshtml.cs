using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPerformanceManagement.Models;
using StudentPerformanceManagment;

public class EnrollStudentModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EnrollStudentModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public StudentInputModel Input { get; set; } = new();

    public List<SelectListItem> CourseList { get; set; } = new();
    public List<SelectListItem> CourseGroupList { get; set; } = new();

    public void OnGet()
    {
        LoadCourses();
    }

    public IActionResult OnPostLoadGroups()
    {
        LoadCourses();
        LoadGroups(Input.CourseId);
        return Page();
    }

    public IActionResult OnPostEnroll()
    {
        LoadCourses();
        LoadGroups(Input.CourseId);

        // Auto-generate PRN
        string prn = GeneratePRN();

        var student = new Student
        {
            Name = Input.FullName,
            Email = Input.Email,
            MobileNo = Input.Mobile,
            CourseId = Input.CourseId,
            CourseGroupId = Input.CourseGroupId,
            PRN = prn
        };

        _context.Students.Add(student);
        _context.SaveChanges();

        return RedirectToPage("EnrollSuccess", new { prn });
    }

    private void LoadCourses()
    {
        CourseList = _context.Courses
            .Select(c => new SelectListItem
            {
                Value = c.CourseId.ToString(),
                Text = c.CourseName
            })
            .ToList();
    }

    private void LoadGroups(int courseId)
    {
        CourseGroupList = _context.CourseGroups
            .Where(g => g.CourseId == courseId)
            .Select(g => new SelectListItem
            {
                Value = g.CourseGroupId.ToString(),
                Text = g.GroupName
            })
            .ToList();
    }

    private string GeneratePRN()
    {
        return DateTime.Now.Year.ToString() +
               new Random().Next(100000, 999999).ToString();
    }

    public class StudentInputModel
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Mobile { get; set; } = "";
        public int CourseId { get; set; }
        public int CourseGroupId { get; set; }
    }
}

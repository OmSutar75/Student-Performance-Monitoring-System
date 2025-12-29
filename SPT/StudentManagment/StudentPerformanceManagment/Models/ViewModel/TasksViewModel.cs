using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class TasksViewModel
    {
        public int TasksId { get; set; }

        public string TasksTitle { get; set; } = "";
        public string TasksDescription { get; set; } = "";

        public int CourseId { get; set; }
        public int CourseGroupId { get; set; }
        public int SubjectId { get; set; }
        public int StaffId { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        // Display-only fields
        public string CourseName { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public string StaffName { get; set; } = "";

        // KEEP STRING (VERY IMPORTANT)
        /*public string Status { get; set; } = "";*/
        public Status Status { get; set; } = Status.Pending;

        // Dropdowns
        public List<SelectListItem> Courses { get; set; } = new();
        public List<SelectListItem> CourseGroups { get; set; } = new();
        public List<SelectListItem> Subjects { get; set; } = new();
        public List<SelectListItem> Staffs { get; set; } = new();
    }
}
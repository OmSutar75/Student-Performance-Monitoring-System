using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagement.ViewModel
{
    public class StudentEnrollmentViewModel
    {
        [Required(ErrorMessage = "Please select a course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please select a course group")]
        public int CourseGroupId { get; set; }

        public List<SelectListItem> Courses { get; set; } = new();
        public List<SelectListItem> CourseGroups { get; set; } = new();

        public string Name { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }

        public string ProfileImagePath { get; set; } = "profile.img";

    }

}
using StudentPerformanceManagment.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagement.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        [MaxLength(12)]
        public string PRN { get; set; } = "11111111";
        public string Name { get; set; }
        [EmailAddress(ErrorMessage ="Invalid Email!")]
        public string Email { get; set; }
        [MaxLength(10)]
        public string? MobileNo { get; set; }

        public string ProfileImagePath { get; set; } = "URL";

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int CourseGroupId { get; set; }
        public CourseGroup CourseGroup { get; set; } = null;

        public ICollection<Mark> Marks { get; set; } = new List<Mark>();

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }


    }
}

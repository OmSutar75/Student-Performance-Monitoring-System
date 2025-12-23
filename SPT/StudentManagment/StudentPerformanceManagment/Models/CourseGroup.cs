using StudentPerformanceManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagment.Models
{
    public class CourseGroup
    {
        [Key]
        public int CourseGroupId { get; set; }
        public string GroupName { get; set; }


        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class StudentViewModel
    {
        public string PRN { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string CourseName { get; set; }

        public int SubjectCount { get; set; }
        public string CourseGroupName { get; set; }

    }
}

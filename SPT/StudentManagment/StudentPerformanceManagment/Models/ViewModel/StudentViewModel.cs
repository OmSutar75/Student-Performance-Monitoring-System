using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class StudentViewModel
    {

        public int StudentId { get; set; }
        public string PRN { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string CourseName { get; set; }

        public int SubjectCount { get; set; }
        public string CourseGroupName { get; set; }

        public string ProfileImage { get; set; }
        public int Rank { get; set; }

    }
}

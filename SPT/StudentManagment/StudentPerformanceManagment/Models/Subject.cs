using System.ComponentModel.DataAnnotations;
using StudentPerformanceManagment.Models;   

namespace StudentPerformanceManagment.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SubjectName { get; set; } = string.Empty;

        public int MaxTheoryMarks { get; set; } = 40;
        public int MaxLabMarks { get; set; } = 40;
        public int MaxInternalMarks { get; set; } = 20;

        public int PassingPercentTotal { get; set; } = 40; //out of all subject
        public int PassingPercentEachComponent { get; set; } = 40; //for theory,lab,internal

        public int CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<Mark> Mark { get; set; } = new List<Mark>();




    }
}

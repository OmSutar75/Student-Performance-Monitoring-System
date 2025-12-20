using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagment.Models
{
    public class Mark
    {
        [Key]
        public int MarkId { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int TheoryMarks { get; set; }
        public int LabMarks { get; set; }
        public int InternalMarks { get; set; }
    }
}

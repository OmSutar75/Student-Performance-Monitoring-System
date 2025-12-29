namespace StudentPerformanceManagment.Models.ViewModel
{
    public class UpdateStudentViewModel
    {
        public static int markcount { get; set; } = 0;
        public static int studcount { get; set; } = 0;
        public int TheoryMarks { get; set; }
        public int LabMarks { get; set; }
        public int InternalMarks { get; set; }
        public int SubjectId { get; set; }

        public int CourseGroupId { get; set; }
        public int CourseId { get; set; }
        public int TaskId { get; set; }
        public int StudentId { get; set; }
        public string PRN { get; set; }
        public string Name { get;  set; }


    }
}

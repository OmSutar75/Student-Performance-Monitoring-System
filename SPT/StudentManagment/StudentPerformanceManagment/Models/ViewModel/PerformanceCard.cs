namespace StudentPerformanceManagment.Models.ViewModel
{
    public class PerformanceCard
    {
        public int Rank { get; set; }
        public string StudentPRN { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public List<SubjectMarksViewModel> Subjects { get; set; }
    }
}

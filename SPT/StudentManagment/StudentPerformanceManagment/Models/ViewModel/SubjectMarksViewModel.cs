namespace StudentPerformanceManagment.Models.ViewModel
{
    public class SubjectMarksViewModel
    {
        public string SubjectName { get; set; }
        public int Theory { get; set; }
        public int Lab { get; set; }
        public int Internal { get; set; }
        public int Total { get; set; }
        public bool Status { get; set; }
        public int MaxMarks { get; set; }
        public string FailedIn { get; set; } = "-";
    }
}

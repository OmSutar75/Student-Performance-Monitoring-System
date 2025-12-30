using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class SubjectWiseReportVM
    {
        public int CourseId { get; set; }
        public int SubjectId { get; set; }

        public List<SelectListItem> Courses { get; set; }
        public List<StudentMarksRowVM> ReportRows { get; set; }
        public List<SelectListItem> Subjects { get; set; }
    }

    public class StudentMarksRowVM
    {
        public string PRN { get; set; }
        public string StudentName { get; set; }
        public Dictionary<string, int> SubjectMarks { get; set; }

        public int TheoryMarks { get; set; }
        public int LabMarks { get; set; }
        public int InternalMarks { get; set; }

        public int TotalMarks { get; set; }
        public int ObtainedMarks { get; set; }

        public string FailedIn { get; set; }

        public string ResultStatus { get; set; }
        public int Rank { get; set; }
    }
}

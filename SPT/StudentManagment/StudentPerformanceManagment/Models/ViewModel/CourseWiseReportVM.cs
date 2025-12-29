using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class CourseWiseReportVM
    {

        public int CourseId { get; set; }
        public List<SelectListItem> Courses { get; set; }

        public List<string> SubjectNames { get; set; }
        public List<StudentRankingRowVM> RankingRows { get; set; }
        public int StudentId { get; set; }

    }

    public class StudentRankingRowVM
    {
        public int Rank { get; set; }
        public string PRN { get; set; }
        public string StudentName { get; set; }

        public List<SubjectMarksVM> SubjectMarks { get; set; }

        public int TotalMarks { get; set; }
        public double Percentage { get; set; }
        public string ResultStatus { get; set; }   // PASS / FAIL
    }

    public class SubjectMarksVM
    {
        public string SubjectName { get; set; }
        public int Theory { get; set; }
        public int Lab { get; set; }
        public int Internal { get; set; }
        public string FailedIn { get; set; }   // T / L / I / TL / TLI
    }
}
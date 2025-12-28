using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class CourseWiseReportVM
    {
        public int CourseId { get; set; }
        public List<SelectListItem> Courses { get; set; }
        public List<StudentRankingRowVM> RankingRows { get; set; }
    }

    public class StudentRankingRowVM
    {
        public string PRN { get; set; }
        public string StudentName { get; set; }

        public int TotalMarks { get; set; }
        public double Percentage { get; set; }
        public int Rank { get; set; }
        public string ResultStatus { get; set; }   // PASS / FAIL
    }
}

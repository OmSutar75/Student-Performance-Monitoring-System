using StudentPerformanceManagement.Models;
using System.ComponentModel;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class MarkViewModel
    {

        /*public string Prn {  get; set; }
        public string Name { get; set; }*/

        public int TotalMark(int i, int j, int k) => i + j + k;
        public List<Student> Students { get; set; }

        public int TheoryMarks { get; set; }
        public int LabMarks { get; set; }
        public int InternalMarks { get; set; }

        public int Total { get; set; }

        public string Status { get; set; }
        public int SubjectId { get; set; }

        public int CourseGroupId { get; set; }
        public int CourseId { get; set; }
        public int TaskId { get; set; }
        public int StudentId { get; internal set; }
        public string PRN { get; set; }
        public string Name { get; internal set; }

    }
}
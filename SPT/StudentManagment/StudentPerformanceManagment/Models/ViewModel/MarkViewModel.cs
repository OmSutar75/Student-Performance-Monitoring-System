using StudentPerformanceManagement.Models;
using System.ComponentModel;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class MarkViewModel
    {

        /*public string Prn {  get; set; }
        public string Name { get; set; }*/

        public List<Student> students { get; set; }

        public int TheoryMarks { get; set; }
        public int LabMarks { get; set; }
        public int InternalMarks { get; set; }

        public int Total {  get; set; }

        public string Status { get; set; } 


    }
}

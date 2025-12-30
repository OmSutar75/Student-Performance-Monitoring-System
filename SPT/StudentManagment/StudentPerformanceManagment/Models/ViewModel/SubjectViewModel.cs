using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class SubjectViewModel
    {
        public int SubjectId { get; set; }

   
        public string SubjectName { get; set; }

        public int MaxTheoryMarks { get; set; } = 40;
        public int MaxLabMarks { get; set; } = 40;
        public int MaxInternalMarks { get; set; } = 20;

        public int PassingPercentTotal { get; set; } = 40; //out of all subject
        public int PassingPercentEachComponent { get; set; } = 40; //for theory,lab,internal

        public int CourseId { get; set; }
        public List<SelectListItem> Courses { get; set; } = new();
    }
}

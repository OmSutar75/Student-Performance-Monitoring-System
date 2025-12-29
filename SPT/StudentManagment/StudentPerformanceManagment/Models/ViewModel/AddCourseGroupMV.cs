using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class AddCourseGroupMV
    {

        
            [Required]
            public string CourseGroupName { get; set; }

            [Required]
            public int CourseId { get; set; }

            public int GroupCount { get; set; }
            public string GroupPrefix {  get; set; }

            public List<SelectListItem> Courses { get; set; }
        
    }
}

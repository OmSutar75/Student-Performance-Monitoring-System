using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceManagement.ViewModel
{
    public class StudentEnrollmentViewModel1
    {
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }

        public string ProfileImagePath { get; set; } = "profile.img";
    }
}


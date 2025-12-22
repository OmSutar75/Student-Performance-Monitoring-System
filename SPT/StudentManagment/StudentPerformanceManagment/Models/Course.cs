
﻿using StudentPerformanceManagement.Models;
using System.ComponentModel.DataAnnotations;


﻿using System.ComponentModel.DataAnnotations;
using StudentPerformanceManagement.Models;

namespace StudentPerformanceManagment.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CourseName { get; set; } = null!;


        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Duration { get; set; }

        public decimal Fees { get; set; }

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<CourseGroup> CourseGroups { get; set; } = new List<CourseGroup>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}

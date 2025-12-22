
﻿using System.ComponentModel.DataAnnotations;
using StudentPerformanceManagment.Models;

namespace StudentPerformanceManagement.Models
{
    public class Staff
    {
        public int StaffId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        public string? Mobile { get; set; }

        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}

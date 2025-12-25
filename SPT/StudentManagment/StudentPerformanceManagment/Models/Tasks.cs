using System.ComponentModel.DataAnnotations.Schema; // Required for [ForeignKey]
using StudentPerformanceManagement.Models;

public enum Status
{
    Pending = 0,
    Completed = 1,
    Overdue = 2
}


namespace StudentPerformanceManagment.Models
{
    public class Tasks
    {
        public int TasksId { get; set; }
        public string TasksTitle { get; set; }
        public string TasksDescription { get; set; }

        public Status Status { get; set; } = Status.Pending;

        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public int CourseGroupId { get; set; }
        public CourseGroup CourseGroup { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public bool IsActive(DateTime now) => now >= ValidFrom && now <= ValidTo ; 
    }
}

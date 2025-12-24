namespace StudentPerformanceManagment.Models
{
    public class LayoutUserViewModel
    {
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";

        // Dashboard Counts
        public int TotalCourses { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalStudents { get; set; }
        public int TotalStaff { get; set; }

        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int CompletedTasks { get; set; }

        // Recent Activity List
        public List<string> RecentActivities { get; set; } = new List<string>();
    }
}



namespace StudentPerformanceManagment.Models.ViewModel
{
    public class StaffDashViewModel
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public int TaskCount { get; set; }

        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int CompletedTasks { get; set; }

        public string AppUserId { get; set; } = "";

        public List<TasksViewModel> Tasks { get; set; } = new();

        // yeh tumhari Task entity ka type hoga (jo _context.Tasks se aata hai)
        /*public List<Tasks> Tasks { get; set; } = new List<Tasks>();*/
    }
}
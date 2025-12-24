namespace StudentPerformanceManagment.Models.ViewModel
{
    public class StaffDashViewModel
    {
        public int TotalTask {get ; set;}
        public int PendingTask { get; set; }
        public int CompletedTasks { get; set; }

        public string StaffName { get; set;}
        public string StaffId { get; set;}
        public List<Tasks> Tasks { get; set;}


    }
}

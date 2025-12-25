namespace StudentPerformanceManagment.Models.ViewModel
{
    public class StaffDashViewModel
    {
        public int TaskCount {get ; set;}

        public string StaffEmail { get; set; }
        public string StaffName { get; set;}
        public int StaffId { get; set;}
        public List<Tasks> Tasks { get; set;}


    }
}

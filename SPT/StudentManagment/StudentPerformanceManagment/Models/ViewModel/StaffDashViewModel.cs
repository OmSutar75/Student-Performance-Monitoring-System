

namespace StudentPerformanceManagment.Models.ViewModel
{
    public class StaffDashViewModel
    {
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public int TaskCount { get; set; }

        // yeh tumhari Task entity ka type hoga (jo _context.Tasks se aata hai)
        public List<Tasks> Tasks { get; set; } = new List<Tasks>();
    }
}
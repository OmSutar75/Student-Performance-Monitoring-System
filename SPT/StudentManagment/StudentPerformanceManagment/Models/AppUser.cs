using Microsoft.AspNetCore.Identity;

namespace StudentPerformanceManagment.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
       
    }
}

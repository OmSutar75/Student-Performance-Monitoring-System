using Microsoft.AspNetCore.Mvc;

namespace StudentPerformanceManagment.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}

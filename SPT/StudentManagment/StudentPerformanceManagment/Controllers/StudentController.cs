using Microsoft.AspNetCore.Mvc;

namespace StudentPerformanceManagment.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult EditProfile()
        {
            return View();
        }
    }
}

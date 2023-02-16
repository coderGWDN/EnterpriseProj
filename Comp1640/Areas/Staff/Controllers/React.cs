using Microsoft.AspNetCore.Mvc;

namespace Comp1640.Areas.Staff.Controllers
{
    public class React : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class PurchaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

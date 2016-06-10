using System.Web.Mvc;

namespace hypster_admin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session.Add("LoggedInRole", "User");
            return View();
        }
    }
}
using System.Web.Mvc;

namespace hypster_admin.Areas.VideoManagement.Controllers
{
    [Authorize]
    public class homeVideoController : Controller
    {
        //
        // GET: /VideoManagement/homeVideo/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }
    }
}

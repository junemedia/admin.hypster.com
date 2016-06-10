using System.Web.Mvc;

namespace hypster_admin.Areas.VideoManagement.Controllers
{
    [Authorize]
    public class pendingVideoController : Controller
    {
        //
        // GET: /VideoManagement/pendingVideo/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }
    }
}

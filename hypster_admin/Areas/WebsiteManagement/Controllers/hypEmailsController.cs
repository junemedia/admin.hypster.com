using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class hypEmailsController : Controller
    {
        //
        // GET: /WebsiteManagement/hypEmails/

        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult SendConfEmail(string email)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.Email_Manager emailManager = new hypster_tv_DAL.Email_Manager();
                emailManager.SendWelcomeEmail("Welcome to Hypster", email);
                return RedirectPermanent("/WebsiteManagement/hypEmails");
            }
            else
                return RedirectPermanent("/home/");
        }
    }
}

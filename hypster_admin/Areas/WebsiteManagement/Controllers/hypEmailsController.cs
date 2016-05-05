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
            return View();
        }


        [HttpPost]
        public ActionResult SendConfEmail(string email)
        {
            hypster_tv_DAL.Email_Manager emailManager = new hypster_tv_DAL.Email_Manager();


            emailManager.SendWelcomeEmail("Welcome to Hypster", email);


            return RedirectPermanent("/WebsiteManagement/hypEmails");
        }





    }
}

using System.Collections.Generic;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class contactUsController : Controller
    {
        //
        // GET: /WebsiteManagement/contactUs/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                List<hypster_tv_DAL.userContact> contList = new List<hypster_tv_DAL.userContact>();
                hypster_tv_DAL.userContactManagement userConManager = new hypster_tv_DAL.userContactManagement();
                contList = userConManager.ActiveUserContactUs();
                return View(contList);
            }
            else
                return RedirectPermanent("/home/");
        }


        public ActionResult editContactUs(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.userContact cont = new hypster_tv_DAL.userContact();
                hypster_tv_DAL.userContactManagement userConManager = new hypster_tv_DAL.userContactManagement();
                cont = userConManager.Get_UserContactUs_byID(id);
                return View(cont);
            }
            else
                return RedirectPermanent("/home/");
        }


        //
        // RIGHT NOW LOGIC JUST DEACTIVATING USER CONTACT
        [HttpPost]
        public ActionResult editContactUs(int CONT_ID, hypster_tv_DAL.userContact cont)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.userContactManagement userConManager = new hypster_tv_DAL.userContactManagement();
                userConManager.Deactivate_UserContactUs(CONT_ID);
                return RedirectToAction("");
            }
            else
                return RedirectPermanent("/home/");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            List<hypster_tv_DAL.userContact> contList = new List<hypster_tv_DAL.userContact>();

            hypster_tv_DAL.userContactManagement userConManager = new hypster_tv_DAL.userContactManagement();
            contList = userConManager.ActiveUserContactUs();

            return View(contList);
        }








        public ActionResult editContactUs(int id)
        {
            hypster_tv_DAL.userContact cont = new hypster_tv_DAL.userContact();

            hypster_tv_DAL.userContactManagement userConManager = new hypster_tv_DAL.userContactManagement();
            cont = userConManager.Get_UserContactUs_byID(id);


            return View(cont);
        }



        //
        // RIGHT NOW LOGIC JUST DEACTIVATING USER CONTACT
        [HttpPost]
        public ActionResult editContactUs(int CONT_ID, hypster_tv_DAL.userContact cont)
        {
            hypster_tv_DAL.userContactManagement userConManager = new hypster_tv_DAL.userContactManagement();
            userConManager.Deactivate_UserContactUs(CONT_ID);

            return RedirectToAction("");
        }



    }
}

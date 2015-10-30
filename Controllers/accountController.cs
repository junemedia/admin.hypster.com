using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Controllers
{
    public class accountController : Controller
    {
        //
        // GET: /account/

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(hypster_tv_DAL.Member p_member)
        {
            if (ModelState.IsValid)
            {
                if (p_member.username == System.Configuration.ConfigurationManager.AppSettings["hypAdmin_UserName"] && p_member.password == System.Configuration.ConfigurationManager.AppSettings["hypAdmin_Pass"])
                {
                    System.Web.Security.FormsAuthentication.SetAuthCookie(System.Configuration.ConfigurationManager.AppSettings["hypAdmin_UserName"], false);
                    return RedirectToAction("Index", "home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username/password");
                }
            }

            return View();
        }




        //
        // GET: /Account/LogOff
        //----------------------------------------------------------------------------------------------------------
        public ActionResult LogOff()
        {
            System.Web.Security.FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
        //----------------------------------------------------------------------------------------------------------



    }
}

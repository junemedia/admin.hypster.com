using System;
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
                hypster_tv_DAL.memberManagement membersManager = new hypster_tv_DAL.memberManagement();
                if (membersManager.ValidateUser(p_member.username, p_member.password))
                {
                    string IP_Address;
                    IP_Address = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (IP_Address == null)
                        IP_Address = Request.ServerVariables["REMOTE_ADDR"];
                    else
                        IP_Address = "";

                    //save to database user activity data
                    hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();
                    membersManager.CleanMemberFromCache(p_member.username);
                    member = membersManager.getMemberByUserName(p_member.username);
                    membersManager.UpdateMemberActivityData(User.Identity.Name, member.id, DateTime.Now, IP_Address);

                    //track user logins
                    hypster_tv_DAL.TrackLoginManagement trackLoginManager = new hypster_tv_DAL.TrackLoginManagement();

                    hypster_tv_DAL.TrackLogin trkLogin = new hypster_tv_DAL.TrackLogin();
                    trkLogin.User_id = member.id;
                    trkLogin.Login_Date = DateTime.Now;
                    trackLoginManager.hyDB.TrackLogins.AddObject(trkLogin);
                    trackLoginManager.hyDB.SaveChanges();

                    //----------------------------------------------------------------------------------------------
                    //this code is updating email tracker (some another tracker can be implemented)
                    //
                    if (HttpContext.Request.Cookies.AllKeys.Contains("ETT") || member.ArtistLevel > 0)
                    {
                        membersManager.UpdateMemberTrackData(User.Identity.Name, member.id);

                        if (HttpContext.Request.Cookies["ETT"] != null)
                        {
                            HttpCookie cookie = HttpContext.Request.Cookies["ETT"];
                            cookie.Expires = DateTime.Now.AddDays(-4);
                            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                        }
                    }
                    //----------------------------------------------------------------------------------------------

                    bool isActive_check = true;
                    isActive_check = membersManager.isActiveCheck(member.id);
                    if (isActive_check == true)
                    {
                        if (p_member.username == member.username && p_member.password == member.password && member.adminLevel > 0)
                        {
                            System.Web.Security.FormsAuthentication.SetAuthCookie(p_member.username, false);
                            if (member.adminLevel > 1)
                                Session.Add("Roles", "Admin");
                            else
                                Session.Add("Roles", "Editor");
                            return RedirectToAction("Index", "Home");
                        }
                        else
                            return Redirect("http://www.hypster.com/");
                    }
                    else
                    {
                        ViewBag.ActivateAccount = true;
                        ModelState.AddModelError("", "User is deactivated.");
                    }
                }
                else
                {
                    ViewBag.ForgotPassword = true;
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            else
                ModelState.AddModelError("", "Please enter user name AND password to login.");

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
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.Mvc;

namespace hypster_admin.Areas.Administrators.Controllers
{
    [Authorize]
    public class manageUsersController : Controller
    {
        //
        // GET: /WebsiteManagement/manageUsers/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult Index(string SearchFor, string serUserPar)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.memberManagement membersManager = new hypster_tv_DAL.memberManagement();
                List<hypster_tv_DAL.Member> members_arr = new List<hypster_tv_DAL.Member>();
                switch (SearchFor)
                {
                    case "serUserName":
                        members_arr.Add(membersManager.getMemberByUserName(serUserPar));
                        break;
                    case "serUserEmail":
                        members_arr.Add(membersManager.getMemberByEmail(serUserPar));
                        break;
                    case "serUserID":
                        int iUserID = -1;
                        Int32.TryParse(serUserPar, out iUserID);
                        if(iUserID != -1)
                            members_arr.Add(membersManager.getMemberByID(iUserID));
                        break;
                    default:
                        break;
                }
                return View(members_arr);
            }
            else
                return RedirectPermanent("/home/");
        }

        [HttpGet]
        public ActionResult AddNewUser()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();
                return View(member);
            }
            else
                return RedirectPermanent("/home/");            
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddNewUser(hypster_tv_DAL.Member member)//, int DOB_YYYY, int DOB_MM, int DOB_DD
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.Member memberAdd = new hypster_tv_DAL.Member();
                hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();
                hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();

                memberAdd.username = member.username;
                memberAdd.password = member.password;
                memberAdd.name = member.name;
                memberAdd.email = member.email;
                memberAdd.adminLevel = member.adminLevel;
                //memberAdd.country = member.country;
                //memberAdd.city = member.city;
                //memberAdd.zipcode = member.zipcode;
                //memberAdd.birth = new DateTime(DOB_YYYY, DOB_MM, DOB_DD);
                //memberAdd.sex = Convert.ToByte(member.sex);
                string IP_Address;
                IP_Address = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (IP_Address == null)
                    IP_Address = Request.ServerVariables["REMOTE_ADDR"];
                else
                    IP_Address = "";
                memberAdd.RegistrationIp = IP_Address;
            
                memberAdd.regdate = DateTime.Now;
                if (memberAdd.username != "" && memberAdd.password != "" && memberAdd.name != "" && memberAdd.email != "")
                {
                    bool usernameClean = (memberManager.getMemberByUserName(memberAdd.username).id == 0);
                    bool emailClean = (memberManager.getMemberByEmail(memberAdd.email).id == 0);
                    if (!usernameClean)
                    {
                        ModelState.AddModelError("", "The Username has already been used, please select another one!!");
                        return View(memberAdd);
                    }
                    if (!emailClean)
                    {
                        ModelState.AddModelError("", "The Email Address has already been registered, please choose another email address!!");
                        return View(memberAdd);
                    }
                
                    hyDB.Members.AddObject(memberAdd);
                    hyDB.SaveChanges();
                
                    return RedirectToAction("Index", "manageUsers");
                }
                else
                {
                    ModelState.AddModelError("", "Please Fill Up the Username, Password, Name, and Email for the New User!!");
                    return View(memberAdd);
                }
            }
            else
                return RedirectPermanent("/home/");
        }

        public ActionResult user(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.Member memberEdit = new hypster_tv_DAL.Member();
                hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
                memberEdit = memberManager.getMemberByID(id);
                return PartialView("user", memberEdit);
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult user(int MEM_ID, hypster_tv_DAL.Member member)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.Member memberEdit = new hypster_tv_DAL.Member();
                hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
                memberEdit = memberManager.getMemberByID(MEM_ID);
                memberEdit.id = MEM_ID;
                memberEdit.username = member.username;
                memberEdit.password = member.password.ToString();
                memberEdit.name = member.name;
                memberEdit.email = member.email;
                memberEdit.adminLevel = member.adminLevel;
                memberManager.UpdateMemberUsername(memberEdit.username, memberEdit.id);
                memberManager.UpdateMemberPassword(memberEdit.username, memberEdit.id, memberEdit.password);
                memberManager.UpdateMemberProfileDetailsNameEmailAdminLevel(memberEdit.username, MEM_ID, memberEdit.name, memberEdit.email, memberEdit.adminLevel);
                return RedirectToAction("/");
            }
            else
                return RedirectPermanent("/home/");
        }
        

        public ActionResult manageUsernames()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }

        
        [HttpPost]
        public ActionResult manageUsernames(string txtUsernames)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                string str_to_parse = txtUsernames.Replace("\r\n", ",");
                try
                {
                    //strip usernames
                    string[] usernames_arr = str_to_parse.Split(',');
                    hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
                    hypster_tv_DAL.Email_Manager email_manager = new hypster_tv_DAL.Email_Manager();
                    foreach (string username in usernames_arr)
                    {
                        if (username != "")
                        {
                            hypster_tv_DAL.Member curr_user = new hypster_tv_DAL.Member();
                            curr_user = memberManager.getMemberByUserName(username);
                            //try to replace username 
                            string new_user_name = curr_user.username;
                            new_user_name = new_user_name.Replace("&", "-");
                            new_user_name = new_user_name.Replace("?", "-");
                            new_user_name = new_user_name.Replace("%", "-");
                            new_user_name = new_user_name.Replace("%", "-");
                            new_user_name = new_user_name.Replace("*", "-");
                            new_user_name = new_user_name.Replace("\'", "-");
                            new_user_name = new_user_name.Replace("#", "-");
                            new_user_name = new_user_name.Replace("<", "-");
                            new_user_name = new_user_name.Replace(">", "-");
                            hypster_tv_DAL.Member check_user = new hypster_tv_DAL.Member();
                            check_user = memberManager.getMemberByUserName(new_user_name);
                            if (check_user.id == 0)
                            {
                                curr_user.username = new_user_name;
                                memberManager.UpdateMemberUsername(curr_user.username, curr_user.id);

                                email_manager.SendUsernameChangesNotification(curr_user.username, curr_user.email);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string str_pr = ex.Message.ToString();
                }
                return View();
            }
            else
                return RedirectPermanent("/home/");
        }
    }
}

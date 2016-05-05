using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class manageUsersController : Controller
    {
        //
        // GET: /WebsiteManagement/manageUsers/

        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Index(string SearchFor, string serUserPar)
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





        public ActionResult user(int id)
        {
            hypster_tv_DAL.Member memberEdit = new hypster_tv_DAL.Member();
            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            memberEdit = memberManager.getMemberByID(id);

            return View(memberEdit);
        }


        [HttpPost]
        public ActionResult user(int MEM_ID, hypster_tv_DAL.Member member)
        {
            hypster_tv_DAL.Member memberEdit = new hypster_tv_DAL.Member();
            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            memberEdit = memberManager.getMemberByID(MEM_ID);

            if(member.password != "")
                memberEdit.password = member.password.ToString();

            memberManager.UpdateMemberPassword(User.Identity.Name, memberEdit.id, memberEdit.password);


            return RedirectToAction("/user/" + MEM_ID);
        }




        public ActionResult manageUsernames()
        {
            return View();
        }



        [HttpPost]
        public ActionResult manageUsernames(string txtUsernames)
        {
            string str_to_parse = txtUsernames.Replace("\r\n",",");

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



    }
}

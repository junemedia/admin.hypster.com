using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class manageFestivalsController : Controller
    {
        //
        // GET: /WebsiteManagement/manageFestivals/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.FestivalManager festivalManager = new hypster_tv_DAL.FestivalManager();
                List<hypster_tv_DAL.Festival> festival_list = new List<hypster_tv_DAL.Festival>();
                festival_list = festivalManager.GetAllFestivals();
                return View(festival_list);
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult AddNewFestival(string FestivalName, string FestivalDesc, string FestivalDate, int UserID, int PlaylistID)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.FestivalManager festivalManager = new hypster_tv_DAL.FestivalManager();
                hypster_tv_DAL.Festival festival_add = new hypster_tv_DAL.Festival();
                festival_add.Festival_Name = FestivalName;
                festival_add.Festival_Desc = FestivalDesc;
                festival_add.Festival_Date = FestivalDate;
                festival_add.Festival_User_ID = UserID;
                festival_add.Festival_Playlist_ID = PlaylistID;
                festival_add.Festival_GUID = festival_add.Festival_Name.Replace("/", "").Replace("\\", "").Replace("&", "").Replace("+", "").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace("*", "").Replace("$", "").Replace("\"", "").Replace("'", "").Replace("{", "").Replace("}", "").Replace(")", "").Replace("(", "").Replace("[", "").Replace("]", "").Replace("|", "").Replace(".", "").Replace(",", "").Replace(":", "").Replace(";", "");
                hypster_tv_DAL.Festival check_festival = new hypster_tv_DAL.Festival();
                check_festival = festivalManager.GetFestivalByGuid(festival_add.Festival_GUID);
                if (check_festival.Festival_ID != 0)
                {
                    Random rand = new Random();
                    festival_add.Festival_GUID += "_" + rand.Next(1, 200000).ToString();
                }
                festivalManager.AddNewFestivals(festival_add);
                return RedirectPermanent("/WebsiteManagement/manageFestivals");
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult SaveFestival(int Festival_ID, int Festival_Category_ID, string Festival_Name, string Festival_Desc, string Festival_Date, int Festival_User_ID, int Festival_Playlist_ID)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.FestivalManager festivalManager = new hypster_tv_DAL.FestivalManager();
                hypster_tv_DAL.Festival festival_save = new hypster_tv_DAL.Festival();
                festival_save.Festival_ID = Festival_ID;
                festival_save.Festival_Category_ID = Festival_Category_ID;
                festival_save.Festival_Name = Festival_Name;
                festival_save.Festival_Desc = Festival_Desc;
                festival_save.Festival_Date = Festival_Date;
                festival_save.Festival_User_ID = Festival_User_ID;
                festival_save.Festival_Playlist_ID = Festival_Playlist_ID;
                festivalManager.SaveFestival(festival_save);
                return RedirectPermanent("/WebsiteManagement/manageFestivals");
            }
            else
                return RedirectPermanent("/home/");
        }



        public ActionResult DeleteFestival(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.FestivalManager festivalManager = new hypster_tv_DAL.FestivalManager();
                festivalManager.DeleteFestival(id);
                return RedirectPermanent("/WebsiteManagement/manageFestivals");
            }
            else
                return RedirectPermanent("/home/");
        }
    }
}

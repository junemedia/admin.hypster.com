using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class hypFeaturedPlaylistsController : Controller
    {
        //
        // GET: /WebsiteManagement/hypFeaturedPlaylists/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                List<hypster_tv_DAL.FeaturedPlaylist_Result> plsts_list = new List<hypster_tv_DAL.FeaturedPlaylist_Result>();
                hypster_tv_DAL.FeaturedPlaylistManagement fp_manager = new hypster_tv_DAL.FeaturedPlaylistManagement();
                plsts_list = fp_manager.ReturnFeaturedPlaylists();
                return View(plsts_list);
            }
            else
                return RedirectPermanent("/home");
        }


        [HttpPost]
        public ActionResult AddNewFeaturedPlaylist(string fp_PlaylistName, int fp_PlaylistID, int fp_UserID)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.FeaturedPlaylist plst_add = new hypster_tv_DAL.FeaturedPlaylist();
                plst_add.FeaturedPlaylist_PlaylistID = fp_PlaylistID;
                plst_add.FeaturedPlaylist_UserID = fp_UserID;
                plst_add.FeaturedPlaylist_PlaylistName = fp_PlaylistName;
                plst_add.FeaturedPlaylist_CreateDate = DateTime.Now;
                plst_add.FeaturedPlaylist_Guid = fp_PlaylistName.Replace('&', '-').Replace('?', '-').Replace('*', '-').Replace("\'", "-").Replace('#', '-').Replace(' ', '-');
                hypster_tv_DAL.FeaturedPlaylistManagement fp_manager = new hypster_tv_DAL.FeaturedPlaylistManagement();
                hypster_tv_DAL.FeaturedPlaylist_Result fp_check = new hypster_tv_DAL.FeaturedPlaylist_Result();
                fp_check = fp_manager.FeaturedPlaylistByGuid(plst_add.FeaturedPlaylist_Guid);
                if (fp_check.FeaturedPlaylist_ID != 0)
                {
                    Random rand = new Random();
                    plst_add.FeaturedPlaylist_Guid = plst_add.FeaturedPlaylist_Guid + "-" + rand.Next(1, 10000).ToString();
                }            
                fp_manager.hyDB.FeaturedPlaylists.AddObject(plst_add);
                fp_manager.hyDB.SaveChanges();
                return RedirectPermanent("/WebsiteManagement/hypFeaturedPlaylists");
            }
            else
                return RedirectPermanent("/home");
        }
        

        //delete playlist
        //
        public ActionResult DeleteFeaturedPlaylist()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                int FPLST = 0;
                if (Request.QueryString["FPLST"] != null)
                {
                    Int32.TryParse(Request.QueryString["FPLST"], out FPLST);
                }
                if (FPLST > 0)
                {
                    hypster_tv_DAL.FeaturedPlaylistManagement fp_manager = new hypster_tv_DAL.FeaturedPlaylistManagement();
                    fp_manager.DeleteFeaturedPlaylists(FPLST);
                }
                return RedirectPermanent("/WebsiteManagement/hypFeaturedPlaylists");
            }
            else
                return RedirectPermanent("/home");
        }
    }
}

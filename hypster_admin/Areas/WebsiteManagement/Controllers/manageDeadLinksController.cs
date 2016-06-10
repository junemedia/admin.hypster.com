using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    public class manageDeadLinksController : Controller
    {
        //
        // GET: /WebsiteManagement/manageDeadLinks/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }


        public ActionResult DetectDeadLinks_List(string id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                string user_name = id;
                hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
                hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
                hypster_tv_DAL.Member curr_user = new hypster_tv_DAL.Member();
                curr_user = memberManager.getMemberByUserName(user_name);
                List<hypster_tv_DAL.Playlist> playlists_list = new List<hypster_tv_DAL.Playlist>();
                playlists_list = playlistManager.GetUserPlaylists(curr_user.id);
                return View(playlists_list);
            }
            else
                return RedirectPermanent("/home/");
        }


        public ActionResult GetPlaylistDetails_View(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                int plst_id = id;
                ViewBag.plst_id = plst_id;
                hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
                hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
                List<hypster_tv_DAL.PlaylistData_Song> songs_list = new List<hypster_tv_DAL.PlaylistData_Song>();
                songs_list = playlistManager.GetPlayListDataByPlaylistID(plst_id);
                //get playlist details
                return View(songs_list);
            }
            else
                return RedirectPermanent("/home/");
        }


        public ActionResult GetPlaylistDetails_Check(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                int plst_id = id;
                ViewBag.plst_id = plst_id;
                hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
                hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
                List<hypster_tv_DAL.PlaylistData_Song> songs_list = new List<hypster_tv_DAL.PlaylistData_Song>();
                songs_list = playlistManager.GetPlayListDataByPlaylistID(plst_id);
                //get playlist details
                return View(songs_list);
            }
            else
                return RedirectPermanent("/home/");
        }


        public string SubmitDeadLink(int id)
        {
            int song_id = id;
            hypster_tv_DAL.songsManagement songManager = new hypster_tv_DAL.songsManagement();
            songManager.MarkDeadLink(song_id);
            return "";
        }


        public string DeleteDealLink(int id)
        {
            int song_id = id;
            hypster_tv_DAL.memberManagement member_manager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.Member curr_member = new hypster_tv_DAL.Member();
            string user_name = "";
            if (Request.QueryString["user_name"] != null)
            {
                user_name = Request.QueryString["user_name"].ToString();
                curr_member = member_manager.getMemberByUserName(user_name);
            }
            else
            {
                return "wrong/missing user_name";
            }
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            playlistManager.DeleteSong(curr_member.id, song_id);
            return "ok";
        }
    }
}

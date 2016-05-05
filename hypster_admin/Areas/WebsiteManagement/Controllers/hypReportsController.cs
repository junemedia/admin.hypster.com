using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class hypReportsController : Controller
    {
        //
        // GET: /WebsiteManagement/hypReports/

        [OutputCache(Duration=10)]
        public ActionResult Index()
        {

            //SELECT COUNT(UsFol_ID) AS Followers FROM Followers
            //SELECT COUNT(id) AS Members FROM Members
            //SELECT COUNT(MemberMusicGenre_ID) AS MemberMusicGenre FROM MemberMusicGenre
            //SELECT COUNT(id) AS Photos FROM Photos
            //SELECT COUNT(PlaylistLikesId) AS PlaylistLikes FROM PlaylistLikes
            //SELECT COUNT(id) AS Playlists FROM Playlists
            //SELECT COUNT(RadioStation_ID) AS RadioStation FROM RadioStation


            hypster_tv_DAL.hypReports hypReportsManager = new hypster_tv_DAL.hypReports();

            ViewBag.Followers_NUM = hypReportsManager.Followers_NUM();
            ViewBag.Members_NUM = hypReportsManager.Members_NUM();
            ViewBag.MemberMusicGenre_NUM = hypReportsManager.MemberMusicGenre_NUM();
            ViewBag.Photos_NUM = hypReportsManager.Photos_NUM();
            ViewBag.PlaylistLikes_NUM = hypReportsManager.PlaylistLikes_NUM();
            ViewBag.Playlists_NUM = hypReportsManager.Playlists_NUM();
            ViewBag.RadioStation_NUM = hypReportsManager.RadioStation_NUM();
            //ViewBag.Songs_NUM = hypReportsManager.Songs_NUM();
            ViewBag.Last_Song_ID = hypReportsManager.Last_Song_ID();

            return View();
        }







        public ActionResult newRegisteredMembers()
        {


            return View();
        }


        [HttpPost]
        public ActionResult newRegisteredMembers(string Date_text)
        {
            DateTime dt_now = DateTime.Parse(Date_text);


            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();

            List<hypster_tv_DAL.Member> members_list = new List<hypster_tv_DAL.Member>();
            members_list = memberManager.GetNewMembersRegistrations(dt_now);


            ViewBag.newRegisteredMembersDATE = Date_text;
            ViewBag.newRegisteredMembers = members_list.Count;


            return View();
        }





        public ActionResult SongsPlayReport()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SongsPlayReport(string Date_text)
        {

            DateTime dt_now = DateTime.Parse(Date_text);


            hypster_tv_DAL.hypReports hypReportsManager = new hypster_tv_DAL.hypReports();


            ViewBag.newRegisteredMembersDATE = Date_text;

            ViewBag.SongsStarted = hypReportsManager.SongsStarted_NUM(dt_now);
            ViewBag.SongsPlayed = hypReportsManager.SongsPlayed_NUM(dt_now);
            ViewBag.SongsDesktopPlayer = hypReportsManager.SongsDesktopPlayer_NUM(dt_now);
            ViewBag.SongsAppsMob = hypReportsManager.SongsAppsMob_NUM(dt_now);
            ViewBag.SongsBreakingPlayer = hypReportsManager.SongsBreakingPlayer_NUM(dt_now);

            
            return View();
        }



    }
}

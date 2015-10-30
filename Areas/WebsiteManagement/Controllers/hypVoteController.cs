using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class hypVoteController : Controller
    {
        //
        // GET: /WebsiteManagement/hypVote/

        public ActionResult Index()
        {
            hypster_tv_DAL.voteManagement voteManager = new hypster_tv_DAL.voteManagement();

            hypster_tv_DAL.VoteForSong vfs = new hypster_tv_DAL.VoteForSong();
            vfs = voteManager.Get_Active_VoteForSong();

            return View(vfs);
        }


        [HttpPost]
        public ActionResult AddNewVoteSongs(string song1_ID, string song1_Title, string song2_ID, string song2_Title)
        {
            hypster_tv_DAL.voteManagement voteManager = new hypster_tv_DAL.voteManagement();


            hypster_tv_DAL.VoteForSong vfs = new hypster_tv_DAL.VoteForSong();
            vfs.Song1_ID = song1_ID;
            vfs.Song1_Title = song1_Title;
            vfs.Song2_ID = song2_ID;
            vfs.Song2_Title = song2_Title;
            voteManager.AddNew_VoteForSong(vfs);


            return RedirectPermanent("/WebsiteManagement/hypVote");
        }


    }
}

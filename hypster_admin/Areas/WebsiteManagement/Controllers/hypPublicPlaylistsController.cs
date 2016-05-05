using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class hypPublicPlaylistsController : Controller
    {
        //
        // GET: /WebsiteManagement/hypPublicPlaylists/

        public ActionResult Index()
        {
            return View();
        }




        public ActionResult Edit(int id)
        {
            int playlist_id = id;

            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();


            hypster_tv_DAL.Playlist curr_plst = new hypster_tv_DAL.Playlist();
            curr_plst = playlistManager.GetPlaylistById_Admin(playlist_id);



            return View(curr_plst);
        }





        [HttpPost]
        public ActionResult Edit(string plst_desc, int plst_id)
        {
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();

            if (plst_desc.Length < 100)
            {
                int length = 100 - plst_desc.Length;
                plst_desc = plst_desc.PadRight(100, ' ');
            }

            playlistManager.UpdatePlaylistDesc(plst_id, plst_desc);

            return RedirectPermanent("/WebsiteManagement/hypPublicPlaylists");
        }





    }
}

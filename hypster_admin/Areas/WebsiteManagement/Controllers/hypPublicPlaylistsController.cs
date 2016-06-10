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
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }

        [HttpPost]
        public ActionResult Edit(string plst_desc, int plst_id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
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
            else
                return RedirectPermanent("/home");
        }
    }
}
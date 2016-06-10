using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{

    [Authorize]
    public class hypArtistsController : Controller
    {
        //
        // GET: /WebsiteManagement/hypArtists/



        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.MemberMusicGenreManager genreManager = new hypster_tv_DAL.MemberMusicGenreManager();
                hypster_tv_DAL.visualSearchManager visSearchManager = new hypster_tv_DAL.visualSearchManager();
                hypster_admin.Areas.WebsiteManagement.ViewModels.hypArtistsViewModel model = new ViewModels.hypArtistsViewModel();
                model.genres = genreManager.GetMusicGenresList();
                model.visualSearch = visSearchManager.getVisualSearchArtists();
                return View(model);
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult AddNewArtist(HttpPostedFileBase file, string name, int genre_id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.visualSearchManager visualSearchManager = new hypster_tv_DAL.visualSearchManager();
                if (visualSearchManager.getVisualSearchArtistByName(name).VisualSearch_ID == 0) //if not exist yet - then add new artist
                {
                    string savePath = System.Configuration.ConfigurationManager.AppSettings["visualSearchStorage_Path"] + "\\" + name + ".jpg";
                    file.SaveAs(savePath);
                    hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
                    image_resizer.Resize_Image(savePath, -1, 120, System.Drawing.Imaging.ImageFormat.Jpeg);
                    int ImWidth = 0;
                    System.Drawing.Image img = System.Drawing.Image.FromFile(savePath);
                    ImWidth = img.Width;
                    hypster_tv_DAL.VisualSearch visSearch = new hypster_tv_DAL.VisualSearch();
                    visSearch.Artist_Name = name;
                    visSearch.Genre_ID = genre_id;
                    visSearch.ImWidth = ImWidth;
                    visualSearchManager.addVisualSearch(visSearch);
                    return RedirectPermanent("/WebsiteManagement/hypArtists");
                }
                hypster_admin.Areas.WebsiteManagement.ViewModels.hypArtistsViewModel model = new ViewModels.hypArtistsViewModel();
                ViewBag.Value = "Already Exist";
                return View("Index", model);
            }
            else
                return RedirectPermanent("/home/");
        }


        public string GetGenreArtists()
        {
            string ret_res = "";
            int GENRE_ID = 0;
            if (Request.QueryString["ID"] != null)
            {
                Int32.TryParse(Request.QueryString["ID"], out GENRE_ID);
            }
            hypster_tv_DAL.visualSearchManager visualSearchManager = new hypster_tv_DAL.visualSearchManager();
            List<hypster_tv_DAL.VisualSearch> model = new List<hypster_tv_DAL.VisualSearch>();
            model = visualSearchManager.getVisualSearchArtistsByGenreID(GENRE_ID);
            foreach (var item in model)
            {
                ret_res += "<img alt='" + item.Artist_Name + "' src='http://" + System.Configuration.ConfigurationManager.AppSettings["hypsterHostName"] + "/imgs/visualSearch/" + item.Artist_Name + ".jpg' style='float:left; height:120px; margin:5px;' onclick=\"if(confirm('Are you sure you want to delete?')==true){window.location='/WebsiteManagement/hypArtists/DeleteArtist/" + item.VisualSearch_ID + "';}\" />";
            }
            return ret_res;
        }


        public ActionResult DeleteArtist(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.visualSearchManager visualSearchManager = new hypster_tv_DAL.visualSearchManager();
                hypster_tv_DAL.VisualSearch artist = new hypster_tv_DAL.VisualSearch();
                artist = visualSearchManager.getVisualSearchArtistByID(id);
                visualSearchManager.DeleteArtist(id);
                System.IO.FileInfo file = new System.IO.FileInfo(System.Configuration.ConfigurationManager.AppSettings["visualSearchStorage_Path"] + "\\" + artist.Artist_Name + ".jpg");
                file.Delete();
                return RedirectPermanent("/WebsiteManagement/hypArtists");
            }
            else
                return RedirectPermanent("/home/");
        }


        public ActionResult Confirm_Artist_Names()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.artistManagement artistManager = new hypster_tv_DAL.artistManagement();
                List<hypster_tv_DAL.ArtistCategory> artist_list = new List<hypster_tv_DAL.ArtistCategory>();
                artist_list = artistManager.GetTopNotConfirmedArtists();
                return View(artist_list);
            }
            else
                return RedirectPermanent("/home/");
        }



        public string ConfArtist()
        {
            int artist_id = 0;
            if (Request.QueryString["AR_ID"] != null)
            {
                Int32.TryParse(Request.QueryString["AR_ID"], out artist_id);
                hypster_tv_DAL.artistManagement artistManager = new hypster_tv_DAL.artistManagement();
                artistManager.ConfirmArtist(artist_id);
            }
            return "OK";
        }
    }
}

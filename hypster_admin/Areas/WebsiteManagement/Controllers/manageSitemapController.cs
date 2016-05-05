using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class manageSitemapController : Controller
    {
        //
        // GET: /WebsiteManagement/manageSitemap/

        public ActionResult Index()
        {
            return View();
        }




        public ActionResult Generate(string id)
        {
            return View();
        }




        public ActionResult Generate_Hypster_Website_EN()
        {
            return View();
        }

        public ActionResult Generate_Hypster_Website_RU()
        {
            return View();
        }

        public ActionResult Generate_Hypster_Website_ES()
        {
            return View();
        }





        public ActionResult Generate_Hypster_News()
        {
            string hyp_sitemap = "";

            hypster_tv_DAL.newsManagement newsManagement = new hypster_tv_DAL.newsManagement();

            List<hypster_tv_DAL.newsPost> news_posts = new List<hypster_tv_DAL.newsPost>();
            news_posts = newsManagement.GetAllNews();

            foreach (var item in news_posts)
            {
                string item_str = "";

                item_str += "<url><loc>http://hypster.com/breaking/details/" + item.post_guid + "</loc><changefreq>weekly</changefreq><priority>1.00</priority></url>" + System.Environment.NewLine;

                hyp_sitemap += item_str;
            }

            ViewBag.SiteMap_STR = hyp_sitemap;

            return View();
        }





        public ActionResult Generate_Hypster_Manuals()
        {
            string hyp_sitemap = "";

            hypster_tv_DAL.manualManagement manualManagement = new hypster_tv_DAL.manualManagement();

            List<hypster_tv_DAL.Manual> manuals_list = new List<hypster_tv_DAL.Manual>();
            manuals_list = manualManagement.GetActiveManuals();


            foreach (var item in manuals_list)
            {
                string item_str = "";

                item_str += "<url><loc>http://hypster.com/resources/manuals/details/" + item.Manual_Guid + "</loc><changefreq>weekly</changefreq><priority>1.00</priority></url>" + System.Environment.NewLine;

                hyp_sitemap += item_str;
            }

            ViewBag.SiteMap_STR = hyp_sitemap;

            return View();
        }







        public ActionResult Generate_Hypster_Charts()
        {
            hypster_tv_DAL.chartsManager chartManagement = new hypster_tv_DAL.chartsManager();

            List<hypster_tv_DAL.Chart> charts_list = new List<hypster_tv_DAL.Chart>();
            charts_list = chartManagement.GetAllCharts();

            return View(charts_list);
        }




        public ActionResult Generate_Hypster_Festivals()
        {
            hypster_tv_DAL.FestivalManager festivalManagement = new hypster_tv_DAL.FestivalManager();

            List<hypster_tv_DAL.Festival> festivals_list = new List<hypster_tv_DAL.Festival>();
            festivals_list = festivalManagement.GetAllFestivals();

            return View(festivals_list);
        }




        public ActionResult Generate_Hypster_MostLiked_Playlists()
        {
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();

            List<hypster_tv_DAL.Playlist> model = new List<hypster_tv_DAL.Playlist>();
            
            model = playlistManager.GetMostLikedPlaylistsAdmin();
            
            return View(model);
        }




        public ActionResult Generate_Hypster_MostViewed_Playlists()
        {
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();

            List<hypster_tv_DAL.Playlist> model = new List<hypster_tv_DAL.Playlist>();

            model = playlistManager.GetMostViewedPlaylistsAdmin();
            
            return View(model);
        }



        public ActionResult Generate_Hypster_WithDesc_Playlists()
        {
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();

            List<hypster_tv_DAL.Playlist> model = new List<hypster_tv_DAL.Playlist>();

            model = playlistManager.GetWithDescPlaylists();

            return View(model);
        }





        public ActionResult Generate_Hypster_Stations()
        {
            hypster_tv_DAL.MemberMusicGenreManager genreManager = new hypster_tv_DAL.MemberMusicGenreManager();
            
            List<hypster_tv_DAL.MusicGenre> genres_list = new List<hypster_tv_DAL.MusicGenre>();
            genres_list = genreManager.GetMusicGenresList();

            return View(genres_list);
        }





        public ActionResult Generate_Hypster_Featured()
        {
            hypster_tv_DAL.FeaturedPlaylistManagement fp_manager = new hypster_tv_DAL.FeaturedPlaylistManagement();

            List<hypster_tv_DAL.FeaturedPlaylist_Result> fp_list = new List<hypster_tv_DAL.FeaturedPlaylist_Result>();
            fp_list = fp_manager.ReturnFeaturedPlaylists();

            return View(fp_list);
        }
       

        



    }
}

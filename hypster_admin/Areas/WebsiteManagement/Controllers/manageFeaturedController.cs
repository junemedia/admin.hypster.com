using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    public class manageFeaturedController : Controller
    {
        //
        // GET: /WebsiteManagement/manageFeatured/

        public ActionResult Index()
        {

            string contType = "";
            if (Request.QueryString["contType"] != null)
            {
                contType = Request.QueryString["contType"].ToString();
            }




            hypster_tv_DAL.featuredContentManagement fc_manager = new hypster_tv_DAL.featuredContentManagement();
            List<hypster_tv_DAL.FeaturedContent> model = new List<hypster_tv_DAL.FeaturedContent>();
            
            switch (contType)
            {
                case "SP":
                    ViewBag.contType = "Seasonal Playlists";
                    model = fc_manager.ReturnFeaturedContent((int)hypster_tv_DAL.fc_Type.Seasonal_Playlists);
                    break;
                case "AP":
                    ViewBag.contType = "Artist Playlists";
                    model = fc_manager.ReturnFeaturedContent((int)hypster_tv_DAL.fc_Type.Artist_Playlists);
                    break;
                case "OT":
                    ViewBag.contType = "Other";
                    model = fc_manager.ReturnFeaturedContent((int)hypster_tv_DAL.fc_Type.Other);
                    break;
                default:
                    ViewBag.contType = "Seasonal Playlists";
                    model = fc_manager.ReturnFeaturedContent((int)hypster_tv_DAL.fc_Type.Seasonal_Playlists);
                    break;
            }



            return View(model);
        }




        public ActionResult AddToFeatured()
        {
            int id = 0;
            if (Request.QueryString["id"] != null)
            {
                Int32.TryParse(Request.QueryString["id"].ToString(), out id);
            }

            string type = "";
            if (Request.QueryString["type"] != null)
            {
                type = Request.QueryString["type"].ToString();
            }

            ViewBag.ContID = id;
            ViewBag.ContType = type;



            hypster_tv_DAL.homeSlideshowManager slideshowManager = new hypster_tv_DAL.homeSlideshowManager();

            hypster_tv_DAL.homeSlideshow slideshow = new hypster_tv_DAL.homeSlideshow();
            slideshow = slideshowManager.homeSlideshowByID(id);




            return View(slideshow);
        }





        [HttpPost]
        public ActionResult AddToFeatured(string href, string href_mobile, string name, string image, int plstType, int ContID, string ContType)
        {
            hypster_tv_DAL.homeSlideshowManager slideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            hypster_tv_DAL.homeSlideshow slideshow = new hypster_tv_DAL.homeSlideshow();
            slideshow = slideshowManager.homeSlideshowByID(ContID);


            //hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
            //image_resizer.Resize_Image(System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + image, 250, -1, System.Drawing.Imaging.ImageFormat.Jpeg,  System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + "sm_" + image);


            hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
            image_resizer.Resize_Image(System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + image, 800, -1, System.Drawing.Imaging.ImageFormat.Jpeg, System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + "sm_" + image);



            hypster_tv_DAL.featuredContentManagement fc_manager = new hypster_tv_DAL.featuredContentManagement();
            hypster_tv_DAL.FeaturedContent fc_add = new hypster_tv_DAL.FeaturedContent();

            fc_add.fc_active = true;
            fc_add.fc_href = href;
            fc_add.fc_href_mobile = href_mobile;
            fc_add.fc_name = name;
            fc_add.fc_image = "sm_" + image; // need to resize
            fc_add.fc_type = plstType;

            fc_manager.hyDB.FeaturedContents.AddObject(fc_add);
            fc_manager.hyDB.SaveChanges();



            return RedirectPermanent("/WebsiteManagement/manageFeatured");
        }



        public ActionResult deleteFC(int id)
        {
            hypster_tv_DAL.featuredContentManagement fc_manager = new hypster_tv_DAL.featuredContentManagement();
            fc_manager.delete_fc(id);

            return RedirectPermanent("/WebsiteManagement/manageFeatured");
        }




        public ActionResult AddToFeaturedSlideshow(int? id)
        {
            hypster_tv_DAL.homeSlideshowManager slideshow_manager = new hypster_tv_DAL.homeSlideshowManager();

            if (id != null)
            {
                slideshow_manager.AddFeaturedSlideshow((int)id);
            }


            List<hypster_tv_DAL.homeSlideshow> slideshow_list = new List<hypster_tv_DAL.homeSlideshow>();
            slideshow_list = slideshow_manager.getFeaturedSlideshowActive();


            return View(slideshow_list);
        }


        public ActionResult DeleteFeaturedSlideshow(int? id)
        {
            hypster_tv_DAL.homeSlideshowManager slideshow_manager = new hypster_tv_DAL.homeSlideshowManager();

            if (id != null)
            {
                slideshow_manager.DeleteFeaturedSlideshow((int)id);
            }

            return RedirectPermanent("/WebsiteManagement/manageFeatured/AddToFeaturedSlideshow");
        }




    }
}

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.Editors.Controllers
{
    [Authorize]
    public class manageSlideshowController : Controller
    {
        //
        // GET: /Editors/manageSlideshow/
        public ActionResult Index()
        {
            hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            List<hypster_tv_DAL.homeSlideshow> model = new List<hypster_tv_DAL.homeSlideshow>();
            model = homeSlideshowManager.getHomeSlideshow();
            return View(model);
        }

        [HttpGet]
        public ActionResult AddNewSlideshow()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewSlideshow(HttpPostedFileBase file, string href, bool isPopupMusicPlayer)
        {

            if (file != null && file.ContentLength > 0)
            {
                hypster_tv_DAL.homeSlideshow homeSlide = new hypster_tv_DAL.homeSlideshow();
                if (isPopupMusicPlayer == true)
                {
                    homeSlide.href = "OpenPlayerM('" + href + "');";
                }
                else
                {
                    homeSlide.href = "window.location='" + href + "';";
                }

                var extension = System.IO.Path.GetExtension(file.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/uploads"), "new_home_slide" + extension);
                file.SaveAs(path);


                string image_guid = System.Guid.NewGuid().ToString();
                System.IO.FileInfo file_slide = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_home_slide" + extension);
                file_slide.CopyTo(System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + image_guid + file_slide.Extension, true);

                //delete file
                System.IO.FileInfo del_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_home_slide" + extension);
                del_file.Delete();

                hypster_tv_DAL.homeSlideshowManager slideshowManager = new hypster_tv_DAL.homeSlideshowManager();
                slideshowManager.IncAllSlides();

                homeSlide.isActive = true;
                homeSlide.SortOrder = 1;
                homeSlide.ImageSrc = image_guid + file_slide.Extension;

                hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();
                hyDB.homeSlideshows.AddObject(homeSlide);
                hyDB.SaveChanges();

                return RedirectToAction("Index");

            }
            else
            {
                ModelState.AddModelError("", "Please add image");
            }
            return View();
        }


        public string ResetSortOrder()
        {
            hypster_tv_DAL.homeSlideshowManager slideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            slideshowManager.ResetSortOrder();
            return "OK";
        }

        [HttpGet]
        public ActionResult EditSlideshow(int id)
        {
            hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            hypster_tv_DAL.homeSlideshow slide = new hypster_tv_DAL.homeSlideshow();
            hypster_tv_DAL.homeslideImageTracking tracking = new hypster_tv_DAL.homeslideImageTracking();
            slide = homeSlideshowManager.homeSlideshowByID(id);
            ViewBag.ID = id;
            if (slide.href.StartsWith("OpenPlayerM"))
                ViewBag.check = true;
            else
                ViewBag.check = false;
            slide.href = slide.href.Substring(slide.href.IndexOf("'") + 1).Replace("';","").Replace("');", "");
            return View(slide);
        }

        [HttpPost]
        public ActionResult EditSlideshow(HttpPostedFileBase file, string ImgSrc, string href, bool isPopupMusicPlayer)
        {
            string[] s = Request.AppRelativeCurrentExecutionFilePath.Split('/');
            hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();
            hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            hypster_tv_DAL.homeSlideshow slide = new hypster_tv_DAL.homeSlideshow();
            slide = homeSlideshowManager.homeSlideshowByID(Convert.ToInt32(s[s.Length - 1]));
            string image_guid = "";
            if (file != null && file.ContentLength > 0)
            {
                var extension = System.IO.Path.GetExtension(file.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/uploads"), "new_home_slide" + extension);
                file.SaveAs(path);

                image_guid = System.Guid.NewGuid().ToString();
                //
                // resize image
                //
                hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
                image_resizer.Resize_Image(path, 621, 376, System.Drawing.Imaging.ImageFormat.Jpeg);

                System.IO.FileInfo file_slide = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_home_slide" + extension);
                file_slide.CopyTo(System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + image_guid + extension, true);

                //delete file
                System.IO.FileInfo del_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_home_slide" + extension);
                del_file.Delete();
                slide.ImageSrc = image_guid + extension;
            }
            else
            {
                slide.ImageSrc = ImgSrc;
            }

            if (isPopupMusicPlayer == true)
            {
                slide.href = "OpenPlayerM('" + href + "');";
            }
            else
            {
                slide.href = "window.location='" + href + "';";
            }
            hyDB.sp_homeSlideshow_UpdateHomeSlideshow(slide.homeSlideshow_ID, slide.href, slide.ImageSrc);
            hyDB.SaveChanges();
            return RedirectToAction("Index");
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public ActionResult Delete(int id)
        {
            hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            hypster_tv_DAL.homeSlideshow slide = new hypster_tv_DAL.homeSlideshow();
            slide = homeSlideshowManager.homeSlideshowByID(id);

            if (slide.homeSlideshow_ID != 0)
            {
                //remove image
                System.IO.FileInfo del_file = new System.IO.FileInfo(System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + slide.ImageSrc);
                del_file.Delete();

                homeSlideshowManager.DeleteHomeSlideshow(slide.homeSlideshow_ID);
            }

            homeSlideshowManager.ResetSortOrder();

            return RedirectToAction("Index");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public ActionResult Deactivate(int id)
        {
            hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            hypster_tv_DAL.homeSlideshow slide = new hypster_tv_DAL.homeSlideshow();
            slide = homeSlideshowManager.homeSlideshowByID(id);

            if (slide.homeSlideshow_ID != 0)
            {
                homeSlideshowManager.DeactivateHomeSlideshow(slide.homeSlideshow_ID);
            }
            return RedirectToAction("Index");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public ActionResult Activate(int id)
        {
            hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            hypster_tv_DAL.homeSlideshow slide = new hypster_tv_DAL.homeSlideshow();
            slide = homeSlideshowManager.homeSlideshowByID(id);

            if (slide.homeSlideshow_ID != 0)
            {
                homeSlideshowManager.ActivateHomeSlideshow(slide.homeSlideshow_ID);
            }
            return RedirectToAction("Index");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        [OutputCache(Duration = 0)]
        public ActionResult SlideUp()
        {
            int id = 0;
            if (Request.QueryString["sl"] != null)
            {
                Int32.TryParse(Request.QueryString["sl"].ToString(), out id);
            }
            hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            homeSlideshowManager.MoveSlideUp((int)id);
            return RedirectPermanent("/Editors/manageSlideshow");
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public ActionResult SlideDown()
        {
            int id = 0;
            if (Request.QueryString["sl"] != null)
            {
                Int32.TryParse(Request.QueryString["sl"].ToString(), out id);
            }
            hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            homeSlideshowManager.MoveSlideDown((int)id);
            return RedirectPermanent("/Editors/manageSlideshow");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------

        //[HttpPost]
        //public ActionResult AddToFeatured(string href, string href_mobile, string name, string image, int plstType, int ContID, string ContType)
        //{
        //    if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
        //    {
        //        hypster_tv_DAL.homeSlideshowManager slideshowManager = new hypster_tv_DAL.homeSlideshowManager();
        //        hypster_tv_DAL.homeSlideshow slideshow = new hypster_tv_DAL.homeSlideshow();
        //        slideshow = slideshowManager.homeSlideshowByID(ContID);
        //        hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
        //        image_resizer.Resize_Image(System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + image, 800, -1, System.Drawing.Imaging.ImageFormat.Jpeg, System.Configuration.ConfigurationManager.AppSettings["homeSlideshowStorage_Path"] + "\\" + "sm_" + image);
        //        hypster_tv_DAL.featuredContentManagement fc_manager = new hypster_tv_DAL.featuredContentManagement();
        //        hypster_tv_DAL.FeaturedContent fc_add = new hypster_tv_DAL.FeaturedContent();
        //        fc_add.fc_active = true;
        //        fc_add.fc_href = href;
        //        fc_add.fc_href_mobile = href_mobile;
        //        fc_add.fc_name = name;
        //        fc_add.fc_image = "sm_" + image; // need to resize
        //        fc_add.fc_type = plstType;
        //        fc_manager.hyDB.FeaturedContents.AddObject(fc_add);
        //        fc_manager.hyDB.SaveChanges();
        //        return RedirectPermanent("/Editors/manageSlideshow");
        //    }
        //    else
        //        return RedirectPermanent("/home/");
        //}

        //public ActionResult AddToFeaturedSlideshow(int? id)
        //{
        //    if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
        //    {
        //        hypster_tv_DAL.homeSlideshowManager slideshow_manager = new hypster_tv_DAL.homeSlideshowManager();
        //        if (id != null)
        //        {
        //            slideshow_manager.AddFeaturedSlideshow((int)id);
        //        }
        //        List<hypster_tv_DAL.homeSlideshow> slideshow_list = new List<hypster_tv_DAL.homeSlideshow>();
        //        slideshow_list = slideshow_manager.getFeaturedSlideshowActive();
        //        return View(slideshow_list);
        //    }
        //    else
        //        return RedirectPermanent("/home/");
        //}
    }
}
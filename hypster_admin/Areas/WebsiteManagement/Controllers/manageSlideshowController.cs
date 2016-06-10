using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class manageSlideshowController : Controller
    {
        //
        // GET: /WebsiteManagement/manageSlideshow/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
                List<hypster_tv_DAL.homeSlideshow> model = new List<hypster_tv_DAL.homeSlideshow>();
                model = homeSlideshowManager.getHomeSlideshow();
                return View(model);
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpGet]
        public ActionResult AddNewSlideshow()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult AddNewSlideshow(HttpPostedFileBase file, string href, bool isPopupMusicPlayer)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
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
                    //
                    // resize image
                    //
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
            else
                return RedirectPermanent("/home/");
        }


        public string ResetSortOrder()
        {
            hypster_tv_DAL.homeSlideshowManager slideshowManager = new hypster_tv_DAL.homeSlideshowManager();
            slideshowManager.ResetSortOrder();
            return "OK";
        }


        //--------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
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
            else
                return RedirectPermanent("/home/");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public ActionResult Deactivate(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
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
            else
                return RedirectPermanent("/home/");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public ActionResult Activate(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
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
            else
                return RedirectPermanent("/home/");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        [OutputCache(Duration = 0)]
        public ActionResult SlideUp()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                int id = 0;
                if (Request.QueryString["sl"] != null)
                {
                    Int32.TryParse(Request.QueryString["sl"].ToString(), out id);
                }
                hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
                homeSlideshowManager.MoveSlideUp((int)id);
                return RedirectPermanent("/WebsiteManagement/manageSlideshow");
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpGet]
        [OutputCache(Duration=0)]
        public ActionResult SlideDown()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                int id = 0;
                if (Request.QueryString["sl"] != null)
                {
                    Int32.TryParse(Request.QueryString["sl"].ToString(), out id);
                }
                hypster_tv_DAL.homeSlideshowManager homeSlideshowManager = new hypster_tv_DAL.homeSlideshowManager();
                homeSlideshowManager.MoveSlideDown((int)id);
                return RedirectPermanent("/WebsiteManagement/manageSlideshow");
            }
            else
                return RedirectPermanent("/home/");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------
    }
}

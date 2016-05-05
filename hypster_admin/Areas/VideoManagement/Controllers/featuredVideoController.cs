using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.VideoManagement.Controllers
{
    [Authorize]
    public class featuredVideoController : Controller
    {
        //
        // GET: /VideoManagement/featuredVideo/

        public ActionResult Index()
        {
            hypster_tv_DAL.videoFeaturedManager featuredVideoManager = new hypster_tv_DAL.videoFeaturedManager();
            List<hypster_tv_DAL.videoFeaturedSlideshow> videos_list = new List<hypster_tv_DAL.videoFeaturedSlideshow>();
            videos_list = featuredVideoManager.getFeaturedVideos();

            return View(videos_list);
        }






        [HttpGet]
        public ActionResult AddNewFeaturedVideo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewFeaturedVideo(HttpPostedFileBase file, hypster_tv_DAL.videoFeaturedSlideshow p_featuredVideo)
        {

            if (file != null && file.ContentLength > 0)
            {
                hypster_tv_DAL.videoFeatured featuredVideo = new hypster_tv_DAL.videoFeatured();



                var extension = System.IO.Path.GetExtension(file.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/uploads"), "new_featured_slide" + extension);
                file.SaveAs(path);


                string image_guid = System.Guid.NewGuid().ToString();
                //
                // resize image
                //
                //int video_width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["postImage_maxWidth"]);
                hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
                image_resizer.Resize_Image(path, 621, 376, System.Drawing.Imaging.ImageFormat.Jpeg);

                System.IO.FileInfo file_slide = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_featured_slide" + extension);
                file_slide.CopyTo(System.Configuration.ConfigurationManager.AppSettings["videoSlideshowStorage_Path"] + "\\" + image_guid + file_slide.Extension, true);

                //delete file
                System.IO.FileInfo del_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_featured_slide" + extension);
                del_file.Delete();





                hypster_tv_DAL.videoClipManager_Admin videoManager = new hypster_tv_DAL.videoClipManager_Admin();
                int new_video_ID = videoManager.getVideoByGUID(p_featuredVideo.Guid).videoClip_ID;

                //if (new_video_ID != null && new_video_ID > 0)
                if (new_video_ID > 0)
                {

                    featuredVideo.videoClip_ID = new_video_ID;
                    featuredVideo.SortOrder = 0;
                    featuredVideo.ImageSrc = image_guid + file_slide.Extension;

                    hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();
                    hyDB.videoFeatureds.AddObject(featuredVideo);
                    hyDB.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Please check image GUID. System can't find video.");
                }

            }
            else
            {
                ModelState.AddModelError("", "Please add image");
            }
            



            return View();
        }









        [HttpGet]
        public ActionResult Edit(int id)
        {
            hypster_tv_DAL.videoFeaturedManager featuredManager = new hypster_tv_DAL.videoFeaturedManager();
            hypster_tv_DAL.videoFeaturedSlideshow slide = new hypster_tv_DAL.videoFeaturedSlideshow();
            slide = featuredManager.getFeaturedVideoByID(id);


            ViewBag.ID = id;


            return View(slide);
        }

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, hypster_tv_DAL.videoFeaturedSlideshow featuredVideo)
        {

            if (file != null && file.ContentLength > 0)
            {
                var extension = System.IO.Path.GetExtension(file.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/uploads"), "new_featured_slide" + extension);
                file.SaveAs(path);


                string image_guid = featuredVideo.ImageSrc;
                //
                // resize image
                //
                //int video_width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["postImage_maxWidth"]);
                hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
                image_resizer.Resize_Image(path, 621, 376, System.Drawing.Imaging.ImageFormat.Jpeg);

                System.IO.FileInfo file_slide = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_featured_slide" + extension);
                file_slide.CopyTo(System.Configuration.ConfigurationManager.AppSettings["videoSlideshowStorage_Path"] + "\\" + image_guid, true);

                //delete file
                System.IO.FileInfo del_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_featured_slide" + extension);
                del_file.Delete();
            }


            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult Delete(int id)
        {
            hypster_tv_DAL.videoFeaturedManager featuredManager = new hypster_tv_DAL.videoFeaturedManager();
            hypster_tv_DAL.videoFeaturedSlideshow slide = new hypster_tv_DAL.videoFeaturedSlideshow();
            slide = featuredManager.getFeaturedVideoByID(id);

            if (slide.videoFeatured_ID != 0)
            {
                //remove image
                System.IO.FileInfo del_file = new System.IO.FileInfo(System.Configuration.ConfigurationManager.AppSettings["videoSlideshowStorage_Path"] + "\\" + slide.ImageSrc);
                del_file.Delete();

                featuredManager.DeleteFeaturedVideo(slide.videoFeatured_ID);
            }

            return RedirectToAction("Index");
        }

    }
}

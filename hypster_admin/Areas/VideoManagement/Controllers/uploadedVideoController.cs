using System;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.VideoManagement.Controllers
{
    [Authorize]
    public class uploadedVideoController : Controller
    {
        //
        // GET: /VideoManagement/uploadedVideo/
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }


        [HttpGet]
        public ActionResult Edit(string video_guid)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_admin.Areas.VideoManagement.ViewModels.UploadedVideos_ViewModel viewModel = new ViewModels.UploadedVideos_ViewModel();
                //
                // fill out videos list
                hypster_tv_DAL.videoClipManager_Admin videoClipManager = new hypster_tv_DAL.videoClipManager_Admin();
                viewModel.videoClips_list = videoClipManager.getTopRatedVideos(); //will need to change to show all videos list
                //
                // if edit video then need to populate form
                if (video_guid != null)
                {
                    viewModel.edit_clip = videoClipManager.getVideoByGUID(video_guid);
                    //
                    // if video is pending need to check if it in propper format
                    //-----------------------------------------------------------------------------------------------------
                    if (viewModel.edit_clip.Status == -1)
                    {
                        System.IO.DirectoryInfo dirInf = new System.IO.DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["videoClipsPending_Path"]);
                        System.IO.FileInfo[] files = dirInf.GetFiles();
                        foreach (System.IO.FileInfo file in files)
                        {
                            if (file.Name.Contains(viewModel.edit_clip.Guid))
                            {
                                ViewBag.DownloadPath = "http://" + System.Configuration.ConfigurationManager.AppSettings["videoHostName"] + "/uploads_pending/" + viewModel.edit_clip.Guid + file.Extension;
                                if (file.Extension != ".flv")
                                {
                                    ViewBag.hideStatus = "display:none;";
                                    ViewBag.Message = "Video needs to be converted to flv format.";
                                }
                            }
                        }                    
                    }
                    //-----------------------------------------------------------------------------------------------------
                }
                return View(viewModel);
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, hypster_tv_DAL.videoClip p_clip)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.videoClipManager_Admin videoManager = new hypster_tv_DAL.videoClipManager_Admin();
                bool isEditingAllowed = true;
                //
                //check existing clip before editing
                //-------------------------------------------------------------------------------------------------------------------------------
                hypster_tv_DAL.videoClip checkClip = new hypster_tv_DAL.videoClip();
                checkClip = videoManager.getVideoByGUID(p_clip.Guid);
                // if old status was pending and now moving to another status need to move video file from pending directory
                if (checkClip.Status == -1 && p_clip.Status != -1)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(System.Configuration.ConfigurationManager.AppSettings["videoClipsPending_Path"] + "\\" + p_clip.Guid + ".flv");
                    if (fileInfo.Exists)
                        fileInfo.MoveTo(System.Configuration.ConfigurationManager.AppSettings["videoClipsStorage_Path"] + "\\" + p_clip.Guid + ".flv");
                    else
                        isEditingAllowed = false; //do not allow editing since it PENDING video with not supported video encoding
                }
                //-------------------------------------------------------------------------------------------------------------------------------

                //
                //editing
                //-------------------------------------------------------------------------------------------------------------------------------
                if (isEditingAllowed)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var extension = ".png"; //System.IO.Path.GetExtension(file.FileName);
                        var path = System.IO.Path.Combine(Server.MapPath("~/uploads"), "new_video_cover" + extension);
                        file.SaveAs(path);

                        string image_guid = p_clip.Guid;
                        //
                        // resize image
                        int video_thumb_width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["videoThumbnail_Width"]);
                        int video_thumb_height = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["videoThumbnail_Height"]);

                        hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
                        image_resizer.Resize_Image(path, video_thumb_width, video_thumb_height, System.Drawing.Imaging.ImageFormat.Png);

                        System.IO.FileInfo file_slide = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_video_cover" + extension);
                        file_slide.CopyTo(System.Configuration.ConfigurationManager.AppSettings["videoCoversStorage_Path"] + "\\" + image_guid + file_slide.Extension, true);

                        //delete file
                        System.IO.FileInfo del_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_video_cover" + extension);
                        del_file.Delete();
                    }
                    //edit video
                    videoManager.EditVideo(p_clip);
                }
                //-------------------------------------------------------------------------------------------------------------------------------
                return RedirectToAction(p_clip.Guid);
            }
            else
                return RedirectPermanent("/home/");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.NewsManagement.Controllers
{
    public class homeCelebsController : Controller
    {
        //
        // GET: /NewsManagement/homeCelebs/

        public ActionResult Index()
        {
            hypster_tv_DAL.celebsManagement celebsManager = new hypster_tv_DAL.celebsManagement();


            List<hypster_tv_DAL.newsCeleb> celebs_list = new List<hypster_tv_DAL.newsCeleb>();
            celebs_list = celebsManager.GetLatestCelebs(256);


            return View(celebs_list);
        }




        [HttpGet]
        public ActionResult AddNewCeleb()
        {
            return View();
        }




        [HttpPost]
        public ActionResult AddNewCeleb(HttpPostedFileBase file, string txt_Name, string txt_Url)
        {
            if (file != null && file.ContentLength > 0)
            {
                hypster_tv_DAL.celebsManagement celebsManager = new hypster_tv_DAL.celebsManagement();
                hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();


                hypster_tv_DAL.newsCeleb p_celeb = new hypster_tv_DAL.newsCeleb();
                p_celeb.celeb_name = txt_Name;
                p_celeb.celeb_url = txt_Url;
                p_celeb.celeb_image = txt_Name.Replace("/", "").Replace("\\", "").Replace("&", "").Replace("+", "").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace("*", "").Replace("$", "").Replace("\"", "").Replace("'", "").Replace("{", "").Replace("}", "").Replace(")", "").Replace("(", "").Replace("[", "").Replace("]", "").Replace("|", "").Replace(".", "").Replace(",", "").Replace(":", "").Replace(";", "");
                p_celeb.celeb_image = p_celeb.celeb_image + "_" + DateTime.Now.ToShortDateString().Replace("/","_");



                var extension = System.IO.Path.GetExtension(file.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/uploads"), "new_post" + extension);
                file.SaveAs(path);


                //
                // resize image
                int video_width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["postCelebImage_maxWidth"]);
                image_resizer.Resize_Image(path, video_width, -1, System.Drawing.Imaging.ImageFormat.Jpeg);





                //save post image
                System.IO.FileInfo fileInf = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_post" + extension);
                fileInf.CopyTo(System.Configuration.ConfigurationManager.AppSettings["newsCelebsImageStorage_Path"] + "\\" + p_celeb.celeb_image + fileInf.Extension, true);

                //save thumbnail
                System.IO.FileInfo thumb_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_post" + extension);
                string new_thumb_path = System.Configuration.ConfigurationManager.AppSettings["newsCelebsImageStorage_Path"] + "\\thumb_" + p_celeb.celeb_image + thumb_file.Extension;
                thumb_file.CopyTo(new_thumb_path, true);

                int thumb_width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["postCelebThumb_maxWidth"]);
                image_resizer.Resize_Image(new_thumb_path, thumb_width, -1, System.Drawing.Imaging.ImageFormat.Jpeg);

                //delete file
                System.IO.FileInfo del_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_post" + extension);
                del_file.Delete();

                p_celeb.celeb_image = p_celeb.celeb_image + fileInf.Extension;



                
                // save post after image is done
                celebsManager.hyDB.newsCelebs.AddObject(p_celeb);
                celebsManager.hyDB.SaveChanges();
            }

            return RedirectPermanent("/NewsManagement/homeCelebs");
        }






        public ActionResult DelCeleb()
        {

            if (Request.QueryString["Celeb_ID"] != null)
            {
                int seleb_id = 0;
                Int32.TryParse(Request.QueryString["Celeb_ID"].ToString(), out seleb_id);


                if (seleb_id > 0)
                {
                    hypster_tv_DAL.celebsManagement celebsManager = new hypster_tv_DAL.celebsManagement();
                    
                    hypster_tv_DAL.newsCeleb celeb_del = new hypster_tv_DAL.newsCeleb();
                    celeb_del = celebsManager.GetCelebByID(seleb_id);


                    try
                    {
                        System.IO.FileInfo fileInf = new System.IO.FileInfo(System.Configuration.ConfigurationManager.AppSettings["newsCelebsImageStorage_Path"] + "\\" + celeb_del.celeb_image);
                        fileInf.Delete();

                        System.IO.FileInfo thumb_file = new System.IO.FileInfo(System.Configuration.ConfigurationManager.AppSettings["newsCelebsImageStorage_Path"] + "\\thumb_" + celeb_del.celeb_image);
                        thumb_file.Delete();
                    }
                    finally
                    {
                    }


                    celebsManager.DeleteSeleb(seleb_id);
                }

            }

            return RedirectPermanent("/NewsManagement/homeCelebs");
        }



    }
}

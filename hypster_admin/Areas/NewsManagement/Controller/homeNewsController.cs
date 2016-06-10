using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;

namespace hypster_admin.Areas.NewsManagement.Controllers
{
    [Authorize]
    public class homeNewsController : Controller
    {
        //
        // GET: /NewsManagement/homeNews/

        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.newsManagement_Admin newsManager = new hypster_tv_DAL.newsManagement_Admin();
                List<hypster_tv_DAL.newsPost> newsPost_list = new List<hypster_tv_DAL.newsPost>();
                newsPost_list = newsManager.GetLatestNews();
                return View(newsPost_list);
            }
            else
                return RedirectPermanent("/home/");
        }

        [HttpGet]
        public ActionResult AddNewPost()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.newsPost newPost = new hypster_tv_DAL.newsPost();
                return View(newPost);
            }
            else
                return RedirectPermanent("/home/");
        }
                
        [HttpPost, ValidateInput(false)] 
        public ActionResult AddNewPost(hypster_tv_DAL.newsPost p_newPost)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();
                hypster_tv_DAL.newsManagement_Admin newsManager = new hypster_tv_DAL.newsManagement_Admin();
                if (p_newPost.post_title != null && p_newPost.post_title != "")
                {

                    p_newPost.post_date = DateTime.Now;
                    p_newPost.post_guid = p_newPost.post_title.Replace("/", "").Replace("\\", "").Replace("&", "").Replace("+", "").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace("*", "").Replace("$", "").Replace("\"", "").Replace("'", "").Replace("{", "").Replace("}", "").Replace(")", "").Replace("(", "").Replace("[", "").Replace("]", "").Replace("|", "").Replace(".", "").Replace(",", "").Replace(":", "").Replace(";", "");
                    p_newPost.post_status = (int)hypster_tv_DAL.postStatus.NoActive;
                    //
                    //check if post guid is exist in database
                    hypster_tv_DAL.newsPost post_check = newsManager.GetPostByGUID(p_newPost.post_guid);
                    if (post_check.post_id != 0 && post_check.post_guid != "")
                    {
                        ModelState.AddModelError("", "NOT ABLE TO GENERATE POST GUID.Please choose modify title. Post with following title already exist.");
                    }
                    else //if post guid is unique then procceed to finish
                    {
                        hyDB.newsPosts.AddObject(p_newPost);
                        hyDB.SaveChanges();
                        return RedirectToAction("Index", "homeNews");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please enter post title. It must be unique from previous posts!!!");
                }
                // if no success
                return View(new hypster_tv_DAL.newsPost());
            }
            else
                return RedirectPermanent("/home/");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.newsPost newsPost = new hypster_tv_DAL.newsPost();
                hypster_tv_DAL.newsManagement_Admin newsManager = new hypster_tv_DAL.newsManagement_Admin();
                newsPost = newsManager.GetPostByID(id);
                ViewBag.ID = id;
                return View(newsPost);
            }
            else
                return RedirectPermanent("/home/");
        }




        [HttpPost, ValidateInput(false)]
        public string Edit(hypster_tv_DAL.newsPost p_Post, string splashArt)
        {
            hypster_tv_DAL.newsManagement_Admin newsManager = new hypster_tv_DAL.newsManagement_Admin();
            newsManager.EditPost(p_Post);
            if (splashArt == "on")
            {
                hypster_tv_DAL.sysHypster_Management sysManager = new hypster_tv_DAL.sysHypster_Management();
                sysManager.SetSplashID(p_Post.post_id);
            }
            if (p_Post.ad_id != "")
            {
                if (p_Post.ad_id == null)
                {
                    p_Post.ad_id = "";
                }
                newsManager.SetPostAdID(p_Post);
            }

            //update sitemaps date and ping google and bing
            //
            int sitemapNewsID = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["sitemap_newsID"]);

            hypster_tv_DAL.SitemapManagement sitemapManager = new hypster_tv_DAL.SitemapManagement();
            sitemapManager.UpdateDateChanged(sitemapNewsID, DateTime.Now);

            HttpWebRequest ping_google_request = (HttpWebRequest)WebRequest.Create("http://www.google.com/webmasters/sitemaps/ping?sitemap=http://hypster.com/sitemaps/sitemap_index");
            HttpWebResponse ping_google_response = (HttpWebResponse)ping_google_request.GetResponse();
            Stream google_resStream = ping_google_response.GetResponseStream();
            String google_responseString = "";
            using (google_resStream)
            {
                StreamReader google_reader = new StreamReader(google_resStream, Encoding.UTF8);
                google_responseString = google_reader.ReadToEnd();
            }

            HttpWebRequest ping_bing_request = (HttpWebRequest)WebRequest.Create("http://www.bing.com/webmaster/ping.aspx?siteMap=http://hypster.com/sitemaps/sitemap_index");
            HttpWebResponse ping_bing_response = (HttpWebResponse)ping_bing_request.GetResponse();
            Stream bing_resStream = ping_bing_response.GetResponseStream();
            String bing_responseString = "";
            using (bing_resStream)
            {
                StreamReader bing_reader = new StreamReader(bing_resStream, Encoding.UTF8);
                bing_responseString = bing_reader.ReadToEnd();
            }            
            //-------------------------------------------------------------------------------------------------------------
            return google_responseString + "<br/>" + bing_responseString + " <br/> <a href='/NewsManagement/homeNews'>Go To News Home</a> ";
        }
        //------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------
        //
        //user uploads post image here
        //

        [HttpPost]
        public ActionResult UploadPostImageEdit(HttpPostedFileBase file, int post_id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                if (file != null && file.ContentLength > 0)
                {
                    hypster_tv_DAL.newsManagement_Admin newsManager = new hypster_tv_DAL.newsManagement_Admin();
                    hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();

                    hypster_tv_DAL.newsPost p_Post = new hypster_tv_DAL.newsPost();
                    p_Post = newsManager.GetPostByID(post_id);

                    var extension = System.IO.Path.GetExtension(file.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/uploads"), "new_post" + extension);
                    file.SaveAs(path);

                    //save post image
                    System.IO.FileInfo fileInf = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_post" + extension);
                    fileInf.CopyTo(System.Configuration.ConfigurationManager.AppSettings["newsImageStorage_Path"] + "\\" + p_Post.post_guid + fileInf.Extension, true);
                    //
                    // resize image old
                    int video_width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["postImage_maxWidth"]);
                    image_resizer.Resize_Image(System.Configuration.ConfigurationManager.AppSettings["newsImageStorage_Path"] + "\\" + p_Post.post_guid + fileInf.Extension, video_width, -1, System.Drawing.Imaging.ImageFormat.Jpeg);

                    //save thumbnail
                    System.IO.FileInfo thumb_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_post" + extension);
                    string new_thumb_path = System.Configuration.ConfigurationManager.AppSettings["newsImageStorage_Path"] + "\\thumb_" + p_Post.post_guid + thumb_file.Extension;
                    thumb_file.CopyTo(new_thumb_path, true);

                    int thumb_width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["postThumb_maxWidth"]);
                    image_resizer.Resize_Image(new_thumb_path, thumb_width, -1, System.Drawing.Imaging.ImageFormat.Jpeg);

                    //save new image
                    System.IO.FileInfo newim_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_post" + extension);
                    string newim_path = System.Configuration.ConfigurationManager.AppSettings["newsImageStorage_Path"] + "\\img_" + p_Post.post_guid + newim_file.Extension;
                    newim_file.CopyTo(newim_path, true);

                    int newim_width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["newPostImage_maxWidth"]);
                    image_resizer.Resize_Image(newim_path, newim_width, -1, System.Drawing.Imaging.ImageFormat.Jpeg);
                    
                    //delete file
                    System.IO.FileInfo del_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "new_post" + extension);
                    del_file.Delete();
                    p_Post.post_image = p_Post.post_guid + fileInf.Extension;
                    //
                    // save post after image is done
                    newsManager.EditPost(p_Post);
                }
                return RedirectToAction("Edit", new { id = post_id });
            }
            else
                return RedirectPermanent("/home/");
        }
        //------------------------------------------------------------------------------------------
        
        //------------------------------------------------------------------------------------------
        public ActionResult getNewsTags(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();
                List<hypster_tv_DAL.sp_Tag_GetNewsTags_Result> news_tags = new List<hypster_tv_DAL.sp_Tag_GetNewsTags_Result>();
                news_tags = tagManager.GetNewsTags(id);
                return View(news_tags);
            }
            else
                return RedirectPermanent("/home/");
        }
        //------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------
        public string addnewtag()
        {
            string ret_res = "";
            string tag_name = "";
            if (Request.QueryString["tag_name"] != null)
            {
                tag_name = Request.QueryString["tag_name"].ToString();
            }

            int article_id = 0;
            if (Request.QueryString["article_id"] != null)
            {
                Int32.TryParse(Request.QueryString["article_id"], out article_id);
            }
            
            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();
            member = memberManager.getMemberByUserName(User.Identity.Name);

            hypster_tv_DAL.newsManagement newsManager = new hypster_tv_DAL.newsManagement();
            hypster_tv_DAL.newsPost curr_article = new hypster_tv_DAL.newsPost();
            curr_article = newsManager.GetPostByID(article_id);

            if (curr_article.post_id != 0 && article_id == curr_article.post_id)
            {
                hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();
                int tag_ID = 0;
                tag_ID = tagManager.AddNewTag(tag_name);
                tagManager.AddTagToNewsArticle(tag_ID, article_id);
                ret_res = tag_ID.ToString() + "|" + article_id.ToString();
            }
            else
            {
                ret_res = "n/a";
            }
            return ret_res.ToString();
        }
        //------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------
        public string deletetag()
        {
            int tag_plst_id = 0;
            if (Request.QueryString["tag_plst_id"] != null)
            {   
                Int32.TryParse(Request.QueryString["tag_plst_id"].ToString(), out tag_plst_id);
            }

            int article_id = 0;
            if (Request.QueryString["article_id"] != null)
            {
                Int32.TryParse(Request.QueryString["article_id"], out article_id);
            }

            hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();
            tagManager.DeleteNewsTag(tag_plst_id);

            return "";
        }
        //------------------------------------------------------------------------------------------


        public string getUserIdByName()
        {
            string str_val = "";

            string username = "";
            if (Request.QueryString["username"] != null)
            {
                username = Request.QueryString["username"];
            }

            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();

            str_val = memberManager.getMemberByUserName(username).id.ToString();
            return str_val;
        }
    }
}

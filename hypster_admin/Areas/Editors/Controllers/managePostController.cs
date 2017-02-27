using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using hypster_tv_DAL;

namespace hypster_admin.Areas.Editors.Controllers
{
    [Authorize]
    public class managePostController : Controller
    {
        // GET: Editors/managePost
        public ActionResult Index()
        {
            newsManagement_Admin newsManager = new newsManagement_Admin();
            List<newsPost> newsPost_list = new List<newsPost>();
            newsPost_list = newsManager.GetLatestNews();
            return View(newsPost_list);
        }

        [HttpGet]
        public ActionResult AddNewPost()
        {
            newsPost newPost = new newsPost();
            return View(newPost);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddNewPost(newsPost p_newPost, HttpPostedFileBase file, string scheduled, string datetimepicker, string postgenres, string attributes)
        {
            Hypster_Entities hyDB = new Hypster_Entities();
            newsManagement_Admin newsManager_admin = new newsManagement_Admin();
            newsManagement newsManager = new newsManagement();
            if (p_newPost.post_title != null && p_newPost.post_title != "")
            {
                if (p_newPost.post_content == null || p_newPost.post_content == "")
                {
                    ModelState.AddModelError("", "Please Enter the Post Content; it is REQUIRED!!");
                }
                if (p_newPost.post_short_content == null || p_newPost.post_short_content == "")
                {
                    ModelState.AddModelError("", "Please Enter the Short Post Content; it is REQUIRED!!");
                }

                p_newPost.post_date = DateTime.Now;
                p_newPost.post_guid = p_newPost.post_title.Replace("/", "").Replace("\\", "").Replace("&", "").Replace("+", "").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace("*", "").Replace("$", "").Replace("\"", "").Replace("'", "").Replace("{", "").Replace("}", "").Replace(")", "").Replace("(", "").Replace("[", "").Replace("]", "").Replace("|", "").Replace(".", "").Replace(",", "").Replace(":", "").Replace(";", "");
                p_newPost.post_status = (int)postStatus.NoActive;
                //
                //check if post guid is exist in database
                newsPost post_check = newsManager_admin.GetPostByGUID(p_newPost.post_guid);
                if (post_check.post_id != 0 && newsManager_admin.GetPostByGUID(post_check.post_guid).post_guid != "")
                {
                    ModelState.AddModelError("", "NOT ABLE TO GENERATE POST GUID.Please choose modify title. Post with following title already exist.");
                }
                else //if post guid is unique then procceed to finish
                {
                    hyDB.newsPosts.AddObject(p_newPost);
                    hyDB.SaveChanges();
                    int id = p_newPost.post_id;
                    UploadImage(file, id);
                    if (scheduled == "Yes")
                    {
                        ScheduledPost sPost = new ScheduledPost();
                        sPost.post_id = id;
                        sPost.scheduled_date = convertDateTime(datetimepicker);
                        sPost.activated = 0;
                        hyDB.ScheduledPost.AddObject(sPost);
                        hyDB.SaveChanges();
                    }                    
                    if (attributes != "")
                    {
                        string[] attribute = attributes.Split(';');
                        foreach (string attr in attribute)
                        {
                            postNewsletter pNewsletter = new postNewsletter();
                            pNewsletter.post_id = id;
                            pNewsletter.attribute = attr;
                            hyDB.postNewsletters.AddObject(pNewsletter);
                            hyDB.SaveChanges();
                        }
                    }
                    if (postgenres.Length != 0)
                    {
                        string[] postgenre = postgenres.Split(';');
                        for (int i = 0; i < postgenre.Length; i++)
                        {
                            Post_Genre postGenre = new Post_Genre();
                            postGenre.post_id = id;
                            postGenre.genre_id = Convert.ToInt32(postgenre[i]);
                            hyDB.Post_Genre.AddObject(postGenre);
                            hyDB.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index", "managePost");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please enter post title. It must be unique from previous posts!!!");
            }

            // if no success
            return View(new newsPost());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            newsPost newsPost = new newsPost();
            newsManagement_Admin newsManager = new newsManagement_Admin();
            newsPost = newsManager.GetPostByID(id);
            ViewBag.ID = id;
            return View(newsPost);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(newsPost p_Post, HttpPostedFileBase file, string scheduled, string datetimepicker, string postgenres, string attributes)
        {
            newsManagement_Admin newsManager_admin = new newsManagement_Admin();
            newsManagement newsManager = new newsManagement();
            Hypster_Entities hyDB = new Hypster_Entities();
            ScheduledPost sPost = new ScheduledPost();
            if (p_Post.post_status == 0 && scheduled == "Yes")
            {
                try
                {
                    // newsManager.GetSchedulePostByID(p_Post.post_id) may throw ArgumentOutOfRangeException error
                    sPost = newsManager_admin.GetSchedulePostByID(p_Post.post_id);
                    sPost.scheduled_date = convertDateTime(datetimepicker);
                    sPost.activated = 0;
                    newsManager_admin.EditSPost(sPost);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // newsManager.GetSchedulePostByID(p_Post.post_id) throws ArgumentOutOfRangeException error which was occurred because the post is not having any records in the schedule post table.
                    // Therefore, one must be created.
                    Console.WriteLine(e.Message + " Scheduled Post does not exist previously for this post. Therefore, one must be created.\n\n" + e.StackTrace.ToString());
                    sPost.post_id = p_Post.post_id;
                    sPost.scheduled_date = convertDateTime(datetimepicker);
                    sPost.activated = 0;
                    hyDB.ScheduledPost.AddObject(sPost);
                }
            }
            else
            {
                try
                {
                    // newsManager.GetSchedulePostByID(p_Post.post_id) may throw ArgumentOutOfRangeException error
                    sPost = newsManager_admin.GetSchedulePostByID(p_Post.post_id);
                    sPost.activated = 1;
                    newsManager_admin.EditSPost(sPost);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // newsManager.GetSchedulePostByID(p_Post.post_id) throws ArgumentOutOfRangeException error which was occurred because the post is not having any records in the schedule post table.
                    // Therefore, one must be created.
                    Console.WriteLine(e.Message + " Scheduled Post does not exist previously for this post. Therefore, one must be created.\n\n" + e.StackTrace.ToString());
                    sPost.post_id = p_Post.post_id;
                    sPost.scheduled_date = DateTime.Now;
                    sPost.activated = 1;
                    hyDB.ScheduledPost.AddObject(sPost);
                }
            }
            manageGenres(hyDB, newsManager, p_Post.post_id, postgenres, getAssociatedGenreIds(p_Post.post_id));
            manageAttributes(hyDB, newsManager_admin, p_Post.post_id, attributes, getAttrs(p_Post.post_id));
            newsManager_admin.EditPost(p_Post);
            UploadImage(file, p_Post.post_id);
            hyDB.SaveChanges();        
            //update sitemaps date and ping google and bing
            //
            int sitemapNewsID = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["sitemap_newsID"]);

            SitemapManagement sitemapManager = new SitemapManagement();
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
            return RedirectToAction("Index");
        }        

        //------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------
        //
        //user uploads post image here
        //

        [HttpPost]
        public ActionResult UploadPostImageEdit(HttpPostedFileBase file, int post_id)
        {
            UploadImage(file, post_id);
            return RedirectToAction("Edit", new { id = post_id });
        }
        //------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------
        public ActionResult getNewsTags(int id)
        {
            TagManagement tagManager = new TagManagement();
            List<sp_Tag_GetNewsTags_Result> news_tags = new List<sp_Tag_GetNewsTags_Result>();
            news_tags = tagManager.GetNewsTags(id);
            return View(news_tags);
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

            memberManagement memberManager = new memberManagement();
            Member member = new Member();
            member = memberManager.getMemberByUserName(User.Identity.Name);

            newsManagement newsManager = new newsManagement();
            newsPost curr_article = new newsPost();
            curr_article = newsManager.GetPostByID(article_id);

            if (curr_article.post_id != 0 && article_id == curr_article.post_id)
            {
                TagManagement tagManager = new TagManagement();
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

            TagManagement tagManager = new TagManagement();
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

            memberManagement memberManager = new memberManagement();

            str_val = memberManager.getMemberByUserName(username).id.ToString();
            return str_val;
        }

        public string getPlaylistsByUsername()
        {
            string sel_list = "";
            string username = "";
            if (Request.QueryString["username"] != null)
            {
                username = Request.QueryString["username"];
            }
            memberManagement memberManager = new memberManagement();
            playlistManagement playlistManagement = new playlistManagement();
            for (int i = 0; i < playlistManagement.GetUserPlaylists(memberManager.getMemberByUserName(username).id).Count; i++)
            {
                if (sel_list != "") sel_list += ",";
                sel_list += "{\"id\":" + playlistManagement.GetUserPlaylists(memberManager.getMemberByUserName(username).id)[i].id + ", \"name\":\"" + playlistManagement.GetUserPlaylists(memberManager.getMemberByUserName(username).id)[i].name + "\"}";
            }
            return "[" + sel_list + "]";
        }

        public string getPlaylistName()
        {
            string playlistname = "";
            string playlistid = "";
            if (Request.QueryString["playlistid"] != null)
            {
                playlistid = Request.QueryString["playlistid"];
                try
                {
                    playlistManagement playlistManagement = new playlistManagement();
                    int list = playlistManagement.GetPlaylistById(Convert.ToInt32(playlistid)).Count;
                    if (list == 0)
                        return "Error: The Playlist DOES NOT EXIST!!!";
                    for (int i = 0; i < list; i++)
                    {
                        playlistname = playlistManagement.GetPlaylistById(Convert.ToInt32(playlistid))[i].name;
                    }
                }
                catch (Exception e)
                {
                    return "Error: No such Playlist. " + e.Message + "\n" + e.StackTrace;
                }
                return playlistname;
            }
            else
            {
                return "Error: The ID is null.";
            }
        }

        public string getScheduledPostInfo()
        {
            string post_id = "";
            ScheduledPost s_post = new ScheduledPost();
            if (Request.QueryString["post_id"] != null)
            {
                post_id = Request.QueryString["post_id"];                
                newsManagement_Admin newsManager = new newsManagement_Admin();
                s_post = newsManager.GetSchedulePostByID(Convert.ToInt32(post_id));
            }
            else
            {
                return "non";
            }
            if (s_post.scheduled_date > DateTime.Now)
            {
                DateTime datetime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(s_post.scheduled_date.ToString()));
                return datetime.ToString("yyyy/MM/dd hh:mm tt") + ", " + s_post.activated;
            }
            else
                return "non";
        }

        public void UploadImage(HttpPostedFileBase file, int id)
        {            
            if (file != null && file.ContentLength > 0)
            {
                newsPost p_Post = new newsPost();
                newsManagement_Admin newsManager = new newsManagement_Admin();
                Image_Resize_Manager image_resizer = new Image_Resize_Manager();
                p_Post = newsManager.GetPostByID(id);
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (file.FileName != "")
                {
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
            }
        }

        public DateTime convertDateTime(string datetime)
        {
            DateTime standard = TimeZoneInfo.ConvertTime(DateTime.Parse(datetime), TimeZoneInfo.Local);
            return standard;
        }

        public string getPostNewsletters()
        {
            int id = 0;
            string attributes = "";
            newsManagement_Admin newsManager = new newsManagement_Admin();
            List<sp_postNewsletter_GetPostAttributes_Result> attrs = new List<sp_postNewsletter_GetPostAttributes_Result>();
            if (Request.QueryString["id"] != null)
            {
                id = Convert.ToInt32(Request.QueryString["id"]);
                attrs = newsManager.GetPostAttributes_Result(id);
                for (int i = 0; i < attrs.Count; i++)
                {
                    if (attributes != "") attributes += ";";
                    attributes += attrs[i].attribute;
                }
            }
            else
            {
                return "Error: The Post ID is null.";
            }
            return attributes;
        }

        public string getAttrs(int id)
        {
            string attributes = "";
            newsManagement_Admin newsManager = new newsManagement_Admin();
            List<sp_postNewsletter_GetPostAttributes_Result> attrs = new List<sp_postNewsletter_GetPostAttributes_Result>();
            attrs = newsManager.GetPostAttributes_Result(id);
            for (int i = 0; i < attrs.Count; i++)
            {
                if (attributes != "") attributes += ";";
                attributes += attrs[i].attribute;
            }
            return attributes;
        }

        // Compare 2 attribute strings, attributes (new attributes) and old_attr (old attributes).
        public void manageAttributes(Hypster_Entities hyDB, newsManagement_Admin newsManager, int id, string attributes, string old_attr)
        {
            if (attributes != old_attr)
            {
                string[] attr = attributes.Split(';');
                for (int i = 0; i < attr.Length; i++)
                {
                    if (!old_attr.Contains(attr[i]))
                    {
                        postNewsletter pNewsletter = new postNewsletter();
                        pNewsletter.post_id = id;
                        pNewsletter.attribute = attr[i];
                        hyDB.postNewsletters.AddObject(pNewsletter);
                        hyDB.SaveChanges();
                    }
                }
                string[] o_attr = old_attr.Split(';');
                for (int j = 0; j < o_attr.Length; j++)
                {
                    if (!attributes.Contains(o_attr[j]))
                    {
                        newsManager.DeletePostAttribute(id, o_attr[j]);
                        hyDB.SaveChanges();
                    }
                }
            }
        }

        public string getAssociatedGenreIds(int id)
        {
            string genres = "";
            newsManagement newsManager = new newsManagement();
            List<int?> allGenres = new List<int?>();
            allGenres = newsManager.GetAsociatGenreIds(id);
            for (int i = 0; i < allGenres.Count; i++)
            {
                if (genres != "") genres += ";";
                genres += allGenres[i];
            }
            return genres;
        }

        public void manageGenres(Hypster_Entities hyDB, newsManagement newsManager, int id, string postgenres, string old_pgenres)
        {
            if (postgenres != old_pgenres)
            {
                string[] posgn = postgenres.Split(';');
                for (int i = 0; i < posgn.Length; i++)
                {
                    if (!old_pgenres.Contains(posgn[i]))
                    {
                        Post_Genre postGen = new Post_Genre();
                        postGen.post_id = id;
                        postGen.genre_id = Convert.ToInt32(posgn[i]);
                        hyDB.Post_Genre.AddObject(postGen);
                        hyDB.SaveChanges();
                    }
                }
                string[] o_posgn = old_pgenres.Split(';');
                for (int j = 0; j < o_posgn.Length; j++)
                {
                    if (!postgenres.Contains(o_posgn[j]))
                    {
                        int genId = Convert.ToInt32(o_posgn[j]);
                        newsManager.DeletePostGenre(id, genId);
                        hyDB.SaveChanges();
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class manageGenresController : Controller
    {
        //
        // GET: /WebsiteManagement/manageGenres/

        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.MemberMusicGenreManager genreManager = new hypster_tv_DAL.MemberMusicGenreManager();
                List<hypster_tv_DAL.MusicGenre> genres_list = new List<hypster_tv_DAL.MusicGenre>();
                genres_list = genreManager.GetMusicGenresList();
                return View(genres_list);
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult SaveGenre(int Genre_ID, string GenreName, int Playlist_ID, int User_ID, HttpPostedFileBase ImageThumb, string Username)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.MemberMusicGenreManager genreManager = new hypster_tv_DAL.MemberMusicGenreManager();
                List<hypster_tv_DAL.MusicGenre> genres_list = new List<hypster_tv_DAL.MusicGenre>();
                hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
                //--------------------------------------------------------------------
                //save here
                hypster_tv_DAL.MusicGenre GenreSave = new hypster_tv_DAL.MusicGenre();
                GenreSave.Genre_ID = Genre_ID;
                GenreSave.GenreName = GenreName;
                GenreSave.Playlist_ID = Playlist_ID;
                if (Username != "")
                {
                    GenreSave.User_ID = memberManager.getMemberByUserName(Username).id;
                }
                else
                {
                    GenreSave.User_ID = User_ID;    
                }            
                genreManager.SaveMusicGenre(GenreSave);
                //--------------------------------------------------------------------
                if (ImageThumb != null && ImageThumb.ContentLength > 0)
                {
                    var extension = System.IO.Path.GetExtension(ImageThumb.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/uploads"), "music_genre_" + GenreName + extension);
                    ImageThumb.SaveAs(path);
                    hypster_tv_DAL.Image_Resize_Manager image_resizer = new hypster_tv_DAL.Image_Resize_Manager();
                    image_resizer.Resize_Image(path, 230, 135, System.Drawing.Imaging.ImageFormat.Jpeg);
                    image_resizer.Crop_Image(path, 230, 135, System.Drawing.Imaging.ImageFormat.Jpeg);
                    System.IO.FileInfo file_slide = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "music_genre_" + GenreName + extension);
                    file_slide.CopyTo(System.Configuration.ConfigurationManager.AppSettings["MusicGenreStorage_Path"] + "\\" + GenreSave.GenreName + file_slide.Extension, true);
                    //delete file
                    System.IO.FileInfo del_file = new System.IO.FileInfo(Server.MapPath("~/uploads") + "\\" + "music_genre_" + GenreName + extension);
                    del_file.Delete();
                }
                return RedirectPermanent("/WebsiteManagement/manageGenres");
            }
            else
                return RedirectPermanent("/home/");
        }


        [HttpPost]
        public ActionResult AddNewGenre(string GenreName, int PlaylistID, string Username)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.MemberMusicGenreManager genreManager = new hypster_tv_DAL.MemberMusicGenreManager();
                hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
                hypster_tv_DAL.MusicGenre GenreSave = new hypster_tv_DAL.MusicGenre();
                GenreSave.GenreName = GenreName;
                GenreSave.Playlist_ID = PlaylistID;
                GenreSave.User_ID = memberManager.getMemberByUserName(Username).id;
                genreManager.AddMusicGenre(GenreSave);
                return RedirectPermanent("/WebsiteManagement/manageGenres");
            }
            else
                return RedirectPermanent("/home/");
        }
        

        public ActionResult DeleteGenre(int id)
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                hypster_tv_DAL.MemberMusicGenreManager genreManager = new hypster_tv_DAL.MemberMusicGenreManager();
                genreManager.DeleteGenre(id);
                return RedirectPermanent("/WebsiteManagement/manageGenres");
            }
            else
                return RedirectPermanent("/home/");
        }
    }
}

using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace hypster_admin.Areas.Editors.Controllers
{
    public class managePlaylistController : Controller
    {
        // GET: Editors/managePlaylist
        [Authorize]
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index()
        {
            hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel model = new hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel();
            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            hypster_tv_DAL.memberManagement userManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.songsManagement songsManager = new hypster_tv_DAL.songsManagement();
            model.member = memberManager.getMemberByUserName(User.Identity.Name);
            model.playlist = playlistManager.GetUserPlaylists(model.member.id);

            int playlist_id = 0;
            if (Request.QueryString["playlist_id"] != null)
                playlist_id = Convert.ToInt32(Request.QueryString["playlist_id"]);
            else
                playlist_id = model.member.active_playlist;

            foreach (var item in model.playlist)
            {
                if (item.id == playlist_id)
                {

                    ViewBag.ActivePlaylistName = item.name;
                    ViewBag.ActivePlaylistID = item.id;
                }
            }

            if (playlist_id != 0)
                model.playlistData_Song = playlistManager.GetSongsForPlayList(model.member.id, playlist_id);

            //if (model.playlistData_Song.Count == 0)
            //    model.playlistData_Song = playlistManager.GetSongsForPlayList(model.member.id, model.member.active_playlist);

            hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();
            if (playlist_id != 0)
                model.tags_list = tagManager.GetPlaylistTags(playlist_id);
            else
                model.tags_list = tagManager.GetPlaylistTags(model.member.active_playlist);
            return View(model);
        }

        public ActionResult song(string id)
        {
            if (Request.Url.AbsoluteUri.Contains("www."))
            {
                string new_url = Request.Url.AbsoluteUri.Replace("www.", "");
                return RedirectPermanent(new_url);
            }
            string song_guid = id;
            hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel model = new ViewModels.PlaylistViewModel();

            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();

            ViewBag.MEDIA_TYPE = "song";

            string SongGuid = "";
            if (song_guid != null)
            {
                SongGuid = song_guid;
                ViewBag.SongGuid = SongGuid.Replace("&", "amp;");
            }

            string SongTitle = "";
            if (Request.QueryString != null)
            {
                string[] param_arr = Request.QueryString.ToString().Split('&');
                if (param_arr.Length > 0)
                {
                    SongTitle = HttpContext.Server.UrlDecode(param_arr[0]);
                    ViewBag.SongTitle = SongTitle;
                }
            }

            hypster_tv_DAL.PlaylistData_Song song = new hypster_tv_DAL.PlaylistData_Song();
            song.YoutubeId = SongGuid;
            song.Title = SongTitle;
            //set Page Title
            ViewBag.PageTitle = song.Title + " - song";
            ViewBag.Desc = song.Title + ".";
            ViewBag.PlaylistName = song.Title;
            model.playlistData_Song.Add(song);
            if (model.playlistData_Song.Count > 0)
                ViewBag.SongGuid = model.playlistData_Song[0].YoutubeId;
            ViewBag.ShowRecommendations = 1;
            return View("MPL", model);
        }

        public ActionResult user(string id)
        {
            if (Request.Url.AbsoluteUri.Contains("www."))
            {
                string new_url = Request.Url.AbsoluteUri.Replace("www.", "");
                return RedirectPermanent(new_url);
            }

            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();

            string plst_id_str = "";

            if (Request.QueryString.Count > 0)
            {
                plst_id_str = Request.QueryString[0].ToString();
            }

            //detect curr member
            //
            hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();
            member = memberManager.getMemberByUserName(id);
            ViewBag.UserID = member.id;
            ViewBag.Username = member.username;

            hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel model = new ViewModels.PlaylistViewModel();
            hypster_tv_DAL.Playlist curr_playlist = new hypster_tv_DAL.Playlist();
            int PLAYLIST_ID = 0;
            if (plst_id_str != null && plst_id_str != "")
            {
                PLAYLIST_ID = Convert.ToInt32(plst_id_str);
            }
            else
            {
                PLAYLIST_ID = member.active_playlist;
            }

            if (ViewBag.UserID != 0 && PLAYLIST_ID != 0)
            {
                playlistManager.AddPlaylistView(PLAYLIST_ID);
                curr_playlist = playlistManager.GetUserPlaylistById(Int32.Parse(ViewBag.UserID.ToString()), PLAYLIST_ID);
                model.playlistData_Song = playlistManager.GetSongsForPlayList(Int32.Parse(ViewBag.UserID.ToString()), (int)PLAYLIST_ID);
            }
            //-----------------------------------------------------------------------------------------------------

            //-----------------------------------------------------------------------------------------------------
            //set Page Title
            string title_str = "";
            if (curr_playlist.description != "")
            {
                title_str = curr_playlist.description;
            }
            else
            {
                title_str = curr_playlist.name + " ";
            }
            ViewBag.PageTitle = title_str + "- music playlist";
            ViewBag.Desc = curr_playlist.name + " with music of " + curr_playlist.description + "...";
            ViewBag.DescLength = curr_playlist.description.Length;


            ViewBag.PlaylistID = PLAYLIST_ID;
            ViewBag.PlaylistName = curr_playlist.name;
            ViewBag.PlaylistLikes = curr_playlist.Likes;
            ViewBag.PlaylistViews = curr_playlist.ViewsNum;
            //-----------------------------------------------------------------------------------------------------

            //-----------------------------------------------------------------------------------------------------
            hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();

            List<hypster_tv_DAL.sp_Tag_GetPlaylistTags_Result> tags_list = new List<hypster_tv_DAL.sp_Tag_GetPlaylistTags_Result>();
            tags_list = tagManager.GetPlaylistTags(PLAYLIST_ID);

            string tags_string = "";
            foreach (var item in tags_list)
            {
                tags_string += item.Tag_Name + ", ";
            }


            if (tags_string != "")
            {
                ViewBag.TagsList = tags_string.Split(',');
            }
            else
            {
                ViewBag.TagsList = curr_playlist.description.Split(',');
            }
            //-----------------------------------------------------------------------------------------------------

            //--------------------------------------------------------------------------------------------
            if (member.id != 0)
            {
                model.playlist = playlistManager.GetUserPlaylists(member.id);
            }
            //--------------------------------------------------------------------------------------------

            return View("MPL", model);
        }

        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        public ActionResult AddNewPlaylist(string AddPlaylist_Name)
        {
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();

            if (AddPlaylist_Name != "")
            {
                hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();
                member = memberManager.getMemberByUserName(User.Identity.Name);

                hypster_tv_DAL.Playlist playlist = new hypster_tv_DAL.Playlist();
                playlist.name = AddPlaylist_Name;
                playlist.userid = member.id;

                string crtd = DateTime.Now.ToString("yyyyMMdd");
                int crtd_i = 0;
                Int32.TryParse(crtd, out crtd_i);
                playlist.create_time = crtd_i;

                if (playlist.name.Length > 60)
                    playlist.name = playlist.name.Substring(0, 60);


                hypster_tv_DAL.Hypster_Entities hyDB_man = new hypster_tv_DAL.Hypster_Entities();
                hyDB_man.Playlists.AddObject(playlist);
                hyDB_man.SaveChanges();

                hypster_tv_DAL.playlistManagement playlistManagement = new hypster_tv_DAL.playlistManagement();
                List<hypster_tv_DAL.Playlist> playlists_list = playlistManagement.GetUserPlaylists(member.id);
                if (member.active_playlist == 0 && playlists_list.Count > 0)
                {
                    member.active_playlist = playlists_list[0].id;
                    memberManager.SetUserDefaultPlaylist(User.Identity.Name, member.id, member.active_playlist);
                }
            }
            return RedirectPermanent("/Editors/managePlaylist/");
        }

        [HttpGet]
        public ActionResult addNewSongs()
        {
            hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel model = new ViewModels.PlaylistViewModel();

            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();

            if (User.Identity.IsAuthenticated == true)
                model.playlist = playlistManager.GetUserPlaylists(memberManager.getMemberByUserName(User.Identity.Name).id);
            ViewBag.playlist_id = Request.QueryString["playlist"].ToString();

            return View(model);
        }

        public ActionResult AddToPlayList()
        {
            if (User.Identity.IsAuthenticated == false)
                return Content("<script type='text/javascript'>window.location='/account/Login';</script>");

            hypster_tv_DAL.memberManagement userManager = new hypster_tv_DAL.memberManagement();
            hypster_admin.Areas.Editors.ViewModels.AddToPlayListViewModel model = new ViewModels.AddToPlayListViewModel();
            hypster_tv_DAL.Member curr_user = new hypster_tv_DAL.Member();
            curr_user = userManager.getMemberByUserName(User.Identity.Name);

            if (Request.QueryString["song_title"] != null)
                ViewBag.song_title = Request.QueryString["song_title"];

            if (Request.QueryString["song_guid"] != null)
                ViewBag.song_guid = Request.QueryString["song_guid"];

            if (Request.QueryString["ss"] != null)
                ViewBag.ss = Request.QueryString["ss"];


            int PLAYLIST_ID = 0;
            PLAYLIST_ID = curr_user.active_playlist;

            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            model.songs_list = playlistManager.GetSongsForPlayList(curr_user.id, PLAYLIST_ID);

            model.playlists_list = playlistManager.GetUserPlaylists(curr_user.id);

            model.curr_user = curr_user;

            foreach (var item in model.playlists_list)
            {
                if (PLAYLIST_ID == item.id)
                {
                    ViewBag.Playlist_ID = item.id;
                    ViewBag.Playlist_Name = item.name;
                }
            }

            foreach (var item in model.songs_list)
            {
                if (item.YoutubeId == ViewBag.song_guid)
                {
                    ViewBag.SongAlreadyExist = "Y";
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SubmitAddNewSong(string Song_Title, string Song_Guid, string Sel_Playlist_ID)
        {
            hypster_tv_DAL.Hypster_Entities hypDB = new hypster_tv_DAL.Hypster_Entities();
            hypster_tv_DAL.songsManagement songsManager = new hypster_tv_DAL.songsManagement();
            hypster_tv_DAL.memberManagement userManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.playlistManagement playlistManagement = new hypster_tv_DAL.playlistManagement();
            hypster_tv_DAL.Member curr_user = new hypster_tv_DAL.Member();
            curr_user = userManager.getMemberByUserName(User.Identity.Name);

            if (curr_user.active_playlist == 0)
            {
                hypster_tv_DAL.Playlist create_playlist = new hypster_tv_DAL.Playlist();
                create_playlist.name = curr_user.username + "'s playlist";
                create_playlist.userid = curr_user.id;


                string crtd = DateTime.Now.ToString("yyyyMMdd");
                int crtd_i = 0;
                Int32.TryParse(crtd, out crtd_i);
                create_playlist.create_time = crtd_i;


                if (create_playlist.name.Length > 60)
                    create_playlist.name = create_playlist.name.Substring(0, 60);


                hypDB.Playlists.AddObject(create_playlist);
                hypDB.SaveChanges();

                List<hypster_tv_DAL.Playlist> playlists_list = playlistManagement.GetUserPlaylists(curr_user.id);
                if (playlists_list.Count > 0)
                {
                    curr_user.active_playlist = playlists_list[0].id;
                    userManager.SetUserDefaultPlaylist(User.Identity.Name, curr_user.id, curr_user.active_playlist);
                }
                //else error - need to have dafult playlist
            }
            //-----------------------------------------------------------------------------------------

            //check if user selected playlist
            //-----------------------------------------------------------------------------------------
            if (Sel_Playlist_ID == null)
            {
                Sel_Playlist_ID = curr_user.active_playlist.ToString();
            }
            //-----------------------------------------------------------------------------------------

            // get last sort_number
            //-----------------------------------------------------------------------------------------
            short Sel_Sort_Order = 0;
            playlistManagement.IncrementPlaylistSongOrder(curr_user.id, Convert.ToInt32(Sel_Playlist_ID));
            //set sort order to first position
            Sel_Sort_Order = 1;
            //-----------------------------------------------------------------------------------------

            //get song by guid
            //-----------------------------------------------------------------------------------------
            hypster_tv_DAL.Song song = new hypster_tv_DAL.Song();
            song = songsManager.GetSongByGUID(Song_Guid);
            //-----------------------------------------------------------------------------------------
            if (Song_Title.Length > 160)
            {
                Song_Title = Song_Title.Substring(0, 160);
            }

            //-----------------------------------------------------------------------------------------
            if (song.id == 0) //add new song
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = System.Configuration.ConfigurationManager.AppSettings["YouTubeAPIKEY"],
                    ApplicationName = System.Configuration.ConfigurationManager.AppSettings["YouTubeAPIKEYName"]
                });

                var searchListRequest = youtubeService.Search.List("id,snippet");
                searchListRequest.Q = Song_Guid; // Replace with your search term.
                searchListRequest.MaxResults = 1;

                var searchListResponse = searchListRequest.Execute();
                SearchResult video = new SearchResult();
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            video = searchResult;
                            break;
                    }
                }

                //need to modify to add more song params
                hypster_tv_DAL.Song new_song = new hypster_tv_DAL.Song();
                new_song.Title = Song_Title;
                new_song.YoutubeId = Song_Guid;
                new_song.adddate = DateTime.Now;
                new_song.YoutubeProcessed = false;

                new_song.Author = "";
                new_song.Rating = 1;
                new_song.Syndication = 1;

                hypDB.Songs.AddObject(new_song);
                hypDB.SaveChanges();

                //get newely added song
                song = songsManager.GetSongByGUID(Song_Guid);


                //add to playlist data
                hypster_tv_DAL.PlaylistData new_playlistData = new hypster_tv_DAL.PlaylistData();
                new_playlistData.playlist_id = Convert.ToInt32(Sel_Playlist_ID);
                new_playlistData.songid = song.id;
                new_playlistData.sortid = Sel_Sort_Order;
                new_playlistData.userid = userManager.getMemberByUserName(User.Identity.Name).id;

                hypDB.PlaylistDatas.AddObject(new_playlistData);
                hypDB.SaveChanges();

            }
            else //if song exist in database
            {
                //add to playlist data
                hypster_tv_DAL.PlaylistData new_playlistData = new hypster_tv_DAL.PlaylistData();
                new_playlistData.playlist_id = Convert.ToInt32(Sel_Playlist_ID);
                new_playlistData.songid = song.id;
                new_playlistData.sortid = Sel_Sort_Order;
                new_playlistData.userid = curr_user.id;


                hypDB.PlaylistDatas.AddObject(new_playlistData);
                hypDB.SaveChanges();
            }
            return RedirectPermanent("/Editors/managePlaylist/?playlist_id=" + Sel_Playlist_ID);
        }

        [Authorize]
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0)]
        public ActionResult delelePlaylistSong()
        {
            hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel model = new hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel();
            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            hypster_tv_DAL.memberManagement userManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.songsManagement songsManager = new hypster_tv_DAL.songsManagement();
            model.member = memberManager.getMemberByUserName(User.Identity.Name);

            if (Request.QueryString["ACT"] != null)
            {

                switch (Request.QueryString["ACT"].ToString())
                {
                    case "delete_playlist":
                        int d_playlist_id = 0;
                        if (Int32.TryParse(Request.QueryString["playlist_id"], out d_playlist_id) == false)
                            d_playlist_id = 0;
                        if (d_playlist_id != 0)
                        {
                            playlistManager.Delete_Playlist(model.member.id, d_playlist_id);
                            //check if this playlist is default
                            if (model.member.active_playlist == d_playlist_id)
                            {
                                memberManager.SetUserDefaultPlaylist(User.Identity.Name, model.member.id, 0);
                            }
                            return RedirectPermanent("/Editors/managePlaylist/");
                        }
                        break;
                    case "delete_song":
                        int d_song_id = 0;
                        if (Int32.TryParse(Request.QueryString["song_id"], out d_song_id) == false)
                            d_song_id = 0;
                        string pl_id = "";
                        if (Request.QueryString["playlist_id"] != null)
                            pl_id = Request.QueryString["playlist_id"].ToString();
                        if (d_song_id != 0)
                        {
                            playlistManager.DeleteSong(model.member.id, d_song_id);
                            return RedirectPermanent("/Editors/managePlaylist/?playlist_id=" + pl_id);
                        }
                        break;
                    case "delete_song_plr":
                        int d_song_id1 = 0;
                        if (Int32.TryParse(Request.QueryString["song_id"], out d_song_id1) == false)
                            d_song_id1 = 0;

                        if (d_song_id1 != 0)
                        {
                            playlistManager.DeleteSong(model.member.id, d_song_id1);

                            if (Request.QueryString["ret_url"] == null)
                            {
                                return RedirectPermanent("/Editors/managePlaylist/");
                            }
                            else
                            {
                                return RedirectPermanent("/Editors/managePlaylist/");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            model.playlist = playlistManager.GetUserPlaylists(model.member.id);

            int playlist_id = 0;
            if (Request.QueryString["playlist_id"] != null)
            {
                if (Int32.TryParse(Request.QueryString["playlist_id"], out playlist_id) == false)
                    playlist_id = 0;
            }
            else
            {
                playlist_id = model.member.active_playlist;
            }

            foreach (var item in model.playlist)
            {
                if (item.id == playlist_id)
                {
                    ViewBag.ActivePlaylistName = item.name;
                    ViewBag.ActivePlaylistID = item.id;
                }
            }

            if (playlist_id != 0)
            {
                model.playlistData_Song = playlistManager.GetSongsForPlayList(model.member.id, playlist_id);
            }
            else
            {
                model.playlistData_Song = playlistManager.GetSongsForPlayList(model.member.id, model.member.active_playlist);
            }

            hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();
            if (playlist_id != 0)
            {
                model.tags_list = tagManager.GetPlaylistTags(playlist_id);
            }
            else
            {
                model.tags_list = tagManager.GetPlaylistTags(model.member.active_playlist);
            }

            return View(model);
        }
    }
}
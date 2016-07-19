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

            hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();
            if (playlist_id != 0)
                model.tags_list = tagManager.GetPlaylistTags(playlist_id);
            else
                model.tags_list = tagManager.GetPlaylistTags(model.member.active_playlist);

            return View(model);
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
            if (Song_Title.Length > 75)
            {
                Song_Title = Song_Title.Substring(0, 75);
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

        [Authorize]
        [System.Web.Mvc.OutputCache(NoStore = true, Duration = 0)]
        public ActionResult clonePlaylist()
        {
            hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel model = new hypster_admin.Areas.Editors.ViewModels.PlaylistViewModel();
            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            string playlistId = Request.QueryString["playlistId"];
            string playlistName = "";
            if (playlistId != "")
            {
                hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();

                if (Request.QueryString["cloneTo"] != "")
                    member = memberManager.getMemberByUserName(Request.QueryString["cloneTo"]);
                else
                    member = memberManager.getMemberByUserName(User.Identity.Name);

                if (Request.QueryString["playlistName"] != "")
                {
                    playlistName = Request.QueryString["playlistName"];
                    hypster_tv_DAL.Playlist playlist = new hypster_tv_DAL.Playlist();
                    playlist.name = playlistName;
                    playlist.userid = member.id;

                    string crtd = DateTime.Now.ToString("yyyyMMdd");
                    int crtd_i = 0;
                    Int32.TryParse(crtd, out crtd_i);
                    playlist.create_time = crtd_i;

                    if (playlist.name.Length > 60)
                        playlist.name = playlist.name.Substring(0, 60);

                    hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();
                    hyDB.Playlists.AddObject(playlist);
                    hyDB.SaveChanges();

                    List<hypster_tv_DAL.Playlist> playlists_list = playlistManager.GetUserPlaylists(member.id);
                    int clLsId = playlists_list[playlists_list.Count - 1].id;
                    int plId = Convert.ToInt32(playlistId);
                    model.playlistData_Song = playlistManager.GetPlayListDataByPlaylistID(plId);
                    for (int i = 0; i < model.playlistData_Song.Count; i++)
                    {
                        if (model.playlistData_Song[i].id != null)
                        {
                            hypster_tv_DAL.PlaylistData new_playlistData = new hypster_tv_DAL.PlaylistData();
                            new_playlistData.playlist_id = clLsId;
                            new_playlistData.songid = (int)model.playlistData_Song[i].id;
                            new_playlistData.sortid = model.playlistData_Song[i].sortid;
                            new_playlistData.userid = member.id;
                            hyDB.PlaylistDatas.AddObject(new_playlistData);
                            hyDB.SaveChanges();
                        }
                    }
                }                
                else
                {
                    Exception PlaylistIdNull = new Exception("The Playlist ID " + playlistId + " is NULL.\n\n");
                    Response.Write("Error: " + PlaylistIdNull.Message);
                }
            }
            else
            {
                Exception PlaylistIdNull = new Exception("The Playlist ID " + playlistId + " is NULL.\n\n");
                Response.Write("Error: " + PlaylistIdNull.Message);
            }
            return RedirectPermanent("/Editors/managePlaylist/");
        }

        #region TAGS_LOGIC
        //--------------------------------------------------------------------------------------------------------
        public string addnewtag()
        {
            string ret_res = "";
            string tag_name = "";
            if (Request.QueryString["tag_name"] != null)
                tag_name = Request.QueryString["tag_name"].ToString();
            
            int playlist_id = 0;
            if (Request.QueryString["playlist_id"] != null)
                Int32.TryParse(Request.QueryString["playlist_id"], out playlist_id);

            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();
            member = memberManager.getMemberByUserName(User.Identity.Name);

            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            hypster_tv_DAL.Playlist curr_plst = new hypster_tv_DAL.Playlist();
            curr_plst = playlistManager.GetUserPlaylistById(member.id, playlist_id);

            if (curr_plst.id != 0)
            {
                hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();
                int tag_ID = 0;
                tag_ID = tagManager.AddNewTag(tag_name);
                //tagManager.AddTagToPlaylist(tag_ID, playlist_id);
                ret_res = tagManager.AddTagToPlaylist(tag_ID, playlist_id).ToString();
            }
            else
                ret_res = "n/a";

            return ret_res;
        }
        //--------------------------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------------------
        public string deletePlaylistTag()
        {
            string ret_res = "";
            int tag_plst_id = 0;
            if (Request.QueryString["tag_plst_id"] != null)
                Int32.TryParse(Request.QueryString["tag_plst_id"], out tag_plst_id);

            int playlist_id = 0;
            if (Request.QueryString["playlist_id"] != null)
                Int32.TryParse(Request.QueryString["playlist_id"], out playlist_id);

            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();
            hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();
            member = memberManager.getMemberByUserName(User.Identity.Name);

            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            hypster_tv_DAL.Playlist curr_plst = new hypster_tv_DAL.Playlist();
            curr_plst = playlistManager.GetUserPlaylistById(member.id, playlist_id);

            if (curr_plst.id != 0)
            {
                hypster_tv_DAL.TagManagement tagManager = new hypster_tv_DAL.TagManagement();
                tagManager.DeletePlaylistTag(tag_plst_id);
                ret_res = "+";
            }
            else
                ret_res = "n/a";

            return ret_res.ToString();
        }
        //--------------------------------------------------------------------------------------------------------
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Google.GData;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.YouTube;
using Google.YouTube;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class manageChartsController : Controller
    {
        //
        // GET: /WebsiteManagement/manageCharts/

        public ActionResult Index()
        {
            hypster_tv_DAL.chartsManager chartManager = new hypster_tv_DAL.chartsManager();

            List<hypster_tv_DAL.Chart> charts_list = new List<hypster_tv_DAL.Chart>();
            charts_list = chartManager.GetAllCharts();

            return View(charts_list);
        }




        [HttpPost]
        public ActionResult AddNewChart(string ChartName, string ChartDesc, string ChartDate, int UserID, int PlaylistID)
        {
            hypster_tv_DAL.chartsManager chartManager = new hypster_tv_DAL.chartsManager();
            
            hypster_tv_DAL.Chart chart_add = new hypster_tv_DAL.Chart();
            chart_add.Chart_Name = ChartName;
            chart_add.Chart_Desc = ChartDesc;
            chart_add.Chart_Date = ChartDate;
            chart_add.Chart_User_ID = UserID;
            chart_add.Chart_Playlist_ID = PlaylistID;
            chart_add.Chart_GUID = chart_add.Chart_Name.Replace("/", "").Replace("\\", "").Replace("&", "").Replace("+", "").Replace(" ", "-").Replace("?", "").Replace("!", "").Replace("*", "").Replace("$", "").Replace("\"", "").Replace("'", "").Replace("{", "").Replace("}", "").Replace(")", "").Replace("(", "").Replace("[", "").Replace("]", "").Replace("|", "").Replace(".", "").Replace(",", "").Replace(":", "").Replace(";", "");


            hypster_tv_DAL.Chart check_chart = new hypster_tv_DAL.Chart();
            check_chart = chartManager.GetChartByGuid(chart_add.Chart_GUID);
            if (check_chart.Chart_ID != 0)
            {
                Random rand = new Random();
                chart_add.Chart_GUID += "_" + rand.Next(1,200000).ToString(); 
            }


            chartManager.AddNewChart(chart_add);


            return RedirectPermanent("/WebsiteManagement/manageCharts");
        }





        [HttpPost]
        public ActionResult SaveChart(int Chart_ID, int Chart_Category_ID, string Chart_Name, string Chart_Desc, string Chart_Date, int Chart_Playlist_ID, int Chart_User_ID)
        {
            hypster_tv_DAL.chartsManager chartManager = new hypster_tv_DAL.chartsManager();


            hypster_tv_DAL.Chart chart_save = new hypster_tv_DAL.Chart();
            chart_save.Chart_ID = Chart_ID;
            chart_save.Chart_Category_ID = Chart_Category_ID;
            chart_save.Chart_Name = Chart_Name;
            chart_save.Chart_Desc = Chart_Desc;
            chart_save.Chart_Date = Chart_Date;
            chart_save.Chart_Playlist_ID = Chart_Playlist_ID;
            chart_save.Chart_User_ID = Chart_User_ID;

            chartManager.SaveChart(chart_save);


            return RedirectPermanent("/WebsiteManagement/manageCharts");
        }






        public ActionResult DeleteChart(int id)
        {
            hypster_tv_DAL.chartsManager chartManager = new hypster_tv_DAL.chartsManager();


            chartManager.DeleteChart(id);


            return RedirectPermanent("/WebsiteManagement/manageCharts");
        }




        public ActionResult CloneChart(string id)
        {
            hypster_tv_DAL.chartsManager chartManager = new hypster_tv_DAL.chartsManager();

            hypster_tv_DAL.Chart chart = new hypster_tv_DAL.Chart();
            chart = chartManager.GetChartByGuid(id);

            return View(chart);
        }


        [HttpPost]
        public ActionResult AddNewCloneChart(string ChartGuid, string ChartName, string ChartDesc, string ChartDate, int UserID, string existingChartGuid)
        {
            hypster_tv_DAL.chartsManager chartManager = new hypster_tv_DAL.chartsManager();
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();

            if (chartManager.GetChartByGuid(ChartGuid).Chart_ID == 0)
            {
                //create cloned chart
                hypster_tv_DAL.Chart newClonedChart = new hypster_tv_DAL.Chart();
                newClonedChart.Chart_GUID = ChartGuid;
                newClonedChart.Chart_Name = ChartName;
                newClonedChart.Chart_Desc = ChartDesc;
                newClonedChart.Chart_Date = ChartDate;
                newClonedChart.Chart_User_ID = UserID;



                //get existing chart
                hypster_tv_DAL.Chart existing_chart = new hypster_tv_DAL.Chart();
                existing_chart = chartManager.GetChartByGuid(existingChartGuid);



                //get existing songs for cloning
                List<hypster_tv_DAL.PlaylistData_Song> existing_songs = new List<hypster_tv_DAL.PlaylistData_Song>();
                existing_songs = playlistManager.GetSongsForPlayList((int)existing_chart.Chart_User_ID, (int)existing_chart.Chart_Playlist_ID);



                //clone playlist
                hypster_tv_DAL.Playlist existing_playlist = new hypster_tv_DAL.Playlist();
                existing_playlist = playlistManager.GetUserPlaylistById((int)existing_chart.Chart_User_ID, (int)existing_chart.Chart_Playlist_ID);
                
                hypster_tv_DAL.Playlist new_playlist = new hypster_tv_DAL.Playlist();
                new_playlist.create_time = 0;
                new_playlist.is_artist_playlist = false;
                new_playlist.name = existing_playlist.name + " CLONE";
                new_playlist.update_time = 0;
                new_playlist.userid = existing_playlist.userid;
                new_playlist.ViewsNum = 0;
                new_playlist.Likes = 0;
                int new_playlist_id = playlistManager.AddNewPlaylist(new_playlist);



                //assign to chart new cloned playlist
                newClonedChart.Chart_Playlist_ID = new_playlist_id;




                //clone songs
                List<hypster_tv_DAL.PlaylistData_Song> new_songs = new List<hypster_tv_DAL.PlaylistData_Song>();
                
                hypster_tv_DAL.Hypster_Entities hypDB = new hypster_tv_DAL.Hypster_Entities();
                foreach (var item in existing_songs)
                {
                    hypster_tv_DAL.PlaylistData song = new hypster_tv_DAL.PlaylistData();
                    song.playlist_id = new_playlist_id;
                    song.userid = new_playlist.userid;
                    song.sortid = item.sortid;
                    song.songid = (int)item.id;

                    hypDB.PlaylistDatas.AddObject(song);
                    hypDB.SaveChanges();
                }



                //add new cloned chart (after playlist cloned)
                chartManager.AddNewChart(newClonedChart);
            }
            


            return RedirectPermanent("/WebsiteManagement/manageCharts");
        }






        [HttpPost]
        public ActionResult GenerateHypsterChart(int LimitNum, int CutOffID)
        {
            hypster_tv_DAL.chartsManager chart_manager = new hypster_tv_DAL.chartsManager();


            List<hypster_tv_DAL.MostPopularHypsterSongs_Result> model = new List<hypster_tv_DAL.MostPopularHypsterSongs_Result>();
            model = chart_manager.GetMostPopularHypsterSongs(LimitNum, CutOffID);


            return View(model);
        }




        [HttpPost]
        public ActionResult SaveHypsterChart(List<int> playlist_songs_list, string conv_PlaylistName, int conv_UserID)
        {
            hypster_tv_DAL.Hypster_Entities hypDB = new hypster_tv_DAL.Hypster_Entities();
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            hypster_tv_DAL.memberManagement memberManager = new hypster_tv_DAL.memberManagement();



            hypster_tv_DAL.Playlist add_playlist = new hypster_tv_DAL.Playlist();
            add_playlist.name = conv_PlaylistName;
            add_playlist.userid = conv_UserID;
            

            string crtd = DateTime.Now.ToString("yyyyMMdd");
            int crtd_i = 0;
            Int32.TryParse(crtd, out crtd_i);
            add_playlist.create_time = crtd_i;
            
            int new_plst_id = playlistManager.AddNewPlaylist(add_playlist);



            short Sel_Sort_Order = 1;
            if (playlist_songs_list != null)
            {
                foreach (int item_id in playlist_songs_list)
                {
                    //add to playlist data
                    hypster_tv_DAL.PlaylistData new_playlistData = new hypster_tv_DAL.PlaylistData();
                    new_playlistData.playlist_id = new_plst_id;
                    new_playlistData.songid = item_id;
                    new_playlistData.sortid = Sel_Sort_Order;
                    new_playlistData.userid = conv_UserID;

                    hypDB.PlaylistDatas.AddObject(new_playlistData);
                    hypDB.SaveChanges();

                    Sel_Sort_Order++;
                }
            }


            return RedirectPermanent("/WebsiteManagement/manageCharts");
        }






        public ActionResult Billboard_Year_End_Hot_100()
        {

            return View();
        }



        [HttpPost]
        public ActionResult Billboard_Year_End_Hot_100(int ChartYear)
        {
            string currURL = "http://en.wikipedia.org/wiki/Billboard_Year-End_Hot_100_singles_of_" + ChartYear.ToString();

            var strContent = "";

            var webRequest = WebRequest.Create(currURL);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                strContent = reader.ReadToEnd();
            }
            //--------------------------------------------------------------------------------------------



            //--------------------------------------------------------------------------------------------
            string new_str = "";

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(strContent);



            ViewBag.Chart_Header = "Billboard Year-End Hot 100 singles of " + ChartYear;
            ViewBag.curr_year = ChartYear;



            if (ChartYear >= 1982)
            {

                HtmlAgilityPack.HtmlNode songNode = doc.DocumentNode.SelectSingleNode("/html/body/div/div/div/table");

                if (songNode != null)
                {
                    HtmlAgilityPack.HtmlNodeCollection nodes_list = doc.DocumentNode.SelectNodes("/html/body/div/div/div/table/tr/td");

                    if (nodes_list != null)
                    {

                        int i_counter = 1;
                        for (var i = 0; i < nodes_list.Count; i += 2)
                        {
                            if ((i + 1) < nodes_list.Count)
                            {

                                string artist_song = "";
                                artist_song = nodes_list[i].InnerText + " - " + nodes_list[i + 1].InnerText;


                                string youtube_guid = "";
                                youtube_guid = GetSongByTitle(artist_song);


                                new_str += i_counter + ". " + "<img alt='' src='http://i.ytimg.com/vi/" + youtube_guid + "/0.jpg' style='width:80px;' />" + nodes_list[i].InnerText + " - " + nodes_list[i + 1].InnerText + "<br/><br/>";
                                i_counter += 1;
                            }
                        }
                    }
                }

            }
            else
            {

                HtmlAgilityPack.HtmlNode songNode = doc.DocumentNode.SelectSingleNode("/html/body/div/div/div/table");

                if (songNode != null)
                {
                    HtmlAgilityPack.HtmlNodeCollection nodes_list = doc.DocumentNode.SelectNodes("/html/body/div/div/div/table/tr/td");

                    if (nodes_list != null)
                    {

                        int i_counter = 1;
                        for (var i = 0; i < nodes_list.Count; i += 3)
                        {
                            if ((i + 2) < nodes_list.Count)
                            {

                                string artist_song = "";
                                artist_song = nodes_list[i+1].InnerText + " - " + nodes_list[i + 2].InnerText;


                                string youtube_guid = "";
                                youtube_guid = GetSongByTitle(artist_song);


                                new_str += i_counter + ". " + "<img alt='' src='http://i.ytimg.com/vi/" + youtube_guid + "/0.jpg' style='width:80px;' />" + nodes_list[i+1].InnerText + " - " + nodes_list[i + 2].InnerText + "<br/><br/>";
                                i_counter += 1;
                            }
                        }
                    }
                }

            }

            
            //--------------------------------------------------------------------------------------------





            //--------------------------------------------------------------------------------------------
            ViewBag.RetString = new_str;







            //--------------------------------------------------------------------------------------------
            return View();
        }


        private string GetSongByTitle(string song_title)
        {
            string song_guid = "";


            YouTubeRequestSettings settings = new YouTubeRequestSettings("hypster", "AI39si5TNjKgF6yiHwUhKbKwIui2JRphXG4hPXUBdlrNh4XMZLXu--lf66gVSPvks9PlWonEk2Qv9fwiadpNbiuh-9TifCNsqA");
            YouTubeRequest request = new YouTubeRequest(settings);

            string feedUrl = String.Format("http://gdata.youtube.com/feeds/api/videos?q={0}&category=Music&format=5&start-index={1}&orderby=viewCount", HttpUtility.UrlEncode(song_title.Replace("+", " ")), 1);
            Feed<Video> videoFeed = null;

            try
            {
                videoFeed = request.Get<Video>(new Uri(feedUrl));

                if (videoFeed != null)
                {
                    foreach (var item in videoFeed.Entries)
                    {
                        song_guid = item.VideoId;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }


            return song_guid;
        }



        private Video GetSongVideoByTitle(string song_title)
        {
            YouTubeRequestSettings settings = new YouTubeRequestSettings("hypster", "AI39si5TNjKgF6yiHwUhKbKwIui2JRphXG4hPXUBdlrNh4XMZLXu--lf66gVSPvks9PlWonEk2Qv9fwiadpNbiuh-9TifCNsqA");
            YouTubeRequest request = new YouTubeRequest(settings);

            string feedUrl = String.Format("http://gdata.youtube.com/feeds/api/videos?q={0}&category=Music&format=5&start-index={1}&orderby=viewCount", HttpUtility.UrlEncode(song_title.Replace("+", " ")), 1);
            Feed<Video> videoFeed = null;


            Video ret_Video = new Video();


            try
            {
                videoFeed = request.Get<Video>(new Uri(feedUrl));

                if (videoFeed != null)
                {
                    foreach (var item in videoFeed.Entries)
                    {
                        ret_Video = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }


            return ret_Video;
        }





        [HttpPost]
        public ActionResult ConvertYearToPlaylist(string plst_name, int? curr_year, int? user_id)
        {
            List<hypster_tv_DAL.PlaylistData_Song> new_songs = new List<hypster_tv_DAL.PlaylistData_Song>();

            short CURR_SORT_ID = 1;

            int CURR_USER_ID = (int)user_id;
            //-------------------------------------------------------------------------------------------------------------------------------




            //-------------------------------------------------------------------------------------------------------------------------------
            hypster_tv_DAL.playlistManagement playlistManager = new hypster_tv_DAL.playlistManagement();
            hypster_tv_DAL.songsManagement songsManager = new hypster_tv_DAL.songsManagement();
            hypster_tv_DAL.Hypster_Entities hypDB = new hypster_tv_DAL.Hypster_Entities();
            


            hypster_tv_DAL.Playlist new_playlist = new hypster_tv_DAL.Playlist();
            new_playlist.create_time = 0;
            new_playlist.is_artist_playlist = false;
            new_playlist.name = plst_name;
            new_playlist.update_time = 0;
            new_playlist.userid = (int)user_id;
            new_playlist.ViewsNum = 0;
            new_playlist.Likes = 0;
            int NEW_PLAYLIST_ID = playlistManager.AddNewPlaylist(new_playlist);
            //-------------------------------------------------------------------------------------------------------------------------------




            //-------------------------------------------------------------------------------------------------------------------------------
            string currURL = "http://en.wikipedia.org/wiki/Billboard_Year-End_Hot_100_singles_of_" + curr_year.ToString();

            var strContent = "";

            var webRequest = WebRequest.Create(currURL);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                strContent = reader.ReadToEnd();
            }
            //-------------------------------------------------------------------------------------------------------------------------------






            //-------------------------------------------------------------------------------------------------------------------------------
            string new_str = "";

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(strContent);



            ViewBag.Chart_Header = "Billboard Year-End Hot 100 singles of " + curr_year;
            ViewBag.curr_year = curr_year;
            //-------------------------------------------------------------------------------------------------------------------------------






            //-------------------------------------------------------------------------------------------------------------------------------
            if (curr_year >= 1982)
            {
                HtmlAgilityPack.HtmlNode songNode = doc.DocumentNode.SelectSingleNode("/html/body/div/div/div/table");


                if (songNode != null)
                {
                    HtmlAgilityPack.HtmlNodeCollection nodes_list = doc.DocumentNode.SelectNodes("/html/body/div/div/div/table/tr/td");

                    if (nodes_list != null)
                    {

                        int i_counter = 1;
                        for (var i = 0; i < nodes_list.Count; i += 2)
                        {
                            if ((i + 1) < nodes_list.Count)
                            {
                                // get curr song guid
                                //-----------------------------------------------------------------------------------------
                                string artist_song = "";
                                artist_song = nodes_list[i].InnerText + " - " + nodes_list[i + 1].InnerText;


                                Video CURR_VIDEO = new Video();
                                CURR_VIDEO = GetSongVideoByTitle(artist_song);
                                //-----------------------------------------------------------------------------------------



                                //get song by guid
                                //-----------------------------------------------------------------------------------------
                                hypster_tv_DAL.Song song = new hypster_tv_DAL.Song();
                                song = songsManager.GetSongByGUID(CURR_VIDEO.VideoId);
                                //-----------------------------------------------------------------------------------------



                                //-----------------------------------------------------------------------------------------
                                if (song.id == 0) //add new song
                                {

                                    if (CURR_VIDEO.Title != null && CURR_VIDEO.VideoId != null)
                                    {
                                        //need to modify to add more song params
                                        hypster_tv_DAL.Song new_song = new hypster_tv_DAL.Song();
                                        new_song.Title = CURR_VIDEO.Title;
                                        new_song.YoutubeId = CURR_VIDEO.VideoId;
                                        new_song.adddate = DateTime.Now;
                                        new_song.YoutubeProcessed = false;


                                        if (CURR_VIDEO.Author != null)
                                            new_song.Author = CURR_VIDEO.Uploader;
                                        if (CURR_VIDEO.RatingAverage != null)
                                            new_song.Rating = (float)CURR_VIDEO.RatingAverage;
                                        if (CURR_VIDEO.AppControl != null)
                                            new_song.Syndication = 1;


                                        hypDB.Songs.AddObject(new_song);
                                        hypDB.SaveChanges();





                                        //get newely added song
                                        song = songsManager.GetSongByGUID(CURR_VIDEO.VideoId);


                                        //add to playlist data
                                        hypster_tv_DAL.PlaylistData new_playlistData = new hypster_tv_DAL.PlaylistData();
                                        new_playlistData.playlist_id = NEW_PLAYLIST_ID;
                                        new_playlistData.songid = song.id;
                                        new_playlistData.sortid = CURR_SORT_ID;
                                        new_playlistData.userid = CURR_USER_ID;

                                        hypDB.PlaylistDatas.AddObject(new_playlistData);
                                        hypDB.SaveChanges();
                                    }

                                }
                                else //if song exist in database
                                {
                                    //add to playlist data
                                    hypster_tv_DAL.PlaylistData new_playlistData = new hypster_tv_DAL.PlaylistData();
                                    new_playlistData.playlist_id = NEW_PLAYLIST_ID;
                                    new_playlistData.songid = song.id;
                                    new_playlistData.sortid = CURR_SORT_ID;
                                    new_playlistData.userid = CURR_USER_ID;


                                    hypDB.PlaylistDatas.AddObject(new_playlistData);
                                    hypDB.SaveChanges();
                                }
                                //-----------------------------------------------------------------------------------------


                                
                                
                                i_counter += 1;
                                CURR_SORT_ID += 1;
                            }
                        }
                    }
                }

            }
            else
            {
                HtmlAgilityPack.HtmlNode songNode = doc.DocumentNode.SelectSingleNode("/html/body/div/div/div/table");


                if (songNode != null)
                {
                    HtmlAgilityPack.HtmlNodeCollection nodes_list = doc.DocumentNode.SelectNodes("/html/body/div/div/div/table/tr/td");

                    if (nodes_list != null)
                    {

                        int i_counter = 1;
                        for (var i = 0; i < nodes_list.Count; i += 3)
                        {
                            if ((i + 2) < nodes_list.Count)
                            {
                                // get curr song guid
                                //-----------------------------------------------------------------------------------------
                                string artist_song = "";
                                artist_song = nodes_list[i+1].InnerText + " - " + nodes_list[i + 2].InnerText;


                                Video CURR_VIDEO = new Video();
                                CURR_VIDEO = GetSongVideoByTitle(artist_song);
                                //-----------------------------------------------------------------------------------------



                                //get song by guid
                                //-----------------------------------------------------------------------------------------
                                hypster_tv_DAL.Song song = new hypster_tv_DAL.Song();
                                song = songsManager.GetSongByGUID(CURR_VIDEO.VideoId);
                                //-----------------------------------------------------------------------------------------



                                //-----------------------------------------------------------------------------------------
                                if (song.id == 0) //add new song
                                {

                                    //need to modify to add more song params
                                    hypster_tv_DAL.Song new_song = new hypster_tv_DAL.Song();
                                    new_song.Title = CURR_VIDEO.Title;
                                    new_song.YoutubeId = CURR_VIDEO.VideoId;
                                    new_song.adddate = DateTime.Now;
                                    new_song.YoutubeProcessed = false;


                                    if (CURR_VIDEO.Author != null)
                                        new_song.Author = CURR_VIDEO.Uploader;
                                    if (CURR_VIDEO.RatingAverage != null)
                                        new_song.Rating = (float)CURR_VIDEO.RatingAverage;
                                    if (CURR_VIDEO.AppControl != null)
                                        new_song.Syndication = 1;


                                    hypDB.Songs.AddObject(new_song);
                                    hypDB.SaveChanges();





                                    //get newely added song
                                    song = songsManager.GetSongByGUID(CURR_VIDEO.VideoId);


                                    //add to playlist data
                                    hypster_tv_DAL.PlaylistData new_playlistData = new hypster_tv_DAL.PlaylistData();
                                    new_playlistData.playlist_id = NEW_PLAYLIST_ID;
                                    new_playlistData.songid = song.id;
                                    new_playlistData.sortid = CURR_SORT_ID;
                                    new_playlistData.userid = CURR_USER_ID;

                                    hypDB.PlaylistDatas.AddObject(new_playlistData);
                                    hypDB.SaveChanges();

                                }
                                else //if song exist in database
                                {
                                    //add to playlist data
                                    hypster_tv_DAL.PlaylistData new_playlistData = new hypster_tv_DAL.PlaylistData();
                                    new_playlistData.playlist_id = NEW_PLAYLIST_ID;
                                    new_playlistData.songid = song.id;
                                    new_playlistData.sortid = CURR_SORT_ID;
                                    new_playlistData.userid = CURR_USER_ID;


                                    hypDB.PlaylistDatas.AddObject(new_playlistData);
                                    hypDB.SaveChanges();
                                }
                                //-----------------------------------------------------------------------------------------




                                i_counter += 1;
                                CURR_SORT_ID += 1;
                            }
                        }
                    }
                }

            }
            //-------------------------------------------------------------------------------------------------------------------------------
            




            return RedirectPermanent("/WebsiteManagement/manageCharts");
        }


    }
}

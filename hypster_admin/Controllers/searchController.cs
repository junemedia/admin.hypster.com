using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;


namespace hypster_admin.Controllers
{
    public class searchController : Controller
    {
        private int MAX_RECENT_SEARCHES_NUM = 255;

        // GET: search
        public ActionResult Index()
        {
            return View();
        }

        //public string Music()
        //{
        //    if (Request.QueryString["ss"] != null)
        //    {
        //        string search_string = Request.QueryString["ss"].ToString();
        //        ViewBag.search_string = search_string.Replace(" ", "+");

        //        string Curr_Page = "";
        //        if (Request.QueryString["page"] != null)
        //            Curr_Page = Request.QueryString["page"];

        //        string orderBy = "";
        //        if (Request.QueryString["orderBy"] != null)
        //            orderBy = Request.QueryString["orderBy"].ToString();
        //        ViewBag.orderBy = orderBy;

        //        #region save_recent_searches_to_application-varibles
        //        //save recent searches to application varibles
        //        if (HttpContext.Application["RECENT_SEARCHES"] != null)
        //        {
        //            List<string> recent_searches = (List<string>)HttpContext.Application["RECENT_SEARCHES"];
        //            recent_searches.Add(search_string);
        //            if (recent_searches.Count > MAX_RECENT_SEARCHES_NUM)
        //                recent_searches.RemoveAt(recent_searches.Count - 1);
        //            HttpContext.Application["RECENT_SEARCHES"] = recent_searches;
        //        }
        //        else
        //        {
        //            List<string> recent_searches = new List<string>();
        //            recent_searches.Add(search_string);
        //            HttpContext.Application["RECENT_SEARCHES"] = recent_searches;
        //        }

        //        if (HttpContext.Application["RECENT_SEARCHES"] != null)
        //            ViewBag.recent_searches = (List<string>)HttpContext.Application["RECENT_SEARCHES"];
        //        #endregion

        //        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        //        {
        //            ApiKey = System.Configuration.ConfigurationManager.AppSettings["YouTubeAPIKEY"],
        //            ApplicationName = System.Configuration.ConfigurationManager.AppSettings["YouTubeAPIKEYName"]
        //        });

        //        var searchListRequest = youtubeService.Search.List("snippet");
        //        searchListRequest.Q = search_string; // Replace with your search term.
        //        if (Curr_Page != "")
        //        {
        //            searchListRequest.PageToken = Curr_Page;
        //        }
        //        searchListRequest.MaxResults = 25;

        //        var searchListResponse = searchListRequest.Execute();
        //        List<SearchResult> videos = new List<SearchResult>();
        //        foreach (var searchResult in searchListResponse.Items)
        //        {
        //            switch (searchResult.Id.Kind)
        //            {
        //                case "youtube#video":
        //                    videos.Add(searchResult);
        //                    break;
        //            }
        //        }

        //        ViewBag.TotalResults = searchListResponse.PageInfo.TotalResults;
        //        ViewBag.PageSize = searchListResponse.PageInfo.ResultsPerPage;
        //        ViewBag.NextPageToken = searchListResponse.NextPageToken;
        //        ViewBag.PrevPageToken = searchListResponse.PrevPageToken;
        //        return "hi " + search_string + " " + Curr_Page + " " + orderBy;
        //    } // NEED TO CHECK AND FIX THIS SECTION
        //    else
        //    {
        //        if (HttpContext.Application["RECENT_SEARCHES"] != null)
        //            ViewBag.recent_searches = (List<string>)HttpContext.Application["RECENT_SEARCHES"];
        //        return "hi";
        //    }
        //}
        public ActionResult Music()
        {
            if (Request.QueryString["ss"] != null)
            {
                string search_string = Request.QueryString["ss"].ToString();
                ViewBag.search_string = search_string.Replace(" ", "+");

                string Curr_Page = "";
                if (Request.QueryString["page"] != null)
                    Curr_Page = Request.QueryString["page"];

                string orderBy = "";
                if (Request.QueryString["orderBy"] != null)
                {
                    orderBy = Request.QueryString["orderBy"].ToString();
                }
                ViewBag.orderBy = orderBy;

                #region save_recent_searches_to_application-varibles
                //save recent searches to application varibles
                if (HttpContext.Application["RECENT_SEARCHES"] != null)
                {
                    List<string> recent_searches = (List<string>)HttpContext.Application["RECENT_SEARCHES"];
                    recent_searches.Add(search_string);
                    if (recent_searches.Count > MAX_RECENT_SEARCHES_NUM)
                        recent_searches.RemoveAt(recent_searches.Count - 1);
                    HttpContext.Application["RECENT_SEARCHES"] = recent_searches;
                }
                else
                {
                    List<string> recent_searches = new List<string>();
                    recent_searches.Add(search_string);
                    HttpContext.Application["RECENT_SEARCHES"] = recent_searches;
                }

                if (HttpContext.Application["RECENT_SEARCHES"] != null)
                    ViewBag.recent_searches = (List<string>)HttpContext.Application["RECENT_SEARCHES"];
                #endregion

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = System.Configuration.ConfigurationManager.AppSettings["YouTubeAPIKEY"],
                    ApplicationName = System.Configuration.ConfigurationManager.AppSettings["YouTubeAPIKEYName"]
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = search_string; // Replace with your search term.
                if (Curr_Page != "")
                {
                    searchListRequest.PageToken = Curr_Page;
                }
                searchListRequest.MaxResults = 25;


                var searchListResponse = searchListRequest.Execute();
                List<SearchResult> videos = new List<SearchResult>();
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            videos.Add(searchResult);
                            break;
                    }
                }

                ViewBag.TotalResults = searchListResponse.PageInfo.TotalResults;
                ViewBag.PageSize = searchListResponse.PageInfo.ResultsPerPage;
                ViewBag.NextPageToken = searchListResponse.NextPageToken;
                ViewBag.PrevPageToken = searchListResponse.PrevPageToken;
                return View(videos);
            } // NEED TO CHECK AND FIX THIS SECTION
            else
            {
                if (HttpContext.Application["RECENT_SEARCHES"] != null)
                    ViewBag.recent_searches = (List<string>)HttpContext.Application["RECENT_SEARCHES"];
                return View();
            }
        }

        // perform youtube search
        public ActionResult MusicYTID()
        {
            string search_string = "";
            if (Request.QueryString["ss"] != null)
            {
                search_string = Request.QueryString["ss"].ToString();
                ViewBag.search_string = search_string.Replace(" ", "+");
            }

            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = System.Configuration.ConfigurationManager.AppSettings["YouTubeAPIKEY"],
                    ApplicationName = System.Configuration.ConfigurationManager.AppSettings["YouTubeAPIKEYName"]
                });

                var searchListRequest = youtubeService.Search.List("id,snippet");
                searchListRequest.Q = search_string; // Replace with your search term.
                searchListRequest.MaxResults = 1;

                var searchListResponse = searchListRequest.Execute();
                List<SearchResult> videos = new List<SearchResult>();
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            videos.Add(searchResult);
                            break;
                    }
                }
                return View(videos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace + " " + ex.Message);
            }
            return View();
        }
    }
}
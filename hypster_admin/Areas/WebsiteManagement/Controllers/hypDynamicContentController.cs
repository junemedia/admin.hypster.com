using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class hypDynamicContentController : Controller
    {
        //
        // GET: /WebsiteManagement/hypDynamicContent/

        public ActionResult Index()
        {
            hypster_tv_DAL.DynamicContent_Management dynContent_manager = new hypster_tv_DAL.DynamicContent_Management();

            List<hypster_tv_DAL.DynamicContent> model = new List<hypster_tv_DAL.DynamicContent>();


            model = dynContent_manager.GetDynamicPages();

            return View(model);
        }









        [HttpPost, ValidateInput(false)]
        public ActionResult AddNewDynPage(int dr_dynPageType, string text_BoxName, string text_BoxCont)
        {
            hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();

            hypster_tv_DAL.DynamicContent dynCont = new hypster_tv_DAL.DynamicContent();

            dynCont.DynCont_Name = text_BoxName;
            dynCont.DynCont_isNameVisible = false;

            dynCont.DynCont_BoxCont = text_BoxCont;
            dynCont.DynCont_ButtonText = "Listen Now";
            dynCont.DynCont_Href = "http://hypster.com";
            dynCont.DynCont_isNewWindow = false;
            dynCont.DynCont_PageType = dr_dynPageType;
           

            hyDB.DynamicContents.AddObject(dynCont);
            hyDB.SaveChanges();


            return RedirectPermanent("/WebsiteManagement/hypDynamicContent");
        }








        
        [HttpPost, ValidateInput(false)]
        public ActionResult AddNewBanerPage(string text_AddNewBaner_BanerName, string text_AddNewBaner_href, string text_AddNewBaner_image)
        {
            hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();

            hypster_tv_DAL.DynamicContent dynCont = new hypster_tv_DAL.DynamicContent();

            dynCont.DynCont_Name = text_AddNewBaner_BanerName;
            dynCont.DynCont_isNameVisible = false;

            dynCont.DynCont_BoxCont = generate_Baner(text_AddNewBaner_image, text_AddNewBaner_href);
            dynCont.DynCont_ButtonText = "Listen Now";
            dynCont.DynCont_Href = text_AddNewBaner_href;
            dynCont.DynCont_isNewWindow = false;
            dynCont.DynCont_PageType = 1;
           

            hyDB.DynamicContents.AddObject(dynCont);
            hyDB.SaveChanges();

            return RedirectPermanent("/WebsiteManagement/hypDynamicContent");
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AddNewChartPage(string text_AddNewChart_ChartName, string text_AddNewChart_chartGuid)
        {
            hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();

            hypster_tv_DAL.DynamicContent dynCont = new hypster_tv_DAL.DynamicContent();

            dynCont.DynCont_Name = text_AddNewChart_ChartName;
            dynCont.DynCont_isNameVisible = false;

            dynCont.DynCont_BoxCont = generate_Chart(text_AddNewChart_chartGuid);
            dynCont.DynCont_ButtonText = "Listen Now";
            dynCont.DynCont_Href = "";
            dynCont.DynCont_isNewWindow = false;
            dynCont.DynCont_PageType = 1;


            hyDB.DynamicContents.AddObject(dynCont);
            hyDB.SaveChanges();


            return RedirectPermanent("/WebsiteManagement/hypDynamicContent");
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AddNewPostPage(string text_AddNewPost_PostName, string text_AddNewPost_postGuid)
        {
            hypster_tv_DAL.Hypster_Entities hyDB = new hypster_tv_DAL.Hypster_Entities();

            hypster_tv_DAL.DynamicContent dynCont = new hypster_tv_DAL.DynamicContent();

            dynCont.DynCont_Name = text_AddNewPost_PostName;
            dynCont.DynCont_isNameVisible = false;

            dynCont.DynCont_BoxCont = generate_PostPreview(text_AddNewPost_postGuid);
            dynCont.DynCont_ButtonText = "Listen Now";
            dynCont.DynCont_Href = "";
            dynCont.DynCont_isNewWindow = false;
            dynCont.DynCont_PageType = 1;


            hyDB.DynamicContents.AddObject(dynCont);
            hyDB.SaveChanges();


            return RedirectPermanent("/WebsiteManagement/hypDynamicContent");
        }





        public ActionResult DeleteDynPage()
        {
            int dynContID = 0;

            if(Request.QueryString["ID"] != null)
            {
                hypster_tv_DAL.DynamicContent_Management dynManager = new hypster_tv_DAL.DynamicContent_Management();
                Int32.TryParse(Request.QueryString["ID"].ToString(), out dynContID);

                if (dynContID > 0)
                {
                    dynManager.DeleteDynPage(dynContID);
                }
            }

            return RedirectPermanent("/WebsiteManagement/hypDynamicContent");
        }







        private string generate_Baner(string image_src, string href)
        {
            string cont_str = "";

            cont_str += "<div class=\"boxContRight\">";
            cont_str += "<div class=\"slideRight\" \" min-height:250px; background-color:#202020;\">";
            cont_str += "<div class=\"chartHoldCont\">";

            cont_str += "<img alt=\"\" src=\"/imgs/home/home_slideshow/" + image_src + "\" onclick=\"window.location='" + href + "';\" style=\" float:left; margin:0px 0 0 0; cursor:pointer;\" />";

            cont_str += "<a onclick=\"#\" class=\"exploreMoreBtn\">";
            cont_str += "<iframe src=\"http://feed-rt.baronsoffers.com/offer/feed/q/aT0zNTQ4LHM9MzAweDI1MCxuPWlmcmFtZQ==?subid=default\" width=\"300\" height=\"250\" scrolling=\"no\" marginwidth=\"0\" frameborder=\"0\"></iframe>";
            cont_str += "</a>";

            cont_str += "</div>";
            cont_str += "</div>";
            cont_str += "</div>";

            return cont_str;
        }


        private string generate_Chart(string chart_guid)
        {
            string cont_str = "";
            cont_str += "<div class=\"boxContRight\">";
            cont_str += "<div class=\"slideRight\" style=\" min-height:250px; background-color:#202020;\">";
            cont_str += "<div id=\"chart" + chart_guid + "\"></div>";


            cont_str += "<script type=\"text/javascript\">";
            cont_str += "$.ajax({";
            cont_str += "type: \"POST\",";
            cont_str += "url: \"/popular/PopularCharts_GetChartByGuid/" + chart_guid + "\",";
            cont_str += "async: true,";
            cont_str += "success: function (data) {";
            cont_str += "$(\'#chart" + chart_guid + "\').html(data);";
            cont_str += "}";
            cont_str += "});";
            cont_str += "</script>";

            cont_str += "</div>";
            cont_str += "</div>";

            return cont_str;
        }


        private string generate_PostPreview(string post_guid)
        {
            string cont_str = "";
            cont_str += "<div class=\"boxContRight\">";
            cont_str += "<div class=\"slideRight\" style=\" min-height:250px; background-color:#202020;\">";
            cont_str += "<div id=\"post" + post_guid + "\"></div>";


            cont_str += "<script type=\"text/javascript\">";
            cont_str += "$.ajax({";
            cont_str += "type: \"POST\",";
            cont_str += "url: \"/content/HypNews/PostPreview/" + post_guid + "\",";
            cont_str += "async: true,";
            cont_str += "success: function (data) {";
            cont_str += "$(\'#post" + post_guid + "\').html(data);";
            cont_str += "}";
            cont_str += "});";
            cont_str += "</script>";

            cont_str += "</div>";
            cont_str += "</div>";

            return cont_str;
        }





    }
}

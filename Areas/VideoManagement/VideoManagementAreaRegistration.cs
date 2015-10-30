using System.Web.Mvc;

namespace hypster_admin.Areas.VideoManagement
{
    public class VideoManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "VideoManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                "ManageVideo_default",
                "VideoManagement/uploadedVideo/{video_guid}",
                new { controller = "uploadedVideo", action = "Edit", video_guid = UrlParameter.Optional }
            );
            

            context.MapRoute(
                "VideoManagement_default",
                "VideoManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );


            

        }
    }
}

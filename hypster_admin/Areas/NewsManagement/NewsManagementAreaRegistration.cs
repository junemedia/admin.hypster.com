using System.Web.Mvc;

namespace hypster_admin.Areas.NewsManagement
{
    public class NewsManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NewsManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "NewsManagement_default",
                "NewsManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

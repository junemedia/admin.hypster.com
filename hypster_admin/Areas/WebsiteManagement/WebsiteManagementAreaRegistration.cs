using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement
{
    public class WebsiteManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WebsiteManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WebsiteManagement_default",
                "WebsiteManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "WebsiteManagement_user",
                "WebsiteManagement/{controller}/user/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "WebsiteManagement_editContactUs",
                "WebsiteManagement/{controller}/editContactUs/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}

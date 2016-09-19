using System.Web.Mvc;

namespace hypster_admin.Areas.Administrators
{
    public class AdministratorsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Administrators";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Administrators_default",
                "Administrators/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Administrators_user",
                "Administrators/{controller}/user/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
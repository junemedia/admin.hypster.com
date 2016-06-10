using System.Web.Mvc;

namespace hypster_admin.Areas.Editors
{
    public class EditorsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Editors";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Editors_default",
                "Editors/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
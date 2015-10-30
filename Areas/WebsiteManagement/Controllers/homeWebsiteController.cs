using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.WebsiteManagement.Controllers
{
    [Authorize]
    public class homeWebsiteController : Controller
    {
        //
        // GET: /WebsiteManagement/homeWebsite/

        public ActionResult Index()
        {
            return View();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.VideoManagement.Controllers
{
    [Authorize]
    public class homeVideoController : Controller
    {
        //
        // GET: /VideoManagement/homeVideo/

        public ActionResult Index()
        {
            return View();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.VideoManagement.Controllers
{
    [Authorize]
    public class pendingVideoController : Controller
    {
        //
        // GET: /VideoManagement/pendingVideo/

        public ActionResult Index()
        {
            return View();
        }

    }
}

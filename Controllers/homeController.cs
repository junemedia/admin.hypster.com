using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Controllers
{
    [Authorize]
    public class homeController : Controller
    {
        //
        // GET: /home/

        
        public ActionResult Index()
        {
            return View();
        }

    }
}

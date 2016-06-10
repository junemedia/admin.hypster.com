using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hypster_admin.Areas.Editors.Controllers
{
    [Authorize]
    public class editorController : Controller
    {
        // GET: Editors/Editors
        public ActionResult Index()
        {
            return View();
        }
    }
}
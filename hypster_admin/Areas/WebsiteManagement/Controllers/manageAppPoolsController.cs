using System.Web.Mvc;
using Microsoft.Web.Administration;
using System.Threading;


namespace hypster_admin.Areas.WebsiteManagement.Controllers
{

    [Authorize]
    public class manageAppPoolsController : Controller
    {
        //
        // GET: /WebsiteManagement/manageAppPools/



        
        public ActionResult Index()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
                return View();
            else
                return RedirectPermanent("/home/");
        }


        public ActionResult StartApplicationPool()
        {
            if (Session["Roles"] != null && Session["Roles"].Equals("Admin"))
            {
                ServerManager serverManager = new ServerManager();
                ApplicationPool apppool = serverManager.ApplicationPools[System.Configuration.ConfigurationManager.AppSettings["appPoolName"]];
                serverManager.CommitChanges();
                apppool.Start();
            }
            return RedirectPermanent("/home");
        }


        [AllowAnonymous]
        public string StartApplicationPoolComm()
        {
            string IP_Address;
            IP_Address = Request.ServerVariables["HTTP_X_FORWARDED_FOR_ADMIN"];
            if (IP_Address == null)
                IP_Address = Request.ServerVariables["REMOTE_ADDR_ADMIN"];
            else
                IP_Address = "";

            Thread.Sleep(3000);

            if (IP_Address == "216.240.146.2")
            {
                ServerManager serverManager = new ServerManager();
                ApplicationPool apppool = serverManager.ApplicationPools[System.Configuration.ConfigurationManager.AppSettings["appPoolName"]];
                serverManager.CommitChanges();
                apppool.Start();
            }
            return "DONE RESTART";
        }
    }
}

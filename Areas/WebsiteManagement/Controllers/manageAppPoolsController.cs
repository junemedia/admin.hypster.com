using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
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
            


            return View();
        }



        public ActionResult StartApplicationPool()
        {
            ServerManager serverManager = new ServerManager();

            ApplicationPool apppool = serverManager.ApplicationPools[System.Configuration.ConfigurationManager.AppSettings["appPoolName"]];
            serverManager.CommitChanges();
            apppool.Start();

            return RedirectPermanent("/home");
        }




        [AllowAnonymous]
        public string StartApplicationPoolComm()
        {
            string IP_Address;
            IP_Address = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (IP_Address == null)
                IP_Address = Request.ServerVariables["REMOTE_ADDR"];
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




        /*
        [ModuleServiceMethod(PassThrough = true)]
        public ArrayList GetApplicationPoolCollection()
        {
            // Use an ArrayList to transfer objects to the client.
            ArrayList arrayOfApplicationBags = new ArrayList();

            ServerManager serverManager = new ServerManager();
            ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;
            foreach (ApplicationPool applicationPool in applicationPoolCollection)
            {
                PropertyBag applicationPoolBag = new PropertyBag();
                applicationPoolBag[ServerManagerDemoGlobals.ApplicationPoolArray] = applicationPool;
                arrayOfApplicationBags.Add(applicationPoolBag);
                // If the applicationPool is stopped, restart it.
                if (applicationPool.State == ObjectState.Stopped)
                {
                    applicationPool.Start();
                }

            }

            // CommitChanges to persist the changes to the ApplicationHost.config.
            serverManager.CommitChanges();
            return arrayOfApplicationBags;
        }
        */


    }
}

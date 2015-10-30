using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hypster_admin.Areas.WebsiteManagement.ViewModels
{
    public class ManualViewModel
    {
        public hypster_tv_DAL.Manual manual = new hypster_tv_DAL.Manual();
        public List<hypster_tv_DAL.Manual_Slide> manual_slides = new List<hypster_tv_DAL.Manual_Slide>();

        public ManualViewModel()
        {
        }

    }

}
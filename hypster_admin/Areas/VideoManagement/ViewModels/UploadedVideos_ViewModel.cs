using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hypster_admin.Areas.VideoManagement.ViewModels
{
    public class UploadedVideos_ViewModel
    {
        public List<hypster_tv_DAL.videoClip> videoClips_list = new List<hypster_tv_DAL.videoClip>();
        public hypster_tv_DAL.videoClip edit_clip = new hypster_tv_DAL.videoClip();

        public UploadedVideos_ViewModel()
        {
        }
    }
}
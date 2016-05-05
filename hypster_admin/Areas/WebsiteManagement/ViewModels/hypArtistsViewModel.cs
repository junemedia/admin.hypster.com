using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hypster_admin.Areas.WebsiteManagement.ViewModels
{
    public class hypArtistsViewModel
    {
        public List<hypster_tv_DAL.VisualSearch> visualSearch = new List<hypster_tv_DAL.VisualSearch>();
        public List<hypster_tv_DAL.MusicGenre> genres = new List<hypster_tv_DAL.MusicGenre>();
        

        public hypArtistsViewModel()
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hypster_admin.Areas.Editors.ViewModels
{
    public class PlaylistViewModel
    {
        public hypster_tv_DAL.Member member = new hypster_tv_DAL.Member();
        public List<hypster_tv_DAL.Playlist> playlist = new List<hypster_tv_DAL.Playlist>();
        public List<hypster_tv_DAL.PlaylistData_Song> playlistData_Song = new List<hypster_tv_DAL.PlaylistData_Song>();
        public List<hypster_tv_DAL.sp_Tag_GetPlaylistTags_Result> tags_list = new List<hypster_tv_DAL.sp_Tag_GetPlaylistTags_Result>();
        public PlaylistViewModel()
        {
        }
    }
}
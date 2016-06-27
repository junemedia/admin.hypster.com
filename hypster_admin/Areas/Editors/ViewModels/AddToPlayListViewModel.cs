using System.Collections.Generic;

namespace hypster_admin.Areas.Editors.ViewModels
{
    public class AddToPlayListViewModel
    {
        public List<hypster_tv_DAL.PlaylistData_Song> songs_list = new List<hypster_tv_DAL.PlaylistData_Song>();
        public List<hypster_tv_DAL.Playlist> playlists_list = new List<hypster_tv_DAL.Playlist>();
        public hypster_tv_DAL.Member curr_user = new hypster_tv_DAL.Member();
        public AddToPlayListViewModel()
        {
        }
    }
}
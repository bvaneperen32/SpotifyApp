namespace SpotifyApp.Models
{
    public class UserProfileViewModel
    {
        public PlaylistsViewModel Playlists { get; set; }
        public RecTracksViewModel RecTracks { get; set; }
        public string ProfileImageUrl { get; set; }
        public string DisplayName { get; set; }
    }
}

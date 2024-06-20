namespace SpotifyApp.Models
{
    public class PlaylistDetailsViewModel
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string SpotifyUrl { get; set; }
        public List<Track> Tracks { get; set; }
    }
}

namespace SpotifyApp.Models
{
    public class Track
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Popularity { get; set; }
        public List<string> DominantColors { get; set; }
        public string ImageUrl { get; set; }
        public string SongUrl { get; set; }
        public DateOnly DateAdded { get; set; }
    }
}

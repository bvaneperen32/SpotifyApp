namespace SpotifyApp.Models
{
    public class ArtistInfo
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Popularity { get; set; }
        public IList<string> Genres { get; set; }
        public int Followers { get; set; }
        public string SpotifyUrl { get; set; }
        public List<string> DominantColors { get; set; }
    }
}

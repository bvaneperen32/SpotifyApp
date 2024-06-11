using SpotifyAPI.Web;
using SpotifyApp.Models;

namespace SpotifyApp.Services
{
    public class SpotifyService
    {
        private readonly SpotifyClient _spotify;

        public SpotifyService(PKCETokenResponse tokenResponse)
        {
            var authenticator = new PKCEAuthenticator("YourClientId", tokenResponse);
            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);
            _spotify = new SpotifyClient(config);
        }

        public async Task<TopArtistsViewModel> GetTopArtists()
        {
            var topArtists = await _spotify.Personalization.GetTopArtists();

            var artistsInfos = topArtists.Items.Select(artist => new ArtistInfo
            {
                Name = artist.Name,
                ImageUrl = artist.Images.FirstOrDefault()?.Url,
                Popularity = artist.Popularity,
                Genres = artist.Genres,
                Followers = artist.Followers.Total,
                SpotifyUrl = artist.ExternalUrls["spotify"]
            }).ToList();

            return new TopArtistsViewModel
            {
                Artists = artistsInfos
            };
        }
    }
}

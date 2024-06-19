using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyApp.Services;
using ColorThiefDotNet;
using SpotifyApp.Models;

namespace SpotifyApp.Controllers
{
    public class SpotifyController : Controller
    {
        private SpotifyService _spotifyService;

        public SpotifyController(IHttpContextAccessor httpContextAccessor)
        {
            var tokenResponseJson = httpContextAccessor.HttpContext.Session.GetString("Token");
            var tokenResponse = JsonConvert.DeserializeObject<PKCETokenResponse>(tokenResponseJson);
            _spotifyService = new SpotifyService(tokenResponse);
        }

        public async Task<IActionResult> TopArtists()
        {
            var viewModel = await _spotifyService.GetTopArtists();
            return View(viewModel);
        }

        public async Task<IActionResult> Profile()
        {
            var currentUser = await _spotifyService.GetCurrentUserProfile();
            var playlists = await _spotifyService.GetPlaylists();
            var recTracks = await _spotifyService.GetRecommendations();

            HttpContext.Session.SetString("Username", currentUser.DisplayName);
            HttpContext.Session.SetString("ProfileImageUrl", currentUser.Images.FirstOrDefault()?.Url);

            var userProfileViewModel = new UserProfileViewModel
            {
                Playlists = playlists,
                RecTracks = recTracks,
                ProfileImageUrl = currentUser.Images.FirstOrDefault()?.Url,
                DisplayName = currentUser.DisplayName
            };

            return View(userProfileViewModel);
        }

        public async Task<IActionResult> TopTracks()
        {
            var viewModel = await _spotifyService.GetTopTracks();
            return View(viewModel);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyApp.Services;
using ColorThiefDotNet; 

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
            ViewBag.UserName = currentUser.DisplayName;
           
            return View(currentUser);
        }
    }
}

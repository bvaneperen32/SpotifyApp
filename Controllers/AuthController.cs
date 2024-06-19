using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyApp.Services;

namespace SpotifyApp.Controllers
{
    public class AuthController : Controller
    {

        [Route("login")]
        public async Task<IActionResult> Login()
        {
            var (verifier, challenge) = PKCEUtil.GenerateCodes();
            HttpContext.Session.SetString("verifier", verifier);

            var loginRequest = new LoginRequest(
              new Uri("https://localhost:7118/callback"),
              "a97145a9397d4a96b7e681552779c5fb",
              LoginRequest.ResponseType.Code
            )
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserTopRead }
            };
            var uri = loginRequest.ToUri();

            return Redirect(uri.ToString());
        }

        [Route("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code) || HttpContext.Session.GetString("Token") != null)
            {
                return RedirectToAction("Profile", "Spotify");
            }

            var codeVerifier = HttpContext.Session.GetString("verifier");

            var tokenRequest = new PKCETokenRequest(
                "a97145a9397d4a96b7e681552779c5fb",
                code,
                new Uri("https://localhost:7118/callback"),
                codeVerifier
            );

            var tokenResponse = await new OAuthClient().RequestToken(tokenRequest);
            HttpContext.Session.SetString("Token", JsonConvert.SerializeObject(tokenResponse));

            var authenticator = new PKCEAuthenticator("a97145a9397d4a96b7e681552779c5fb", tokenResponse);
            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);
            var spotify = new SpotifyClient(config);

            var currentUser = await spotify.UserProfile.Current();
            HttpContext.Session.SetString("UserName", currentUser.DisplayName);

            return RedirectToAction("Profile", "Spotify"); 
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace SpotifyApp.Controllers
{
    public class SpotifyController : Controller
    {

        [Route("login")]
        public async Task<IActionResult> Login()
        {
            var (verifier, challenge) = PKCEUtil.GenerateCodes(); 
            TempData["verifier"] = verifier;    

            var loginRequest = new LoginRequest(
              new Uri("https://localhost:7118/callback"),
              "a97145a9397d4a96b7e681552779c5fb",
              LoginRequest.ResponseType.Code
            )
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative }
            };
            var uri = loginRequest.ToUri();

            return Redirect(uri.ToString());
        }

        [Route("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var codeVerifier = TempData["verifier"] as string;

            var tokenRequest = new PKCETokenRequest(
                "a97145a9397d4a96b7e681552779c5fb",
                code,
                new Uri("https://localhost:7118/callback"),
                codeVerifier
            );

            var tokenResponse = await new OAuthClient().RequestToken(tokenRequest);

            var spotify = new SpotifyClient(tokenResponse.AccessToken);

            var currentUser = await spotify.UserProfile.Current();
            ViewBag.UserName = currentUser.DisplayName;

            return View("Profile"); 
        }

    }
}

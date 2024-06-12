using Microsoft.AspNetCore.Mvc;

namespace SpotifyApp.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

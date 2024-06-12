using Microsoft.AspNetCore.Mvc;

namespace SpotifyApp.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }   
}

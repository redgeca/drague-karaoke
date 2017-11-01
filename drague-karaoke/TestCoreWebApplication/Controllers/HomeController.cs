using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TestCoreWebApplication.Models;

namespace TestCoreWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            String singerNameCookie = Request.Cookies["SingerName"];

            SongRequest songRequest = new SongRequest();
            songRequest.singerName = singerNameCookie;
            songRequest.songId = 1;

            return View("Index", songRequest);
        }

        public ActionResult Autocomplete()
        {
            var items = new[] { "Apple", "Pear", "Banana", "Pineapple", "Peach" };

            var filteredItems = items.Where(
                item => item.IndexOf("a", StringComparison.CurrentCultureIgnoreCase) >= 0
                );
            return Json(filteredItems);
        }

        public ActionResult Autocomplete(string term)
        {
            var items = new[] { "Apple", "Pear", "Banana", "Pineapple", "Peach" };

            var filteredItems = items.Where(
                item => item.IndexOf(term, StringComparison.CurrentCultureIgnoreCase) >= 0
                );
            return Json(filteredItems);
        }

        [HttpPost]
        public ActionResult SubmitBtn(SongRequest songRequest)
        {
            songRequest.songId = 100;
            if (ModelState.IsValid)
            {
                string singerName = songRequest.singerName;

                ModelState.Clear();

                ViewData["SubmitSong"] = "Demande effectuée avec succès à " + String.Format("{0:HH:mm:ss}", DateTime.Now);
                CookieOptions cookieOption = new CookieOptions();

                cookieOption.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("SingerName", singerName, cookieOption);

                SongRequest newRequest = new SongRequest();
                newRequest.singerName = singerName;

                return View("Index", newRequest);
            }

            return View("Index");
        }
    }
}

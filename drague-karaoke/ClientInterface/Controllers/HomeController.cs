using ClientInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientInterface.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Song song = new Song { mId = 1, mTitle = "Poker Face", mContent = "Réduction de vitesse de moitié" };
            return View(song);
        }

        public ActionResult Ajouter(Song pSong)
        {
            pSong.mId++;
            return View(pSong);
        }
    }
}
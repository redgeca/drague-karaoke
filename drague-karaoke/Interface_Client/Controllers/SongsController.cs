using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Interface_Client.Models;

namespace Interface_Client.Controllers
{
    public class SongsController : ApiController
    {
       
        Songs[] songs = new Songs[]
        {
            new Songs { Id = 1, m_Name = "Poker Face", m_Artist = "Lady Gaga", m_Category = "Pop", m_Notes = "Réduction de vitesse de moitié"},
            new Songs { Id = 2, m_Name = "Whine up", m_Artist = "Kat de Luna", m_Category = "Pop", m_Notes = ""},
            new Songs { Id = 3, m_Name = "StarLight", m_Artist = "Muse", m_Category = "Rock", m_Notes = "Double la vitesse"},
        };

        /*GetAllSongs
          This method returns all songs recordes in the list/Cette méthode retourne toutes les chansons enregistrés dans la liste
          */
        public IEnumerable<Songs> GetAllSongs()
        {
            return songs;
        }
        /*GetProduct
         * Parameters/paramètres
         * id : int
       This method returns a specific song from the list/Cette méthode retourne une chanson spécifique dans la liste
       */
        public IHttpActionResult GetSong(int id)
        {
            var soloSong = songs.FirstOrDefault((p) => p.Id == id);
            if (soloSong == null)
            {
                return NotFound();
            }
            return Ok(soloSong);
        }
    }
}

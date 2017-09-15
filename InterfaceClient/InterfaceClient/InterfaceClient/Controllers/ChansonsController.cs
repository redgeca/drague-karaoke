using InterfaceClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace ProductsApp.Controllers
{
    public class ProductsController : ApiController
    {
        /*
         * Hard code temporaire qui devra être retiré lorsque la base de donnée sera prête.
         * 
         * */
        Chansons[] chansons = new Chansons[]
        {
            new Chansons { Id = 1, m_Name = "Poker Face", m_Artist = "Lady Gaga", m_Category = "Pop", m_Notes = "Réduction de vitesse de moitié"},
            new Chansons { Id = 2, m_Name = "Whine up", m_Artist = "Kat de Luna", m_Category = "Pop", m_Notes = ""},
            new Chansons { Id = 3, m_Name = "StarLight", m_Artist = "Muse", m_Category = "Rock", m_Notes = "Double la vitesse"},
        };

        /*GetAllSongs
          This method returns all songs recordes in the list/Cette méthode retourne toutes les chansons enregistrés dans la liste
          */  
        public IEnumerable<Chansons> GetAllSongs()
        {
            return chansons;
        }
        /*GetProduct
         * Parameters/paramètres
         * id : int
       This method returns a specific song from the list/Cette méthode retourne une chanson spécifique dans la liste
       */
        public IHttpActionResult GetSong(int id)
        {
            var chansonIndividuel = chansons.FirstOrDefault((p) => p.Id == id);
            if (chansonIndividuel == null)
            {
                return NotFound();
            }
            return Ok(chansonIndividuel);
        }
    }
}

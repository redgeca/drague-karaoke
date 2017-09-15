using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Diagnostics;
namespace InterfaceClient.Models
{
    /*
        * Constructeur par initialisation
        * 
        * Paramètres/parameters
        * 
        * m_Id : L'identifiant de la chanson/the id of the song
        * m_Name : Le nom de la chanson/name of the song
        * m_Artist : Le nom de l'artiste/name of the song's artist
        * m_Category : La catégorie de la chanson/the category of the song
        * m_Notes : Les notes additionnels du consommateur/additional notes the users would want to integrate
        * */

    public class Chansons
    {
        public int Id { get; set; }
        public string m_Name { get; set; }
        public string m_Artist { get; set; }
        public string m_Category { get; set; }
        public string m_Notes { get; set; }
    
    }

    
}
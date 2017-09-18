using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Diagnostics;
namespace KaraokeObjectsLibrary
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class Song
    {
        [JsonProperty("ID")]
        public long mId { get; set; }

        [JsonProperty("title")]
        public String mTitle { get; set; }

        [JsonProperty("artist")]
        public String m_artist { get; set; }


        [JsonProperty("categories.ID")]
        public long mCategory { get; set; }

        [Key]
        public Guid uniqueKey { get; set; }


        /*
         * songTitleReturn
         * return a list of songs that contains the value of "p_title"
         * 
         * Parameters
         * 
         * List<song> p_songList
         * string p_title
         * 
         * Return a list of songs at the end
         *
         */

        public List<Song> containsTitle(List<Song> p_songList, string p_title)
        {
            Debug.Assert(p_songList != null, "La liste de chanson doit avoir au moins une entrée valide");
            Debug.Assert(p_title != "", "Le titre doit avoir un minimum d'un caractère");
            List<Song> songList = new List<Song>();
            foreach (Song songSuccess in p_songList)
                if (songSuccess.mTitle.Contains(p_title))
                {
                    songList.Add(songSuccess);
                }

            return songList;
        }

        /*
        * songTitleReturn
        * return a list of songs that contains the value of "p_title"
        * 
        * Parameters
        * 
        * List<song> p_songList
        * string p_title
        * 
        * Return a list of songs at the end
        *
        */

        public List<Song> containsArtist(List<Song> p_songList, string p_artist)
        {
            Debug.Assert(p_songList != null, "La liste de chanson doit avoir au moins une entrée valide");
            Debug.Assert(p_artist != "", "Le nom de l'artiste doit avoir un minimum d'un caractère");
            List<Song> songList = new List<Song>();
            foreach (Song songSuccess in p_songList)
                if (songSuccess.mTitle.Contains(p_artist))
                {
                    songList.Add(songSuccess);
                }

            return songList;
        }
        /*
        * toString
        * 
        * return a string version of a song list
        *
        * Paremeters
        * 
        * p_songList
        * 
        */

        public  string ToString(List<Song> p_songList)
        {
            string temp = string.Empty;
            
            foreach (Song s in p_songList)
            {

                temp = "Titre : " + s.mTitle + " Artiste : " + s.m_artist +  " Categorie : " + s.mCategory;

            }
            return temp;
        }


    }
}

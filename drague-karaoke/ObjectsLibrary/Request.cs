using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace KaraokeObjectsLibrary
{
    class Request
    {
        public Song mSong { get; set; }
        public String mNom { get; set; }
        public String mNote { get; set; }
        /*
         * AddRequest
         * 
         * this method allow to create a request
         * 
         * Parameters
         * 
         * string p_nom
         * string p_note
         * Song p_song
         */
        public void AddRequest(string p_nom, string p_note, Song p_song)
        {
            Debug.Assert(CheckIfText(), "A customer name and a song is needed in order to proceed");
            this.mSong = p_song;
            this.mNom = p_nom;
            if (p_note != "")
            {
                this.mNote = p_note;
            }
            else
            {
                this.mNote = "";
            }

        }
        
        /*
         * 
         * 
         * CheckIfText
         * 
         * This method is used to verify that the textboxes are all full.
         * 
         * Please note that the notes are optional
         */

        public bool CheckIfText()
        {
            
            if (this.mSong == null || this.mNom == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public override string ToString()
        {
            StringBuilder chaine = new StringBuilder("");
            chaine.Append("Demande par : " + this.mNom + "\nChanson:" + this.mSong + "\nNotes: " + this.mNote + "\r\n");
            return chaine.ToString();
        }
    }
}

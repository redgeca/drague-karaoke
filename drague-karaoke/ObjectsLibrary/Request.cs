using System;
using System.Text;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace KaraokeObjectsLibrary
{
    class Request
    {
        [Key]
        public Guid uniqueKey { get; set; }

        public Song mSong { get; set; }
        public String mSingerName { get; set; }
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
        public void AddRequest(string pSingerName, string pNote, Song p_song)
        {
            Debug.Assert(CheckIfText(), "A customer name and a song is needed in order to proceed");
            mSong = p_song;
            mSingerName = pSingerName;

            if (pNote != null && !pNote.Trim().Equals(""))
            {
                mNote = pNote.Trim();
            }
            else
            {
                mNote = "";
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
            
            if (this.mSong == null || this.mSingerName == "")
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
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.Append("Demande par : " + this.mSingerName + "\nChanson:" + this.mSong + "\nNotes: " + this.mNote + "\r\n");
            return stringBuilder.ToString();
        }
    }
}

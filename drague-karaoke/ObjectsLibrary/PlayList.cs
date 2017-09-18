using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace KaraokeObjectsLibrary
{
    class PlayList
    {

        public List<Request> mListRequest { get; set; }


        /*
         * Constructor
         * 
         * 
         * Initiate a new List of Request
         * 
         */
        public PlayList()
        {

            this.mListRequest = new List<Request>();
        }
        /*
      * AddSongInPlayList
      * 
      * 
      * Add a song in the current playlist
      * 
      * Parameters
      * 
      * Request p_request
      */
        public void AddSongInPlayList(Request p_request)
        {
            Debug.Assert(p_request != null, "Please add a Request that is not null");
            this.mListRequest.Add(p_request);

        }
        /*
     * AddSongInPlayList
     * 
     * 
     * Remove a song in the current playlist
     * 
     * Parameters
     * 
     * int p_no
     */
        public void RemoveSongFromPlayList(int p_no)
        {

            Debug.Assert(p_no >= 0, "you need to enter a number superior to 0, there is no '-1' in the list");
            if (this.mListRequest.Count > 0)
            {
                this.mListRequest.RemoveAt(p_no);
            }
        }
        /*
    * AddSongInPlayList
    * 
    * Empty the current playlist
    */
        public void EmptyPlayList()
        {
            this.mListRequest.Clear();

        }
        /*
           * SwapRequest
           * GetRequest
           * 
           * parameters
           * 
           * p_no
           * 
           * return a specific song
           * 
           */

        public Request GetRequest(int p_no)
        {
            return this.mListRequest[p_no];
        }
        /*
            * SwapRequest
            * 
            * Exchange the value of two items in the list of requests
            */
        public void SwapRequest<Request>(int song1, int song2)
        {
            
            KaraokeObjectsLibrary.Request temp =  this.mListRequest[song1];
            this.mListRequest[song1] = this.mListRequest[song2];
            this.mListRequest[song2] = temp;
        }
    


    }
}

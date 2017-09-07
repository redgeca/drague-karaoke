using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KaraokeObjectsLibrary;
using System.Collections.Generic;

namespace SongExtractor
{
    class Program
    {
        private static String REMOTE_REPOSITORY = "remote-song-repository";
        private static String LOCAL_REPOSITORY = "local-song-repository";
        private static String FIELDS = "fields";

        static List<Song> getSongs(String pBaseUrl, int pPageNumber)
        {
            HttpClient songsClient = new HttpClient();
            songsClient.BaseAddress = new Uri(pBaseUrl + "posts");
            songsClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage songsResponse = songsClient.GetAsync("?fields=ID,title,tags,categories&page=" + pPageNumber).Result;

            if (songsResponse.IsSuccessStatusCode)
            {
                var dataObjects = songsResponse.Content.ReadAsStringAsync().Result;

                JObject converted = JsonConvert.DeserializeObject<JObject>(dataObjects);
                JValue foundObjects = (JValue)converted["found"];
                JArray songs = (JArray) converted["posts"];

                return songs.ToObject<List<Song>>();
            }

            // In case of error, return an empty list...
            return new List<Song>();
        }

        static List<Category> getCategories(String pBaseUrl, int pPageNumber)
        {
            HttpClient categoriesClient = new HttpClient();
            categoriesClient.BaseAddress = new Uri(pBaseUrl + "categories");
            categoriesClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage categoriesResponse = categoriesClient.GetAsync("?fields=ID,name&page=" + pPageNumber).Result;

            if (categoriesResponse.IsSuccessStatusCode)
            {
                var dataObjects = categoriesResponse.Content.ReadAsStringAsync().Result;

                JObject converted = JsonConvert.DeserializeObject<JObject>(dataObjects);
                JValue foundObjects = (JValue)converted["found"];
                JArray categories = (JArray)converted["categories"];

                return categories.ToObject<List<Category>>();
            }

            // In case of error, return an empty list...
            return new List<Category>();
        }

        static void Main(string[] args)
        {
            String remoteRepository = System.Configuration.ConfigurationManager.AppSettings[REMOTE_REPOSITORY];
            String localRepository = System.Configuration.ConfigurationManager.AppSettings[LOCAL_REPOSITORY];
            String fields = System.Configuration.ConfigurationManager.AppSettings[FIELDS];

            int pageNumber = 1;
            List<Category> categoryList = getCategories(remoteRepository, pageNumber);
            while (categoryList.Count > 0)
            {
                pageNumber++;
                foreach (Category category in categoryList)
                {
                    Console.WriteLine("Category " + category.mName);
                }

                categoryList = getCategories(remoteRepository, pageNumber);
            }

            Console.WriteLine("");

            pageNumber = 1;
            List<Song> songList = getSongs(remoteRepository, pageNumber);
            while (songList.Count > 0)
            {
                pageNumber++;
                foreach (Song song in songList)
                {
                    Console.WriteLine("Song : " + song.mTitle + "   : Tag : " + song.mTagName);
                }

                songList = getSongs(remoteRepository, pageNumber);
            }

            System.Console.ReadLine();
        }
    }
}

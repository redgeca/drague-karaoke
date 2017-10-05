
using KaraokeCoreObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace DBSetupAndDataSeed
{
    class Program
    {
        static List<Category> getCategories(String pBaseUrl, int pPageNumber)
        {
            HttpClient categoriesClient = new HttpClient();
            categoriesClient.BaseAddress = new Uri(pBaseUrl + "categories");
            categoriesClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage categoriesResponse = categoriesClient.GetAsync("?fields=name&page=" + pPageNumber).Result;

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

        static List<Song> getSongs(String pBaseUrl, int pPageNumber)
        {
            HttpClient songsClient = new HttpClient();
            songsClient.BaseAddress = new Uri(pBaseUrl + "posts");
            songsClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage songsResponse = songsClient.GetAsync("?fields=title,tags,categories,content&page=" + pPageNumber).Result;

            if (songsResponse.IsSuccessStatusCode)
            {
                var dataObjects = songsResponse.Content.ReadAsStringAsync().Result;

                JObject converted = JsonConvert.DeserializeObject<JObject>(dataObjects);
                JValue foundObjects = (JValue)converted["found"];
                JArray songs = (JArray)converted["posts"];

                return songs.ToObject<List<Song>>();
            }

            // In case of error, return an empty list...
            return new List<Song>();
        }


        static void Main(string[] args)
        {
            using(var db = new SongDBContext())
            {

                db.Database.ExecuteSqlCommand("delete from Songs");
                db.Database.ExecuteSqlCommand("delete from Categories");
                db.Database.ExecuteSqlCommand("delete from Artists");

                String repository = "https://public-api.wordpress.com/rest/v1.1/sites/lekaraoke.ca/";
                int pageNumber = 1;

                List<Category> categoryList = getCategories(repository, pageNumber);
                while (categoryList.Count > 0)
                {
                    pageNumber++;
                    foreach (Category category in categoryList)
                    {

                        db.Categories.Add(category);
                        
                        Console.WriteLine("Category " + category.Name);
                    }
                    db.SaveChanges();

                    categoryList = getCategories(repository, pageNumber);
                }

                pageNumber = 1;
                List<Song> songList = getSongs(repository, pageNumber);
                while (songList.Count > 0)
                {
                    pageNumber++;
                    foreach (Song song in songList)
                    {
                        String artistName = HttpUtility.HtmlDecode(song.ArtistName);
                        String categoryName = HttpUtility.HtmlDecode(song.CategoryName);
                        String content = HttpUtility.HtmlDecode(song.Content);

                        song.Title = HttpUtility.HtmlDecode(song.Title);

                        var artistQuery = from a in db.Artists
                                          where EF.Functions.Like(a.Name, artistName)
                                          select a;

                        if (artistQuery.Count() == 0)
                        {
                            Artist artist = new Artist();
                            artist.Name = artistName;
                            db.Artists.Add(artist);
                            song.Artist = artist;
                            db.SaveChanges();
                        } else
                        {
                            song.Artist = artistQuery.First();
                        }

                        var categoryQuery = from c in db.Categories
                                            where EF.Functions.Like(c.Name, categoryName)
                                            select c;

                        if (categoryQuery.Count() == 0)
                        {
                            Category category = new Category();
                            category.Name = categoryName;
                            db.Categories.Add(category);
                            song.Category = category;
                        } else
                        {
                            song.Category = categoryQuery.First();
                        }

                        db.Songs.Add(song);

                        Console.WriteLine("Song " + song.Title);
                    }
                    db.SaveChanges();

                    songList = getSongs(repository, pageNumber);
                }

            }
        }
    }
}

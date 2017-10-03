
using KaraokeCoreObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DBSetupAndDataSeed
{
    class Program
    {
        private static String REMOTE_REPOSITORY = "remote-song-repository";
        private static String LOCAL_REPOSITORY = "local-song-repository";
        private static String FIELDS = "fields";

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
            using(var db = new SongDBContext())
            {
/*
                List<Category> categoryList = getCategories(remoteRepository, pageNumber);
                while (categoryList.Count > 0)
                {
                    pageNumber++;
                    foreach (Category category in categoryList)
                    {
                        categoriesList.Add(category);
                        Console.WriteLine("Category " + category.mName);
                    }

                    categoryList = getCategories(remoteRepository, pageNumber);
                }
                */
            }
        }
    }
}

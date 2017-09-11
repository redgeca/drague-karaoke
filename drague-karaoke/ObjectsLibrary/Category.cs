using System;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace KaraokeObjectsLibrary
{
    public class Category
    {
        [JsonProperty("ID")]
        public long mId { get; set; }

        [JsonProperty("name")]
        public String mName { get; set;  }
    }
}

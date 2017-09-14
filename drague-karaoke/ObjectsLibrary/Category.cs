using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace KaraokeObjectsLibrary
{
    public class Category
    {
        [JsonProperty("ID")]
        public long mId { get; set; }

        [JsonProperty("name")]
        public String mName { get; set;  }

        [Key]
        public Guid uniqueKey { get; set; }
    }
}

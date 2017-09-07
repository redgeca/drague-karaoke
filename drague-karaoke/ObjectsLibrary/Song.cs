using Newtonsoft.Json;
using System;

namespace KaraokeObjectsLibrary
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class Song
    {
        [JsonProperty("ID")]
        public long mId { get; set; }

        [JsonProperty("title")]
        public String mTitle { get; set; }

        [JsonProperty("tags.name")]
        public String mTagName { get; set; }

        [JsonProperty("categories.ID")]
        public long mCategory { get; set; }
    }

}

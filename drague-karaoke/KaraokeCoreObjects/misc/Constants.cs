using System;
using System.Collections.Generic;
using System.Text;

namespace KaraokeCoreObjects.misc
{
    public class Constants
    {
        public const String INDEX_FOLDER = @"C:\karaoke\index";

        public const String CONNECTION_STRING = @"Server=.\SQLEXPRESS;Database=drague-karaoke;Trusted_Connection=True;";

        public const String TITLE_FIELD = "Title";

        public const String ARTIST_FIELD = "Artist";

        public const String CATEGORY_FIELD = "Category";

        public const string KARAOKE_STATE_FLAG = "KaraokeState";

        public const string RUNNING_FLAG = "RUNNING";

        public const string STOPPED_FLAG = "STOPPED";

        public const string LAST_STARTED_FLAG = "LastStarted";
    }
}

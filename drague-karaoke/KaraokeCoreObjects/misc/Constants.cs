using System;
using System.Collections.Generic;
using System.Text;

namespace KaraokeCoreObjects.misc
{
    public class Constants
    {
        public static readonly String INDEX_FOLDER = @"C:\karaoke\index";

        public static readonly String CONNECTION_STRING = @"Server=.\SQLEXPRESS;Database=drague-karaoke;Trusted_Connection=True;";

        public static readonly String TITLE_FIELD = "Title";

        public static readonly String ARTIST_FIELD = "Artist";

        public static readonly String CATEGORY_FIELD = "Category";

        public static readonly string KARAOKE_RUNNING_FLAG = "KaraokeRunning";

        public static readonly string RUNNING_FLAG = "RUNNING";

        public static readonly string STOPPED_FLAG = "STOPPED";
    }
}

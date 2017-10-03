using System;
using System.Collections.Generic;
using System.Text;

namespace KaraokeCoreObjects
{
    public class Song
    {
        public int Id { get; set; }
        public String Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}

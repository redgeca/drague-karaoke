using System;
using System.Collections.Generic;
using System.Text;

namespace KaraokeCoreObjects
{
    public class Artist
    {
        public int Id { get; set; }
        public String Name { get; set; }

        public List<Song> Songs { get; set; }
    }
}

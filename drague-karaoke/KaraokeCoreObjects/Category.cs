using System;
using System.Collections.Generic;

namespace KaraokeCoreObjects
{
    public class Category
    {
        public int Id { get; set; }
        public String Name { get; set; }

        public List<Song> Songs { get; set; }
    }
}

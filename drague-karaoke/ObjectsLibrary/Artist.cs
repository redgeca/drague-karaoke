using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KaraokeObjectsLibrary
{
    public class Artist
    {
        [Required]
        public String mName { get; set; }

        public List<Song> mSongs;

        [Key]
        public Guid uniqueKey { get; set; }

    }
}

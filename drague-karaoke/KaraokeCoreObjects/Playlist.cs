using System;
using System.Collections.Generic;
using System.Text;

namespace KaraokeCoreObjects
{
    public class Playlist
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int Order { get; set; }
        public int IsDone { get; set; }
    }
}


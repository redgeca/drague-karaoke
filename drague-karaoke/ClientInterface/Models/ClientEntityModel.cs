using KaraokeObjectsLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientInterface.Models
{
    public class ClientEntityModel : DbContext
    {
        DbSet<Song> Song;

    }
}
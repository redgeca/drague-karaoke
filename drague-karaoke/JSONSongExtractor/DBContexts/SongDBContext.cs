using KaraokeCoreObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBSetupAndDataSeed
{
    class SongDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=drague-karaoke;Trusted_Connection=True;", 
                b => b.MigrationsAssembly("DBSetupAndDataSeed"));
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

    }
}

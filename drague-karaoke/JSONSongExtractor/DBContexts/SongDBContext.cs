using KaraokeCoreObjects;
using Microsoft.EntityFrameworkCore;
using KaraokeCoreObjects.misc;

namespace DBSetupAndDataSeed
{
    class SongDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Constants.CONNECTION_STRING, 
                b => b.MigrationsAssembly("DBSetupAndDataSeed"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                        
//            modelBuilder.Entity<Song>().HasOne<Artist>();
//            modelBuilder.Entity<Song>().HasOne<Category>();
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<KaraokeState> KaraokeState { get; set; }

    }
}

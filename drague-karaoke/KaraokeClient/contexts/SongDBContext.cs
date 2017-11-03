using KaraokeCoreObjects;
using KaraokeCoreObjects.misc;
using Microsoft.EntityFrameworkCore;

namespace KaraokeClient.contexts

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
                        
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

    }
}

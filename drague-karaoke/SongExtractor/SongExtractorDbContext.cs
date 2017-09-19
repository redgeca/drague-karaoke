using KaraokeObjectsLibrary;
using Microsoft.EntityFrameworkCore;

namespace SongExtractor
{
 
    class SongExtractorDbContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Artist> Artists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.ConsoleApp.NewDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>().ToTable("Song");
            modelBuilder.Entity<Category>().ToTable("Category");
        }


    }

}

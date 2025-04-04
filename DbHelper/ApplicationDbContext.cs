using InternIntellegence_MovieApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternIntellegence_MovieApi.DbHelper
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Movie> Movies { get; set; }
		public DbSet<Genre> Genres { get; set; }
        public DbSet<UserFavourites> UserFavourites { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<UserFavourites>()
                .HasKey(uf => new { uf.UserId, uf.MovieId });

            modelBuilder.Entity<UserFavourites>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Favourites)
                .HasForeignKey(uf => uf.UserId);

            modelBuilder.Entity<UserFavourites>()
                .HasOne(uf => uf.movie)
                .WithMany(m => m.FavouritedBy)
                .HasForeignKey(uf => uf.MovieId);
        }


    }
}

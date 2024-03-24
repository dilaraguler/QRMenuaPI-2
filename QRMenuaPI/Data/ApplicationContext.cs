using System;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QRMenuaPI.Models;
using Microsoft.AspNetCore.Identity;

namespace QRMenuaPI.Data
{
	public class ApplicationContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {

		}
        public DbSet<Company>? Companies { get; set; }
        public DbSet<State>? States { get; set; }
        public DbSet<Restaurant>? Restaurants { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Food>? Foods { get; set; }
        public DbSet<RestaurantUser>? RestaurantUsers { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<ApplicationUser>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<Company>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<Food>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<Category>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<Restaurant>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<RestaurantUser>().HasOne(ru => ru.Restaurant).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<RestaurantUser>().HasOne(su => su.ApplicationUser).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelbuilder.Entity<RestaurantUser>().HasKey(r => new { r.RestaurantId, r.UserId });
            
            
            base.OnModelCreating(modelbuilder);


        }

        
      


       
    }
}


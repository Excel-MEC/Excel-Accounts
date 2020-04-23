using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(user => user.Id)
                .HasIdentityOptions(startValue: 12246);

            modelBuilder.Entity<College>()
                .Property(college => college.Id)
                .HasIdentityOptions(startValue: 440);

            modelBuilder.Entity<School>()
                .Property(school => school.Id)
                .HasIdentityOptions(startValue:2364);
            
            modelBuilder.Entity<User>()
                .Property(a => a.ReferrerAmbassadorId)
                .HasDefaultValue(value:null);

            modelBuilder.Entity<Ambassador>()
                .Property(a => a.Id)
                .HasIdentityOptions(startValue:502274, incrementBy: 27);

            modelBuilder.Entity<Ambassador>()
                .Property(a => a.UserId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasOne<Ambassador>(user => user.Ambassador)
                .WithOne(a => a.User)
                .HasForeignKey<Ambassador>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<User>()
                .HasOne<Ambassador>(user => user.Referrer)
                .WithMany(a => a.ReferredUsers)
                .HasForeignKey(a => a.ReferrerAmbassadorId)
                .OnDelete(DeleteBehavior.SetNull);   
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Ambassador> Ambassadors { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<School> Schools { get; set; }
    }

}
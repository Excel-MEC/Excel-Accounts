using Excel_Accounts_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Excel_Accounts_Backend.Data
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
                .HasIdentityOptions(startValue: 208);
            modelBuilder.Entity<School>()
                .Property(school => school.Id)
                .HasIdentityOptions(startValue: 1575);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<School> Schools { get; set; }
    }

}
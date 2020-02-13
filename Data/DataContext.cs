using Excel_Accounts_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Excel_Accounts_Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
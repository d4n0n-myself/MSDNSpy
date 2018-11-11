using System.IO;
using Microsoft.EntityFrameworkCore;

namespace MsdnSpy.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = File.ReadAllLines("connectionString.txt")[0];
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MsdnSpy.Infrastructure.Models;
using MsdnSpy.Infrastructure.Settings;

namespace MsdnSpy.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DatabaseContext(IConfigurationProvider configurationProvider)
        {
            _connectionString = configurationProvider.Config
                .GetSection("DatabaseSection").Get<DatabaseSettings>()
                .ConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        private readonly string _connectionString;
    }
}
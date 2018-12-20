using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MsdnSpy.Core;
using MsdnSpy.Infrastructure.Settings;

namespace MsdnSpy.Infrastructure
{
	public class DatabaseContext : DbContext
	{
		public DbSet<User> Users { get; private set; }
		public DbSet<HistoryEntry> History { get; set; }
		
		// For migration creations
		public DatabaseContext()
		{
			_connectionString = new ConfigurationProvider("appconfig.json").Config
				.GetSection("DatabaseSettings").Get<DatabaseSettings>()
				.ConnectionString;
		}

		public DatabaseContext(DatabaseSettings settings)
		{
			_connectionString = settings.ConnectionString;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(_connectionString);
		}

		private readonly string _connectionString;
	}
}
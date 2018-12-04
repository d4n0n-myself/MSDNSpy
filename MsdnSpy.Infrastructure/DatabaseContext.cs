using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MsdnSpy.Core;
using MsdnSpy.Infrastructure.Settings;

namespace MsdnSpy.Infrastructure
{
	public class DatabaseContext : DbContext
	{
		public DbSet<User> Users { get; private set; }
		public DbSet<UserPreferences> UserPreferences { get; private set; }

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
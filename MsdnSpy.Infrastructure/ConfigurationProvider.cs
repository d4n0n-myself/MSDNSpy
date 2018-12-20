using Microsoft.Extensions.Configuration;
using IConfigurationProvider = MsdnSpy.Infrastructure.Interfaces.IConfigurationProvider;

namespace MsdnSpy.Infrastructure
{
	public class ConfigurationProvider : IConfigurationProvider
	{
		public IConfiguration Config { get; }

		public ConfigurationProvider(string settingsFileName)
		{
			Config = new ConfigurationBuilder()
				.AddJsonFile(settingsFileName)
				.Build();
		}
	}
}

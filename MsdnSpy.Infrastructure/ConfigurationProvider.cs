using Microsoft.Extensions.Configuration;

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

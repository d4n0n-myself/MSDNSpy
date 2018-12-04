using Microsoft.Extensions.Configuration;

namespace MsdnSpy.Infrastructure
{
	public class ConfigurationProvider : IConfigurationProvider
	{
		public IConfiguration Config { get; }
			= new ConfigurationBuilder()
				.AddJsonFile("appconfig.json")
				.Build();
	}
}

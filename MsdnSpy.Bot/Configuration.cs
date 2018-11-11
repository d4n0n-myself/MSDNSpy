using Microsoft.Extensions.Configuration;
using MsdnSpy.Bot.Settings;

namespace MsdnSpy.Bot
{
	public static class Configuration
	{
		public static readonly BotSettings BotSettings;

		private static readonly string FileName = "botconfig.json";

		static Configuration()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile(FileName)
				.Build();

			BotSettings = config.Get<BotSettings>();
		}
	}
}

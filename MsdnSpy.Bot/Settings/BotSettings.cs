using System.Collections.Generic;

namespace MsdnSpy.Bot.Settings
{
	public class BotSettings
	{
		public string Token { get; set; }
		public IReadOnlyCollection<ProxySettings> Proxies { get; set; }
	}
}

using System.Collections.Generic;

namespace MsdnSpy.Infrastructure.Settings
{
	public class BotSettings
	{
		public int Port { get; set; }
		public string Token { get; set; }
		public IEnumerable<ProxySettings> Proxies { get; set; }
	}
}

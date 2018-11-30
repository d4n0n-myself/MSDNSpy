using System.Collections.Generic;

namespace MsdnSpy.Infrastructure.Settings
{
    public class BotSettings
    {
        public int Port { get; }
        public string Token { get; }
        public IEnumerable<ProxySettings> Proxies { get; }
    }
}

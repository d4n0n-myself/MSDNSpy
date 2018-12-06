using System;
using System.Net;

namespace MsdnSpy.Domain.Helpers
{
	public class PageDownloader
	{
		public PageDownloader(WebClient webClient)
		{
			_webClient = webClient ?? throw new ArgumentNullException(nameof(webClient));
			_webClient.Headers["User-Agent"] =
				"Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
				"(compatible; MSIE 6.0; Windows NT 5.1; " +
				".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
		}

		public string DownloadPage(string url)
		{
			return _webClient.DownloadString(url);
		}

		private readonly WebClient _webClient;
	}
}

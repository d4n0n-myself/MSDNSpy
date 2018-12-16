using MsdnSpy.Infrastructure.Settings;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MsdnSpy.Application
{
	public class Listener
	{
		public Listener(ApplicationSettings settings, Func<Server> serverProvider)
		{
			_prefix = settings.ServerPrefix;
			_httpListener = new HttpListener();
			_httpListener.Prefixes.Add(_prefix);

			_serverProvider = serverProvider ?? throw new ArgumentNullException(nameof(serverProvider));
		}

		public void Run()
		{
			if (_isListening)
				return;

			_isListening = true;
			_httpListener.Start();
			Console.WriteLine($"Now listening on {_prefix}");

			Listen();
		}

		private bool _isListening;

		private readonly HttpListener _httpListener;
		private readonly Func<Server> _serverProvider;
		private readonly string _prefix;

		private void Listen()
		{
			try
			{
				while (true)
				{
					var context = _httpListener.GetContext();
					Task.Run(() => _serverProvider().HandleRequest(context));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"{DateTime.UtcNow}: {e}");
			}
		}
	}
}
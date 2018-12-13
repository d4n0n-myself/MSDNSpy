using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace MsdnSpy.Application
{
	public class Listener
	{
		public Listener(string url, Server server)
		{
			_httpListener = new HttpListener();
			_httpListener.Prefixes.Add(url);
			_url = url;

			_server = server ?? throw new ArgumentNullException(nameof(server));
		}

		public static Listener RunNew(string url, Server server)
		{
			var listener = new Listener(url, server);
			listener.Run();
			return listener;
		}

		public void Run()
		{
			if (_isListening)
				return;

			_isListening = true;
			try
			{
				_httpListener.Start();
				Console.WriteLine($"Now listening on {_url}");

				Listen();
			}
			finally
			{
				_isListening = false;
			}
		}

		private bool _isListening;

		private readonly HttpListener _httpListener;
		private readonly Server _server;
		private readonly string _url;

		private void Listen()
		{
			try
			{
				while (true)
				{
					var context = _httpListener.GetContext();

					var result = _server.HandleRequest(context);

					var jsonResult = JsonConvert.SerializeObject(result);
					using (var output = new StreamWriter(context.Response.OutputStream))
						output.Write(jsonResult);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"{DateTime.UtcNow}: {e}");
			}
		}
	}
}
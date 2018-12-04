using MsdnSpy.Domain;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace MsdnSpy.Application
{
	public class Listener
	{
		public Listener(string url)
		{
			_httpListener = new HttpListener();
			_httpListener.Prefixes.Add(url);
			_url = url;
		}

		public static Listener RunNew()
		{
			var listener = new Listener($"http://localhost:1234/");
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

				while (true)
				{
					var context = _httpListener.GetContext();
					HandleRequest(context);
				}
			}
			finally
			{
				_isListening = false;
			}
		}

		private bool _isListening;

		private readonly HttpListener _httpListener;
		private readonly string _url;

		private void HandleRequest(HttpListenerContext context)
		{
            try
            {
                var query = context.Request.QueryString["query"];
                var infoGetter = new FromMsdnGetter(new WebClient());
                Console.WriteLine($"{DateTime.UtcNow}: Received query {query}");

                var result = infoGetter.GetInfoByQuery(query);

                using (var output = new StreamWriter(context.Response.OutputStream))
                    output.WriteLine(JsonConvert.SerializeObject(result));
                Console.WriteLine($"{DateTime.UtcNow}: Handled query {query}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.UtcNow}: {e}");
            }
		}
	}
}
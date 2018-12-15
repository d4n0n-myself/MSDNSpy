using Newtonsoft.Json;
using System.Net;

namespace MsdnSpy.Bot.Common
{
	public abstract class RequestHandler : IRequestHandler
	{
		public abstract string Command { get; }

		public abstract RequestResult HandleRequest(string args, long chatId);

		protected T GetObjectFromUrl<T>(string url, out WebException webError)
		{
			var request = WebRequest.Create(url);

			WebResponse response;
			try
			{
				response = request.GetResponse();
			}
			catch (WebException exception)
			{
				webError = exception;
				return default(T);
			}

			webError = null;
			var responseContent = response.GetResponseStream().ReadToEnd();
			return JsonConvert.DeserializeObject<T>(responseContent);
		}
	}
}

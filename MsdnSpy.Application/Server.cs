using MsdnSpy.Domain;
using MsdnSpy.Infrastructure;
using System;
using System.Net;

namespace MsdnSpy.Application
{
	public class Server
	{
		public Server(
			IInfoGetter infoGetter,
			IUserPreferencesRepository preferencesStorage)
		{
			_infoGetter = infoGetter ?? throw new ArgumentNullException(nameof(infoGetter));
			_preferencesStorage = preferencesStorage ?? throw new ArgumentNullException(nameof(preferencesStorage));
		}

		public object HandleRequest(HttpListenerContext context)
		{
			try
			{
				return context.Request.QueryString["query"] == null
					? HandlePreferencesRequest(context)
					: HandleDocumentationRequest(context);
			}
			catch (Exception e)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				var errorMessage = e.ToString();

				Console.WriteLine($"{DateTime.UtcNow}: {errorMessage}");
				return errorMessage;
			}
		}

		private readonly IInfoGetter _infoGetter;
		private readonly IUserPreferencesRepository _preferencesStorage;

		private object HandleDocumentationRequest(HttpListenerContext context)
		{
			var query = context.Request.QueryString["query"];
			Console.WriteLine($"{DateTime.UtcNow}: Received query {query}");
			
			var result = _infoGetter.GetInfoByQuery(query);

			Console.WriteLine($"{DateTime.UtcNow}: Handled query {query}");
			return result;
		}

		private object HandlePreferencesRequest(HttpListenerContext context)
		{
			var chatId = long.Parse(context.Request.QueryString["chatId"]);
			var category = context.Request.QueryString["category"];
			Console.WriteLine($"{DateTime.UtcNow}: Received preferences change: {category}");

			var result = _preferencesStorage.AddCategory(chatId, category);

			Console.WriteLine($"{DateTime.UtcNow}: Handled preferences change: {category}");
			return result;
		}
	}
}

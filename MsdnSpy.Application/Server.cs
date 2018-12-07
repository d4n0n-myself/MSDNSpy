using MsdnSpy.Domain;
using MsdnSpy.Domain.Helpers;
using MsdnSpy.Infrastructure;
using System;
using System.Net;

namespace MsdnSpy.Application
{
	public class Server
	{
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

		private static readonly DatabaseContext _databaseContext = new DatabaseContext(new ConfigurationProvider("appconfig.json"));
		private static readonly IUserPreferencesRepository _storage = new UserPreferencesRepository(_databaseContext);

		private object HandleDocumentationRequest(HttpListenerContext context)
		{
			var query = context.Request.QueryString["query"];
			Console.WriteLine($"{DateTime.UtcNow}: Received query {query}");

			var infoGetter = new FromMsdnGetter(new PageDownloader(new WebClient()));
			var result = infoGetter.GetInfoByQuery(query);

			Console.WriteLine($"{DateTime.UtcNow}: Handled query {query}");
			return result;
		}

		private object HandlePreferencesRequest(HttpListenerContext context)
		{
			var chatId = long.Parse(context.Request.QueryString["chatId"]);
			var category = context.Request.QueryString["category"];
			Console.WriteLine($"{DateTime.UtcNow}: Received preferences change: {category}");

			var result = _storage.AddCategory(chatId, category);

			Console.WriteLine($"{DateTime.UtcNow}: Handled preferences change: {category}");
			return result;
		}
	}
}

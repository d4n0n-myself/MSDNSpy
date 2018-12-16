using MsdnSpy.Domain;
using MsdnSpy.Infrastructure;
using MsdnSpy.Infrastructure.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public void HandleRequest(HttpListenerContext context)
		{
			RequestResult result;
			try
			{
				var args = context.Request.QueryString.AllKeys
					.ToDictionary(key => key, key => context.Request.QueryString[key]);
				result = context.Request.QueryString["query"] == null
					? HandlePreferencesRequest(args)
					: HandleDocumentationRequest(args);
			}
			catch (Exception exception)
			{
				Console.WriteLine($"{DateTime.UtcNow}: {exception}");
				result = new RequestResult(exception.Message, HttpStatusCode.InternalServerError);
			}
			
			context.Response.StatusCode = (int)result.StatusCode;
			var jsonResult = JsonConvert.SerializeObject(result.BodyContent);
			context.Response.OutputStream.Write(jsonResult);
		}

		private readonly IInfoGetter _infoGetter;
		private readonly IUserPreferencesRepository _preferencesStorage;

		private RequestResult HandleDocumentationRequest(IDictionary<string, string> args)
		{
			if (!args.ContainsKey("query"))
				return new RequestResult(
					@"Expected ""query"" parameter",
					HttpStatusCode.BadRequest);

			var query = args["query"];
			Console.WriteLine($"{DateTime.UtcNow}: Received query {query}");

			object result;
			try
			{
				result = _infoGetter.GetInfoByQuery(query);
			}
			catch (Exception)
			{
				return new RequestResult(
					"Not found information by the given query",
					HttpStatusCode.NotFound);
			}

			Console.WriteLine($"{DateTime.UtcNow}: Handled query {query}");
			return new RequestResult(result);
		}

		private RequestResult HandlePreferencesRequest(IDictionary<string, string> args)
		{
			if (!args.ContainsKey("chatId") ||
				 !args.ContainsKey("category"))
				return new RequestResult(
					@"Expected ""chatId"" and ""category"" parameters",
					HttpStatusCode.BadRequest);

			if (!long.TryParse(args["chatId"], out var chatId))
				return new RequestResult(
					@"Expected integer ""chatId"" parameter",
					HttpStatusCode.BadRequest);
			var category = args["category"];
			Console.WriteLine($"{DateTime.UtcNow}: Received preferences change: {category}");

			var result = _preferencesStorage.AddCategory(chatId, category);

			Console.WriteLine($"{DateTime.UtcNow}: Handled preferences change: {category}");
			return new RequestResult(result);
		}
	}
}

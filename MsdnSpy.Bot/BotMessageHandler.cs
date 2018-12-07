using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace MsdnSpy.Bot
{
	public static class BotMessageHandler
	{
		public static async void HandleMessage(object sender, MessageEventArgs args)
		{
			ITelegramBotClient bot;
			long chatId;

			try
			{
				bot = (ITelegramBotClient)sender;
				chatId = args.Message.Chat.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return;
			}

			try
			{
				var query = args.Message.Text;
				var queryBeginning = query.Substring(0, Math.Min(query.Length, 50));

				Console.WriteLine($"{DateTime.UtcNow}: Received query from {args.Message.Chat.Username}:\r\n" +
				                  queryBeginning);

				var command = GetCommand(args.Message.Text);

				var result = _requestHandlers[command](args.Message.Text, chatId);

				await bot.SendTextMessageAsync(chatId, result.Response, replyMarkup: result.ReplyMarkup);

				Console.WriteLine($"{DateTime.UtcNow}: Handled query from {args.Message.Chat.Username}:\r\n" +
				                  queryBeginning);
			}
			catch (Exception e)
			{
				Console.WriteLine($"{DateTime.UtcNow}: {e}");
				await bot.SendTextMessageAsync(chatId, $"This thing just said: {e.Message}");
			}
		}

		private static string GetCommand(string userInput)
		{
			var x = userInput.IndexOf(' ');
			var y = new string(userInput.Skip(1).TakeWhile(c => c != ' ').ToArray());
			return userInput.FirstOrDefault() != '/'
				? ""
				: y;
		}

		private delegate RequestResult RequestHandler(string query, long chatId);

		private static Dictionary<string, RequestHandler> _requestHandlers = new Dictionary<string, RequestHandler>
		{
			{"", SendDocumentionRequest},
			{"category", SendPreferencesRequest}
		};

		private static RequestResult SendDocumentionRequest(string query, long chatId)
		{
			var request = WebRequest.Create($"http://127.0.0.1:1234/?query={query}");
			var response = request.GetResponse();
			var responseStream = response.GetResponseStream();

			IDictionary<string, HashSet<string>> responseXml;
			string responseString;

			using (var reader =
				new StreamReader(responseStream ?? throw new ArgumentNullException(nameof(responseStream))))
			{
				var content = reader.ReadToEnd();
				responseXml = JsonConvert.DeserializeObject<IDictionary<string, HashSet<string>>>(content);
				responseString = $"{responseXml["Name"].First()}\r\n\r\n" +
					$"{responseXml["Description"].First()}\r\n\r\n" +
					$"{responseXml["MsdnUrl"].First()}";
			}

			var inlineKeyboardButtons = responseXml
				.Select(pair => new InlineKeyboardButton
					{Text = pair.Key, Url = $"http://127.0.0.1:1234/?query={pair.Key}&chatId={chatId}"})
				.Select(b => new List<InlineKeyboardButton> {b})
				.ToList();
			var replyKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

			return new RequestResult(responseString, replyKeyboardMarkup);
		}

		private static RequestResult SendPreferencesRequest(string query, long chatId)
		{
			query = new string(query.Skip(query.IndexOf(' ') + 1).ToArray());
			var request = WebRequest.Create($"http://127.0.0.1:1234/?category={query}&chatId={chatId}");
			var response = request.GetResponse();
			var responseStream = response.GetResponseStream();

			using (var reader = new StreamReader(responseStream))
			{
				var content = reader.ReadToEnd();
				var result = JsonConvert.DeserializeObject<bool>(content);
				return result
					? new RequestResult("Preferences changed.", null)
					: new RequestResult("Failed to change preferences.", null);
			}
		}
	}
}
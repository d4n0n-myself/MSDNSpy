using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

				Task.Run(() =>
				{
					var parsedInput = ParseInput(args.Message.Text);
					var result = _requestHandlers[parsedInput.command](parsedInput.args, chatId);

					bot.SendTextMessageAsync(chatId, result.Response, replyMarkup: result.ReplyMarkup).Wait();

					Console.WriteLine($"{DateTime.UtcNow}: Handled query from {args.Message.Chat.Username}:\r\n" +
											queryBeginning);
				});
			}
			catch (Exception e)
			{
				Console.WriteLine($"{DateTime.UtcNow}: {e}");
				await bot.SendTextMessageAsync(chatId, $"This thing just said: {e.Message}");
			}
		}

		private delegate RequestResult RequestHandler(string args, long chatId);

		private static readonly Dictionary<string, RequestHandler> _requestHandlers = new Dictionary<string, RequestHandler>
		{
			[""] = SendDocumentionRequest,
			["category"] = SendPreferencesRequest
		};

		private static (string command, string args) ParseInput(string userInput)
		{
			userInput = userInput.Trim();
			if (userInput.FirstOrDefault() != '/')
				return ("", userInput);

			var endOfCommand = userInput.IndexOf(' ');
			if (endOfCommand == -1)
				endOfCommand = userInput.Length;
			var command = userInput.Substring(1, endOfCommand - 1);

			var args = endOfCommand == userInput.Length
				? ""
				: userInput.Substring(endOfCommand + 1, userInput.Length - endOfCommand - 1);
			return (command, args);
		}

		private static RequestResult SendDocumentionRequest(string query, long chatId)
		{
			var request = WebRequest.Create($"http://127.0.0.1:1234/?query={query}");
			var response = request.GetResponse();
			var responseStream = response.GetResponseStream();

			IDictionary<string, HashSet<string>> parsedResponse;
			string result;

			using (var reader = new StreamReader(responseStream))
			{
				var content = reader.ReadToEnd();
				parsedResponse = JsonConvert.DeserializeObject<IDictionary<string, HashSet<string>>>(content);
				result = $"{parsedResponse["Name"].First()}\r\n\r\n" +
					$"{parsedResponse["Description"].First()}\r\n\r\n" +
					$"{parsedResponse["MsdnUrl"].First()}";
			}

			var inlineKeyboardButtons = parsedResponse
				.Select(pair => new InlineKeyboardButton
					{Text = pair.Key, Url = $"http://127.0.0.1:1234/?query={pair.Key}&chatId={chatId}"})
				.Select(b => new List<InlineKeyboardButton> {b})
				.ToList();
			var replyKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

			return new RequestResult(result, replyKeyboardMarkup);
		}

		private static RequestResult SendPreferencesRequest(string query, long chatId)
		{
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
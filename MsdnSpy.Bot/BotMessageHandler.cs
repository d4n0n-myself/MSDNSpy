using MsdnSpy.Core;
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
		public static void HandleMessage(object sender, MessageEventArgs args)
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

				switch (args.Message.Text)
				{
					case null:
						return;
					case "Assembly":
					case "Methods":
						bot.SendTextMessageAsync(chatId, "To be added.");
						break;
					default:
						{
							var inlineKeyboardButtons = UserPreferences.DefaultPreferences
								.Where(x => x.Value)
								.Select(x => new InlineKeyboardButton { Text = x.Key, Url = "https://google.com/" })
								.Select(x => new List<InlineKeyboardButton> { x })
								.ToList();
							var replyKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons) { };

							var result = SendRequest(query);
							var answer = $"{query}\r\n\r\n{result["Docs.summary"]}\r\n\r\n{result["msdnLink"]}";

							bot.SendTextMessageAsync(chatId, answer, replyMarkup: replyKeyboardMarkup);
							break;
						}
				}

				Console.WriteLine($"{DateTime.UtcNow}: Handled query from {args.Message.Chat.Username}:\r\n" +
					queryBeginning);
			}
			catch (Exception e)
			{
				Console.WriteLine($"{DateTime.UtcNow}: {e}");
				bot.SendTextMessageAsync(chatId, $"This thing just said: {e.Message}");
			}
		}

		private static IDictionary<string, string> SendRequest(string query)
		{
			var request = WebRequest.Create($"http://localhost:1234/?query={query}");
			var response = request.GetResponse();
			var responseStream = response.GetResponseStream();

			using (var reader = new StreamReader(responseStream))
			{
				var content = reader.ReadToEnd();
				return JsonConvert.DeserializeObject<IDictionary<string, string>>(content);
			}
		}
	}
}
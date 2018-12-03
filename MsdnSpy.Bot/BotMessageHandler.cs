using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MsdnSpy.Core;
using MsdnSpy.Infrastructure;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;


namespace MsdnSpy.Bot
{
	public class BotMessageHandler
	{
		private static DatabaseContext _context = new DatabaseContext(Program.Provider);
		private static IUserPreferencesRepository _userPreferencesRepository = new UserPreferencesRepository(_context);

		public void HandleMessage(object sender, MessageEventArgs args)
		{
			ITelegramBotClient bot;
			long chatId;
			try
			{
				bot = (ITelegramBotClient) sender;
				chatId = args.Message.Chat.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return;
			}

			Console.WriteLine($"{DateTime.UtcNow} UTC: Hello from {args.Message.Chat.Username}");

			try
			{
				var inlineKeyboardButtons = UserPreferences.DefaultPreferences.Select(x =>
					x.Value ? new InlineKeyboardButton {Text = x.Key, Url = "https://google.com/"} : null).Select(
					x => new List<InlineKeyboardButton> {x}).ToList();
				var replyKeyboardMarkup =
					new InlineKeyboardMarkup(inlineKeyboardButtons) { };

				var responseString = SendRequest(args);
				var answer = $"{args.Message.Text}\r\n\r\n{responseString["Docs.summary"]}\r\n\r\n{responseString["msdnLink"]}";
				bot.SendTextMessageAsync(chatId, answer, replyMarkup: replyKeyboardMarkup);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				bot.SendTextMessageAsync(chatId, $"This thing just said : {e.Message}");
			}

			Console.WriteLine($"{DateTime.UtcNow}: Handled.");
		}

		private static IDictionary<string,string> SendRequest(MessageEventArgs args)
		{
			var request = WebRequest.Create($"http://localhost:1234/?query={args.Message.Text}");
			var reader = request.GetResponse().GetResponseStream() ??
			             throw new ArgumentNullException(nameof(request.GetResponse));
			var buffer = new byte[1024];
			reader.Read(buffer, 0, 1024);
			if (buffer.Length == 0) return new Dictionary<string, string>();
			var result = Encoding.ASCII.GetString(buffer.Where(x => x != 0).ToArray());
			return JsonConvert.DeserializeObject<IDictionary<string, string>>(result);
		}
	}
}
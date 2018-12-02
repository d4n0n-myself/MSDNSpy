using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using MsdnSpy.Core;
using MsdnSpy.Domain;
using MsdnSpy.Infrastructure;
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
				bot = (ITelegramBotClient)sender;
				chatId = args.Message.Chat.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return;
			}
			
			Console.WriteLine($"{DateTime.UtcNow} UTC: Hello from {args.Message.Chat.Username}");

			//var preferences = _userPreferencesRepository.GetPreferencesByChatId(chatId);
			
			try
			{

//				switch (args.Message.Text)
//				{
//					case null:
//						return;
//					case "Assembly":
//					case "Methods":
//						bot.SendTextMessageAsync(chatId, "To be added.");
//						break;
//					default:
//					{
				var inlineKeyboardButtons = UserPreferences.DefaultPreferences.Select(x => x.Value ? new InlineKeyboardButton {Text = x.Key, Url = "https://google.com/"} : null).Select(
					x=> new List<InlineKeyboardButton> {x}).ToList();
				var replyKeyboardMarkup =
							new InlineKeyboardMarkup(inlineKeyboardButtons);
				
						var queryToXml =
							"https://raw.githubusercontent.com/dotnet/dotnet-api-docs/master/" +
							InfoParser.FindXmlFilePath(args.Message.Text);
						var msdnLink =
							Test.GetMsdnUrl(
								$"https://social.msdn.microsoft.com/Search/ru-RU?query={args.Message.Text}&pgArea=header&emptyWatermark=true&ac=4");
						var parsedXml = InfoParser.XmlParser(queryToXml);
//						var wc = WebRequest.Create($"http://localhost:1234/");
//						var response = wc.GetResponse();
						bot.SendTextMessageAsync(chatId,
							$"{args.Message.Text}\r\n\r\n\r\n{parsedXml["Docs.summary"]}\r\n\r\n\r\n{msdnLink}",
							replyMarkup: replyKeyboardMarkup);
//						break;
//					}
//				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				bot.SendTextMessageAsync(chatId, $"This thing just said : {e.Message}");
			}

			Console.WriteLine($"{DateTime.UtcNow}: Handled.");
		}

		private static string SendRequest(MessageEventArgs args)
		{
			
			var request = WebRequest.Create($"http://localhost:1234/?query={args.Message.Text}");
			var response = request.GetResponse();
			
			var buffer = new byte[100];
			response.GetResponseStream().Read(buffer, 0, 100);
			return buffer.Length != 0 ? Encoding.ASCII.GetString(buffer) : string.Empty;
		}
		
	}
}

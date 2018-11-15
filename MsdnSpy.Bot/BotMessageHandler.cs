using System;
using System.Collections.Generic;
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
				Console.WriteLine($"{DateTime.UtcNow} UTC: Hello from {args.Message.Chat.Username}");

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
						var replyKeyboardMarkup =
							new ReplyKeyboardMarkup(
								new List<KeyboardButton> {new KeyboardButton("Assembly"), new KeyboardButton("Methods")},
								true, true);
						var queryToXml =
							"https://raw.githubusercontent.com/dotnet/dotnet-api-docs/master/" + InfoParser.FindXmlFilePath(args.Message.Text);
						var msdnLink =
							Test.GetMsdnUrl(
								$"https://social.msdn.microsoft.com/Search/ru-RU?query={args.Message.Text}&pgArea=header&emptyWatermark=true&ac=4");
						var parsedXml = InfoParser.XmlParser(queryToXml);
						bot.SendTextMessageAsync(chatId,
							$"{args.Message.Text}\r\n\r\n\r\n{parsedXml["Docs.summary"]}\r\n\r\n\r\n{msdnLink}",
							replyMarkup: replyKeyboardMarkup);
						break;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				bot.SendTextMessageAsync(chatId, $"This thing just said : {e.Message}");
			}

			Console.WriteLine($"{DateTime.UtcNow}: Handled.");
		}
	}
}

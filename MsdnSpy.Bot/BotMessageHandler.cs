using MsdnSpy.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace MsdnSpy.Bot
{
    public class BotMessageHandler
	{
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
			
			try
			{
				Console.WriteLine($"{DateTime.UtcNow}: Hello from {args.Message.Chat.Username}");

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
                            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                                new List<KeyboardButton> { new KeyboardButton("Assembly"), new KeyboardButton("Methods") },
                                resizeKeyboard: true,
                                oneTimeKeyboard: true
                            );

                            //var wc = WebRequest.Create($"http://localhost:1234/");
                            //var response = wc.GetResponse();

                            var infoGetter = new FromMsdnGetter(new WebClient());
                            var result = infoGetter.GetInfoByQuery(args.Message.Text);
                            bot.SendTextMessageAsync(
                                chatId,
                                result,
                                replyMarkup: replyKeyboardMarkup
                            );
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

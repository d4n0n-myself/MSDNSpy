using MsdnSpy.Bot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace MsdnSpy.Bot
{
	public class MessageDispatcher
	{
		public MessageDispatcher(IRequestHandler[] requestHandlers)
		{
			_requestHandlers = requestHandlers?.ToDictionary(handler => handler.Command)
				?? throw new ArgumentNullException(nameof(requestHandlers));
		}

		public async void HandleMessage(object sender, MessageEventArgs args)
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

				await Task.Run(async () =>
				{
					var (command, commandArgs) = ParseInput(args.Message.Text);
					if (!_requestHandlers.TryGetValue(command, out var requestHandler))
					{
						ReportError(bot, chatId,
							$"{DateTime.UtcNow}: Not found handler for query from {args.Message.Chat.Username}:\r\n" +
								$"{queryBeginning}",
							"Unknown command");
						return;
					}

					var result = requestHandler.HandleRequest(commandArgs, chatId);
					if (!result.Handled)
					{
						ReportError(bot, chatId,
							$"{DateTime.UtcNow}: Failed to handle query from {args.Message.Chat.Username}:\r\n" +
								$"{queryBeginning}\r\n" +
								$"Error message: {result.ErrorMessage}",
							result.ErrorMessage);
						return;
					}
					
					Console.WriteLine($"{DateTime.UtcNow}: Handled query from {args.Message.Chat.Username}:\r\n" +
											queryBeginning);
					await bot.SendTextMessageAsync(chatId, result.Response, replyMarkup: result.ReplyMarkup);
				});
			}
			catch (Exception e)
			{
				ReportError(bot, chatId,
					$"{DateTime.UtcNow}: {e}",
					$"Internal error: {e.Message}");
			}
		}

		private readonly Dictionary<string, IRequestHandler> _requestHandlers;

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

		private static void ReportError(
			ITelegramBotClient bot,
			long chatId,
			string consoleMessage,
			string botMessage)
		{
			Console.WriteLine(consoleMessage);
			bot.SendTextMessageAsync(chatId, botMessage);
		}
	}
}
using MsdnSpy.Bot.Common;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace MsdnSpy.Bot
{
	public class DocumentationGetter : RequestHandler
	{
		public override string Command => "";

		public override RequestResult HandleRequest(string query, long chatId)
		{
			var documentation = GetObjectFromUrl<IDictionary<string, HashSet<string>>>(
				$"http://127.0.0.1:1234/?query={query}",
				out var webError
			);
			if (webError != null)
				return new RequestResult(webError);

			var result = $"{documentation["Name"].First()}\r\n\r\n" +
			             $"{documentation["Description"].First()}\r\n\r\n" +
			             $"{documentation["MsdnUrl"].First()}";
			var inlineKeyboardButtons = documentation
				.Select(pair => new InlineKeyboardButton
				{ Text = pair.Key, Url = $"http://127.0.0.1:1234/?query={pair.Key}&chatId={chatId}" })
				.Select(b => new List<InlineKeyboardButton> { b })
				.ToList();
			var replyKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

			return new RequestResult(result, replyKeyboardMarkup);
		}
	}
}

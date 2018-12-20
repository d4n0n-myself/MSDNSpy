using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using MsdnSpy.Bot.Common;

namespace MsdnSpy.Bot
{
	public class HistoryHelper : RequestHandler
	{
		public override string Command => "history";

		public override RequestResult HandleRequest(string args, long chatId)
		{
			var history = GetObjectFromUrl<IEnumerable<string>>(
				$"http://127.0.0.1:1234/?history=true&chatId={chatId}",
				out var webError
			);
			if (webError != null)
				return new RequestResult(webError);

//			var result = JsonConvert.DeserializeObject<HistoryEntry[]>(history.ToString());
			var result = history.Join("\r\n");

			return new RequestResult(result, null);
		}
	}
}
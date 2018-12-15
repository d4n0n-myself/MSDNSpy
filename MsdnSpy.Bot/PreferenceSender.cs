using MsdnSpy.Bot.Common;

namespace MsdnSpy.Bot
{
	public class PreferenceSender : RequestHandler
	{
		public override string Command => "category";

		public override RequestResult HandleRequest(string category, long chatId)
		{
			var result = GetObjectFromUrl<bool>(
				$"http://127.0.0.1:1234/?category={category}&chatId={chatId}",
				null,
				out var webError
			);
			if (webError != null)
				return new RequestResult(webError);

			return result
				? new RequestResult("Preferences changed.", null)
				: new RequestResult("Failed to change preferences.", null);
		}
	}
}

using MsdnSpy.Infrastructure.Common;
using System.Net;
using Telegram.Bot.Types.ReplyMarkups;

namespace MsdnSpy.Bot.Common
{
	public class RequestResult
	{
		public readonly bool Handled;

		public readonly IReplyMarkup ReplyMarkup;
		public readonly string Response;
		public readonly WebException WebError;
		public readonly string ErrorMessage;

		public RequestResult(string response, IReplyMarkup markup)
		{
			Handled = true;

			Response = response;
			ReplyMarkup = markup;
		}

		public RequestResult(WebException webError)
		{
			Handled = false;

			WebError = webError;
			ErrorMessage = GetErrorMessage(webError);
		}

		private string GetErrorMessage(WebException webError)
		{
			var statusCode = (webError.Response as HttpWebResponse)?.StatusCode;
			var errorMessage = webError.Response.GetResponseStream().ReadToEnd();

			return statusCode == null
				? errorMessage
				: $"{statusCode} ({(int)statusCode}): {errorMessage}";
		}
	}
}
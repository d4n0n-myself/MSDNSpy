using System.Collections.Generic;
using System.Net;

namespace MsdnSpy.Application
{
	public class RequestResult
	{
		public readonly HttpStatusCode StatusCode;
		public readonly object BodyContent;
		public readonly IEnumerable<string> Preferences;
		
		public RequestResult(
			object result,
			IEnumerable<string> userPreferences,
			HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			StatusCode = statusCode;
			BodyContent = result;
			Preferences = userPreferences;
		}
	}
}

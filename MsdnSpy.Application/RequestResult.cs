using System.Net;

namespace MsdnSpy.Application
{
	public class RequestResult
	{
		public readonly HttpStatusCode StatusCode;
		public readonly object BodyContent;

		public RequestResult(
			object result,
			HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			StatusCode = statusCode;
			BodyContent = result;
		}
	}
}

namespace MsdnSpy.Bot.Common
{
	public interface IRequestHandler
	{
		string Command { get; }

		RequestResult HandleRequest(string args, long chatId);
	}
}

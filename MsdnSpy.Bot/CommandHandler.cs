namespace MsdnSpy.Bot
{
	public static class CommandHandler
	{
		public static bool Handle(string command)
		{
			if (command == "stop")
				return false;
			return true;
		}
	}
}

namespace MsdnSpy.Bot
{
    public class CommandHandler
    {
        public bool Handle(string command)
        {
            return command != "stop";
        }
    }
}

namespace MsdnSpy.Bot
{
    public class CommandHandler
    {
        public bool Handle(string command)
        {
            if (command == "stop")
                return false;
            return true;
        }
    }
}

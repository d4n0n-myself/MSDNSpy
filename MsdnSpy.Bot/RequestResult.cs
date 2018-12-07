using Telegram.Bot.Types.ReplyMarkups;

namespace MsdnSpy.Bot
{
    internal class RequestResult
    {
        public RequestResult(string response, IReplyMarkup markup)
        {
            Response = response;
            ReplyMarkup = markup;
        }
        
        internal IReplyMarkup ReplyMarkup;
        internal string Response;
    }
}
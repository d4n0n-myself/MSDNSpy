using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace MsdnSpy.Bot
{
    public static class BotMessageHandler
    {
        public static void HandleMessage(object sender, MessageEventArgs args)
        {
            try
            {
                var bot = (ITelegramBotClient)sender;
                var chatId = args.Message.Chat.Id;
                Console.WriteLine($"{DateTime.UtcNow} UTC: Hello from {args.Message.Chat.Username}");

                if (args.Message.Text != null)
                    bot.SendTextMessageAsync(chatId, args.Message.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

using com.LandonKey.SocksWebProxy;
using com.LandonKey.SocksWebProxy.Proxy;
using Microsoft.Extensions.Configuration;
using MsdnSpy.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using ConfigurationProvider = MsdnSpy.Infrastructure.ConfigurationProvider;

namespace MsdnSpy.Bot
{
    internal static class Program
    {
        private static readonly List<ITelegramBotClient> Bots = new List<ITelegramBotClient>();

        private static void Main(string[] args)
        {
            InitializeBots(
                new ConfigurationProvider(),
                BotMessageHandler.HandleMessage);
            StartBots();
            HandleConsoleInput(new CommandHandler());
        }

        private static void InitializeBots(
            ConfigurationProvider configurationProvider,
            EventHandler<MessageEventArgs> messageHandler)
        {
            var settings = configurationProvider.Config
                .GetSection("BotSection").Get<BotSettings>();

            Bots.Add(new TelegramBotClient(settings.Token));
            foreach (var proxySettings in settings.Proxies)
            {
                var proxyConfig = new ProxyConfig
                (
                    IPAddress.Loopback,
                    settings.Port,
                    IPAddress.Parse(proxySettings.Ip),
                    proxySettings.Port,
                    ProxyConfig.SocksVersion.Five,
                    proxySettings.UserName,
                    proxySettings.Password
                );
                Bots.Add(new TelegramBotClient
                (
                        settings.Token,
                        new SocksWebProxy
                        (
                            proxyConfig,
                            allowBypass: false
                        )
                ));
            }

            foreach (var bot in Bots)
                bot.OnMessage += messageHandler;
        }

        private static void StartBots()
        {
            foreach (var bot in Bots)
            {
                Console.WriteLine($"Started bot with id {bot.BotId}");
                Task.Run(() => bot.StartReceiving());
            }
        }

        private static void HandleConsoleInput(CommandHandler commandHandler)
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (!commandHandler.Handle(input))
                    return;
            }
        }
    }
}

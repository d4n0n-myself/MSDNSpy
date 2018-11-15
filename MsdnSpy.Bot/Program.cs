using com.LandonKey.SocksWebProxy;
using com.LandonKey.SocksWebProxy.Proxy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MsdnSpy.Bot
{	
	internal static class Program
	{
		private static readonly List<ITelegramBotClient> Bots = new List<ITelegramBotClient>();

		private static void Main(string[] args)
		{
			InitializeBots();
			StartBots();
			HandleConsoleInput();
		}
		
		private static void InitializeBots()
		{
			var settings = Configuration.BotSettings;

			Bots.Add(new TelegramBotClient(settings.Token));
			foreach (var proxySettings in settings.Proxies)
			{
				var proxyConfig = new ProxyConfig
				(
					IPAddress.Loopback,
					29505,
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
				bot.OnMessage += BotMessageHandler.HandleMessage;
		}

		private static void StartBots()
		{
			foreach (var bot in Bots)
			{
				Console.WriteLine($"Started bot with id {bot.BotId}");
				Task.Run(() => bot.StartReceiving());
			}
		}

		private static void HandleConsoleInput()
		{
			while (true)
			{
//				var input = Console.ReadLine();
//				if (!CommandHandler.Handle(input))
//					return;
				Thread.Sleep(100);
			}
		}
	}
}

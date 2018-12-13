using com.LandonKey.SocksWebProxy;
using com.LandonKey.SocksWebProxy.Proxy;
using Microsoft.Extensions.Configuration;
using MsdnSpy.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MsdnSpy.Bot
{
	public static class Program
	{
		private static readonly BotSettings Settings
			= new Infrastructure.ConfigurationProvider("botconfig.json").Config
				.GetSection("BotSettings").Get<BotSettings>();

		private static void Main(string[] args)
		{
			var bots = CreateBots();
			StartBots(bots);

			while (true)
			{
				var command = Console.ReadLine();
				if (command == "stop")
					return;
			}
		}

		private static IEnumerable<ITelegramBotClient> CreateBots()
		{
			yield return new TelegramBotClient(Settings.Token);

			foreach (var proxySettings in Settings.Proxies)
			{
				var proxy = GetProxy(proxySettings, IPAddress.Loopback, Settings.Port);
				yield return new TelegramBotClient(Settings.Token, proxy);
			}
		}

		private static SocksWebProxy GetProxy(
			ProxySettings proxySettings,
			IPAddress selfIp,
			int selfPort)
		{
			var proxyConfig = new ProxyConfig
			(
				selfIp,
				selfPort,
				IPAddress.Parse(proxySettings.Ip),
				proxySettings.Port,
				ProxyConfig.SocksVersion.Five,
				proxySettings.UserName,
				proxySettings.Password
			);
			return new SocksWebProxy(proxyConfig, allowBypass: false);
		}

		private static void StartBots(IEnumerable<ITelegramBotClient> bots)
		{
			foreach (var bot in bots)
			{
				bot.OnMessage += BotMessageHandler.HandleMessage;

				Task.Run(() => bot.StartReceiving());
				Console.WriteLine($"Started bot with id {bot.BotId}");
			}
		}
	}
}

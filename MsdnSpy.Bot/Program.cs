using com.LandonKey.SocksWebProxy;
using com.LandonKey.SocksWebProxy.Proxy;
using Microsoft.Extensions.Configuration;
using MsdnSpy.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using ConfigurationProvider = MsdnSpy.Infrastructure.ConfigurationProvider;

namespace MsdnSpy.Bot
{
	public static class Program
	{
		private static readonly IConfiguration Config
			= new ConfigurationProvider("botconfig.json").Config;

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
			var settings = Config.GetSection("BotSection").Get<BotSettings>();

			yield return new TelegramBotClient(settings.Token);
			foreach (var proxySettings in settings.Proxies)
			{
				var proxy = GetProxy(proxySettings, IPAddress.Loopback, settings.Port);
				yield return new TelegramBotClient(settings.Token, proxy);
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

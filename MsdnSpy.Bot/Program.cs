using com.LandonKey.SocksWebProxy;
using com.LandonKey.SocksWebProxy.Proxy;
using Microsoft.Extensions.Configuration;
using MsdnSpy.Bot.Common;
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
		private static readonly BotSettings Settings;
		private static readonly MessageDispatcher MessageDispatcher;
		internal static readonly IDictionary<long, IDictionary<string, HashSet<string>>> LastRequests;

		static Program()
		{
			Settings = new Infrastructure.ConfigurationProvider("botconfig.json").Config
				.GetSection("BotSettings").Get<BotSettings>();

			MessageDispatcher = new MessageDispatcher(new IRequestHandler[]
				{new DocumentationGetter(), new PreferenceSender(), new HistoryHelper()});

			LastRequests = new Dictionary<long, IDictionary<string, HashSet<string>>>();
		}

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

		private static void StartBots(IEnumerable<ITelegramBotClient> bots)
		{
			foreach (var bot in bots)
			{
				bot.OnMessage += MessageDispatcher.HandleMessage;
				bot.OnCallbackQuery += MessageDispatcher.HandleCallbackQuery;
				Task.Run(() => bot.StartReceiving());
				Console.WriteLine($"Started bot with id {bot.BotId}");
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
	}
}
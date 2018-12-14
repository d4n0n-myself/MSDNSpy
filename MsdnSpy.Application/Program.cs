using Microsoft.Extensions.Configuration;
using MsdnSpy.Domain;
using MsdnSpy.Infrastructure;
using MsdnSpy.Infrastructure.Settings;
using Ninject;
using System;
using System.Net;
using IConfigurationProvider = MsdnSpy.Infrastructure.IConfigurationProvider;

namespace MsdnSpy.Application
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var diContainer = SetupDiContainer();

			diContainer.Get<Listener>().Run();

			Console.WriteLine("Server stopped");
			Console.ReadKey();
		}

		private static StandardKernel SetupDiContainer()
		{
			var diContainer = new StandardKernel();

			SetupConfiguration(diContainer);
			SetupApplication(diContainer);
			SetupDatabase(diContainer);

			return diContainer;
		}

		private static void SetupConfiguration(StandardKernel diContainer)
		{
			diContainer.Bind<IConfigurationProvider>().ToConstructor(x =>
				new Infrastructure.ConfigurationProvider("appconfig.json"));

			diContainer.Bind<ApplicationSettings>().ToMethod(x =>
				x.Kernel.Get<IConfigurationProvider>().Config
					.GetSection("ApplicationSettings").Get<ApplicationSettings>());
			diContainer.Bind<DatabaseSettings>().ToMethod(x =>
				x.Kernel.Get<IConfigurationProvider>().Config
					.GetSection("DatabaseSettings").Get<DatabaseSettings>());
		}

		private static void SetupApplication(StandardKernel diContainer)
		{
			diContainer.Bind<Listener>().ToSelf().InSingletonScope();
			diContainer.Bind<Func<Server>>().ToMethod(
				x => () => x.Kernel.Get<Server>());
			diContainer.Bind<IInfoGetter>().To<FromMsdnGetter>();
			diContainer.Bind<WebClient>().ToMethod(x =>
			{
				var webClient = new WebClient();
				webClient.Headers["User-Agent"] = "Mozilla/4.0";
				return webClient;
			});
		}

		private static void SetupDatabase(StandardKernel diContainer)
		{
			diContainer.Bind<IUserPreferencesRepository>().To<UserPreferencesRepository>();
		}
	}
}
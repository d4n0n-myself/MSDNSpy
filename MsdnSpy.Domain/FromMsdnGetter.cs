using AngleSharp.Dom;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using AngleSharp.Parser.Xml;
using MsdnSpy.Domain.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MsdnSpy.Domain
{
	public class FromMsdnGetter : IInfoGetter
	{
		public FromMsdnGetter(WebClient webClient)
		{
			_webClient = webClient ?? throw new ArgumentNullException(nameof(webClient));
		}

		public IDictionary<string, HashSet<string>> GetInfoByQuery(string query)
		{
			var msdnSearchPage = _webClient.DownloadString(
				$"https://social.msdn.microsoft.com/Search/ru-RU?query={query}");
			var msdnUrl = GetMsdnUrlFromSearchPage(msdnSearchPage);

			var name = "";
			var description = "";
			try
			{
				var msdnPage = _webClient.DownloadString(msdnUrl);
				var parsedMsdnPage = new HtmlParser().Parse(msdnPage);

				var githubUrl = GetGithubUrlFromMsdnPage(parsedMsdnPage)
					.Replace("blob", "raw");
				var githubPage = _webClient.DownloadString(githubUrl);
				var parsedGithubPage = new XmlParser().Parse(githubPage);

				//var name = parsedMsdnPage.GetElementsBy("html > head > meta", new Dictionary<string, string>
				//{
				//	["name"] = "APIName"
				//}).FirstOrDefault()?.GetAttribute("content");
				name = parsedGithubPage.QuerySelector("Type").GetAttribute("FullName");

				description = parsedGithubPage.QuerySelector("Type > Docs > summary").Text();
			}
			catch { }

			return new Dictionary<string, HashSet<string>>
			{
				["Name"] = new HashSet<string> { name },
				["Description"] = new HashSet<string> { description },
				["MsdnUrl"] = new HashSet<string> { msdnUrl }
			};
		}

		private readonly WebClient _webClient;

		private string GetGithubUrlFromMsdnPage(IDocument msdnPage)
		{
			return msdnPage.GetElementsBy("html > head > meta", new Dictionary<string, string>
			{
				["name"] = "original_content_git_url"
			}).First().GetAttribute("content");
		}

		private string GetMsdnUrlFromSearchPage(string content)
		{
			const string resultsBeginning = "var results = ";
			var startIndex = content.IndexOf(resultsBeginning) + resultsBeginning.Length;
			content = content.Substring(startIndex);

			var endIndex = content.IndexOf("};");
			content = content.Substring(0, endIndex + 1);
			
			return JsonConvert.DeserializeObject<JObject>(content)
				["data"]["results"]
				.Select(x => x["url"].ToObject<string>())
				.First();

		}
	}
}
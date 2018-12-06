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

namespace MsdnSpy.Domain
{
	public class FromMsdnGetter : IInfoGetter
	{
		public FromMsdnGetter(PageDownloader pageDownloader)
		{
			_pageDownloader = pageDownloader ?? throw new ArgumentNullException(nameof(pageDownloader));
		}

		public IDictionary<string, HashSet<string>> GetInfoByQuery(string query)
		{
			var msdnSearchPage = _pageDownloader.DownloadPage(
				$"https://social.msdn.microsoft.com/Search/ru-RU?query={query}");
			var msdnUrl = GetMsdnUrlFromSearchPage(msdnSearchPage);

			var name = "";
			var description = "";
			try
			{
				var msdnPage = _pageDownloader.DownloadPage(msdnUrl);
				var parsedMsdnPage = new HtmlParser().Parse(msdnPage);

				var githubUrl = GetGithubUrlFromMsdnPage(parsedMsdnPage)
					.Replace("blob", "raw");
				var githubPage = _pageDownloader.DownloadPage(githubUrl);
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

		private readonly PageDownloader _pageDownloader;

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
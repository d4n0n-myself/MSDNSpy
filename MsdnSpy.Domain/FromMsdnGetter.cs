using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MsdnSpy.Domain
{
	public class FromMsdnGetter : IInfoGetter
	{
		public FromMsdnGetter(WebClient webClient)
		{
			_webClient = webClient ?? throw new ArgumentNullException(nameof(webClient));
			_webClient.Headers["User-Agent"] =
				"Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
				"(compatible; MSIE 6.0; Windows NT 5.1; " +
				".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
		}

		public IDictionary<string, string> GetInfoByQuery(string query)
		{
			var urlToXml =
				"https://raw.githubusercontent.com/dotnet/dotnet-api-docs/master/" + FindPathToXmlFile(query);
			var msdnLink = GetMsdnUrl(
				$"https://social.msdn.microsoft.com/Search/ru-RU?query={query}&pgArea=header&emptyWatermark=true&ac=4");

            var result = DownloadAndParseXml(urlToXml);
            result["msdnLink"] = msdnLink;

            return result;
		}

		private string FindPathToXmlFile(string query)
		{
			var fullQuery = $"https://api.github.com/search/code?q={query}+in:path+repo:dotnet/dotnet-api-docs";
			var jsonResponse = _webClient.DownloadString(fullQuery);
			var apiResult = JsonConvert.DeserializeObject<GithubApiResult>(jsonResponse);
			foreach (var item in apiResult.Items)
			{
				var name = (string)item["name"];
				if (name.EndsWith(".xml"))
					return (string)item["path"];
			}
			throw new InvalidOperationException("No supported information was found.");
		}

		private IDictionary<string, string> DownloadAndParseXml(string queryToXml)
		{
			var reader = new XmlTextReader(queryToXml);
			var document = XDocument.Load(reader);
			var elements = new Dictionary<string, string>();

			GetElementsFromXml(document.Root, "", elements);

			return elements;
		}

		private void GetElementsFromXml(
			XElement parentElement,
			string parentName,
			Dictionary<string, string> elements)
		{
			foreach (var element in parentElement.Elements())
			{
				var elementName = element.Name.LocalName;

				if (elementName.Contains("Member") ||
					 elementName.Contains("Assembly"))
					continue; // TODO

				if (elementName.Contains("Signature") ||
					 elementName.Contains("see") ||
					 elementName.Contains("Parameter"))
					continue; // TODO

				if (elementName.Contains("param"))
					continue; // TODO

				var fullName = string.IsNullOrEmpty(parentName)
					? elementName
					: parentName + "." + elementName;
				elements[fullName] = element.Value;

				GetElementsFromXml(element, fullName, elements);

				if (element.HasAttributes)
				{
					var attributes = element.Attributes()
						.Select(x => new StringBuilder()
							.Append(x.Name.ToString())
							.Append(' ')
							.Append(x.Value))
						.Aggregate(new StringBuilder(), (x, y) => x.Append(y))
						.ToString();
					elements[fullName] = attributes;
				}
			}
		}

		private string GetMsdnUrl(params string[] args)
		{
			if (args == null || args.Length == 0)
				throw new ArgumentException("Specify the URI of the resource to retrieve.", nameof(args));

			using (var data = _webClient.OpenRead(args[0]))
			using (var reader = new StreamReader(data))
			{
				var content = reader.ReadToEnd();
				var startIndex = content.IndexOf("var results = ", StringComparison.Ordinal) + "var results = ".Length;
				var endIndex = content.IndexOf("results.data", StringComparison.Ordinal) - 22; // what is 22?!
				content = content.Substring(startIndex, endIndex - startIndex + 1);

				var urls = new List<string>();
				var index = content.IndexOf("\"url\"", StringComparison.Ordinal);
				while (index != -1)
				{
					var offset = index + 7;
					var length = content.IndexOf('\"', offset) - offset; // what is 7?!
					urls.Add(content.Substring(offset, length));
					index = content.IndexOf("\"url\"", index + 1, StringComparison.Ordinal);
				}

				return urls.First(x => x != "" && !x.StartsWith("https://services"));
			}
		}

		private readonly WebClient _webClient;
	}
}
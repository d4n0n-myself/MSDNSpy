using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace MsdnSpy.Domain
{
    public class InfoParser
    {
        public static string FindXmlFilePath(string telegramQuery)
        {
            var client = new WebClient();
            client.Headers["User-Agent"] =
                "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                "(compatible; MSIE 6.0; Windows NT 5.1; " +
                ".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            var fullQuery = $"https://api.github.com/search/code?q={telegramQuery}+in:path+repo:dotnet/dotnet-api-docs";
            var jsonResponse =
                client.DownloadString(fullQuery);
            var gApiResult = JsonConvert.DeserializeObject<GithubApiResult>(jsonResponse);
            foreach (var item in gApiResult.Items)
            {
                var name = (string) item["name"];
                if (name.EndsWith(".xml"))
                    return (string) item["path"];
            }
             throw new InvalidOperationException("No supported information was found.");
        }
        
        public static IDictionary<string,string> XmlParser(string queryToXml)
        {
            var reader = new XmlTextReader(queryToXml);
            var doc = XDocument.Load(reader);
            var dic = new Dictionary<string,string>();
			
            GetElements(doc.Root, dic);

            return dic;
        }

        private static void GetElements(XElement element, Dictionary<string, string> dic)
        {
            foreach (var el in element.Elements())
            {
                if(el.Name.LocalName.Contains("Member") || el.Name.LocalName.Contains("Assembly")) continue; // TODO
				
                if (el.Name.LocalName.Contains("Signature") ||
                    el.Name.LocalName.Contains("see") ||
                    el.Name.LocalName.Contains("Parameter"))
                    continue; // TODO
                foreach (var innerEl in el.Elements())
                {
                    if(innerEl.Name.LocalName.Contains("param")) continue; // TODO
                    GetElements(innerEl, dic);
                    var name = el.Name.LocalName + "." + innerEl.Name.LocalName;
                    dic.Add(name, innerEl.Value);
                }

                var xw = "";
                if(el.HasAttributes)
                    xw = el.Attributes().Select(x => $"{x.Name} {x.Value}").Aggregate((x, y) => x + " " + y);
                dic.Add(el.Name.LocalName, xw);
            }
        }

    }
}
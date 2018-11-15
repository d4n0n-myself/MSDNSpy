using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MsdnSpy.Bot
{
    public class Test
    {
        public static string GetMsdnUrl(params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new ApplicationException("Specify the URI of the resource to retrieve.");
            }

            return Parser2(args);
        }

        private static string Parser2(string[] args) // lilbit faster
        {
            var client = new WebClient();
            using (var data = client.OpenRead(args[0]))
            using (var reader = new StreamReader(data))
            {
                var s = reader.ReadToEnd();
                var startIndex = s.IndexOf("var results = ", StringComparison.Ordinal) + 14;
                var endIndex = s.IndexOf("results.data", StringComparison.Ordinal) - 22;
                var ss = s.Substring(startIndex, endIndex - startIndex + 1);
                var index = ss.IndexOf("\"url\"", StringComparison.Ordinal);
                var xx = new List<string>();
                while (index != -1)
                {
                    var length = ss.IndexOf('\"', index + 7) - index - 7;
                    xx.Add(ss.Substring(index + 7, length));
                    index = ss.IndexOf("\"url\"", index + 1, StringComparison.Ordinal);
                }

               // xx.Remove("");
                xx.RemoveAll(x => x == "" || x.StartsWith("https://services"));
                return xx[0];
            }
        }
        
        private static string Parser(string[] args)
        {
            var client = new WebClient();
            using (var data = client.OpenRead(args[0]))
            using (var reader = new StreamReader(data))
            {
                var s = reader.ReadToEnd();
                var startIndex = s.IndexOf("var results = ", StringComparison.Ordinal) + 14;
                var endIndex = s.IndexOf("results.data", StringComparison.Ordinal) - 22;
                var ss = s.Substring(startIndex, endIndex - startIndex + 1);                
                var sx = JsonConvert.DeserializeObject<Dictionary<string, object>>(ss);
                var urls = sx["data"].ToString().Replace("\n", "");
                var sss = JsonConvert.DeserializeObject<Dictionary<string, object>>(urls);
                var x = JsonConvert.DeserializeObject<object[]>(sss["results"].ToString());
                var url = JsonConvert.DeserializeObject<Dictionary<string, object>>(x[0].ToString())["url"].ToString();
                return url;
            }
        }

        public static string MicrosoftDocsParser(params string[] args)
        {
            
            // TODO
            var client = new WebClient();
//            using (var data = client.OpenRead(args[0]))
//            using (var reader = new StreamReader(data))
//            {
//            }
            //client.DownloadFile("https://docs.microsoft.com/ru-ru/dotnet/api/system.web.modelbinding.imodelbinder","imodelbinder.html");
//client.DownloadFile("https://docs.microsoft.com/ru-ru/search/index?search=IModelBinder&scope=.NET","docs_search.html");
            return string.Empty;
        }
    }
}

//        private static string FastParser(string[] args) => дольше чем другой
//        {
//            var client = new WebClient();
//            using (var data = client.OpenRead(args[0]))
//            using (var reader = new StreamReader(data))
//            {
//                var s = reader.ReadToEnd();
//
//                var indexOf = s.IndexOf("<script type=\"text/javascript\">", StringComparison.Ordinal);
//                var readyStr = s.Insert(indexOf + 7, " id=\"FuncingMSDN\"");
//                var xxxx = new HtmlDocument();
//                xxxx.LoadHtml(readyStr);
//                var funcingMSDN = xxxx.GetElementbyId("FuncingMSDN").OuterHtml;
//                var startIndex = funcingMSDN.IndexOf("var results = ", StringComparison.Ordinal) + 14;
//                var endIndex = funcingMSDN.IndexOf("results.data", StringComparison.Ordinal) - 22;
//                var ss = funcingMSDN.Substring(startIndex, endIndex - startIndex + 1);
//                var sx = JsonConvert.DeserializeObject<Dictionary<string, object>>(ss);
//                var urls = sx["data"].ToString().Replace("\n", "");
//                var sss = JsonConvert.DeserializeObject<Dictionary<string, object>>(urls);
//                var x = JsonConvert.DeserializeObject<object[]>(sss["results"].ToString());
//                var url = JsonConvert.DeserializeObject<Dictionary<string, object>>(x[0].ToString())["url"].ToString();
//                return url;
//            }
//        }
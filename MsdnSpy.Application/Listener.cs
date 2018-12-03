using System;
using System.Net;
using System.Linq;
using System.Text;
using MsdnSpy.Domain;

namespace MsdnSpy.Application
{
    public class Listener
    {
        private readonly HttpListener _httpListener;
        private bool _isListening;
        private readonly string _url;

        private Listener(string url)
        {
            _url = url;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
            _httpListener.Start();
        }

        internal static Listener RunNew()
        {
            var listener = new Listener($"http://localhost:1234/");
            listener.Run();
            return listener;
        }

        private void Run()
        {
            if (_isListening) return;
            _isListening = true;
            Console.WriteLine($"Now listening on {_url}");

            while (true)
            {
                var context1 = _httpListener.GetContext(); // work
                //var context = await _httpListener.GetContextAsync(); // doesn't work ?   
                var parameterStr = context1.Request.QueryString["query"];
                var queryToXml =
                    "https://raw.githubusercontent.com/dotnet/dotnet-api-docs/master/" +
                    InfoParser.FindXmlFilePath(parameterStr);
                var msdnLink =
                    Test.GetMsdnUrl(
                        $"https://social.msdn.microsoft.com/Search/ru-RU?query={parameterStr[1]}&pgArea=header&emptyWatermark=true&ac=4");
                var parsedXml = InfoParser.XmlParser(queryToXml);
                var buffer = Encoding.UTF8.GetBytes(parsedXml["Docs.summary"]).Concat(Encoding.UTF8.GetBytes(msdnLink)).ToArray();
                using (var outputStream = context1.Response.OutputStream)
                    outputStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
using System;
using System.IO;
using System.Net;

namespace MsdnSpy.Application
{
    public class Listener
    {
        public Listener(string url)
        {
            _url = url;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
        }

        public static Listener RunNew()
        {
            var listener = new Listener($"http://localhost:1234/");
            listener.Run();
            return listener;
        }

        public void Run()
        {
            if (_isListening)
                return;

            _isListening = true;
            try
            {
                _httpListener.Start();
                Console.WriteLine($"Now listening on {_url}");

                while (true)
                {
                    var context = _httpListener.GetContext();
                    using (var output = new StreamWriter(context.Response.OutputStream))
                        output.WriteLine("Not Implemented.");
                }
            }
            finally
            {
                _isListening = false;
            }
        }

        private bool _isListening;

        private readonly HttpListener _httpListener;
        private readonly string _url;
    }
}
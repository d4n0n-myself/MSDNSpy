using System;
using System.Net;
using System.Text;

namespace MsdnSpy.Server
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
//                var context = await _httpListener.GetContextAsync(); // doesn't work ? 
                var buffer = Encoding.UTF8.GetBytes("Not Implemented.");
                using (var outputStream = context1.Response.OutputStream)
                    outputStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
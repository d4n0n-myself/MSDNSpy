using MsdnSpy.Domain;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using MsdnSpy.Infrastructure;

namespace MsdnSpy.Application
{
    public class Listener
    {
        public Listener(string url)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
            _url = url;
        }

        public static Listener RunNew(string url)
        {
            var listener = new Listener(url);
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

                    var result = HandleRequest(context);

                    var jsonResult = JsonConvert.SerializeObject(result);
                    using (var output = new StreamWriter(context.Response.OutputStream))
                        output.Write(jsonResult);
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
        private static readonly DatabaseContext _databaseContext = new DatabaseContext();
        private static readonly UserPreferencesRepository _storage = new UserPreferencesRepository(_databaseContext);
        
        private object HandleRequest(HttpListenerContext context)
        {
            try
            {
                return context.Request.QueryString["query"] == null
                                    ? HandlePreferencesRequest(context)
                                    : HandleDocumentationRequest(context);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessage = e.ToString();

                Console.WriteLine($"{DateTime.UtcNow}: {errorMessage}");
                return errorMessage;
            }
        }

        private object HandleDocumentationRequest(HttpListenerContext context)
        {
            var query = context.Request.QueryString["query"];
            Console.WriteLine($"{DateTime.UtcNow}: Received query {query}");
            var infoGetter = new FromMsdnGetter(new WebClient());

            var result = infoGetter.GetInfoByQuery(query);

            Console.WriteLine($"{DateTime.UtcNow}: Handled query {query}");
            return result;
        }

        private object HandlePreferencesRequest(HttpListenerContext context)
        {
            var chatId = 0L;
            try
            {
                chatId = long.Parse(context.Request.QueryString["chatId"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("blya pizdec ebanyi");
            }
            var category = context.Request.QueryString["category"];
            Console.WriteLine($"{DateTime.UtcNow}: Received preferences change : {category}");
            Console.WriteLine($"{DateTime.UtcNow}: Handled preferences change : {category}");
            _storage.AddCategory(chatId, category);
            return true;
            //throw new NotImplementedException();
        }
    }

}
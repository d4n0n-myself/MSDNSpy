using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace MsdnSpy.Domain
{
    public class Test
    {
        public static string GetMsdnUrl(params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new ApplicationException("Specify the URI of the resource to retrieve.");
            }

            return Parser(args);
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
                var index = ss.IndexOf("\"url\"", StringComparison.Ordinal);
                var xx = new List<string>();
                while (index != -1)
                {
                    var length = ss.IndexOf('\"', index + 7) - index - 7;
                    xx.Add(ss.Substring(index + 7, length));
                    index = ss.IndexOf("\"url\"", index + 1, StringComparison.Ordinal);
                }

                xx.RemoveAll(x => x == "" || x.StartsWith("https://services"));
                return xx[0];
            }
        }
    }
}
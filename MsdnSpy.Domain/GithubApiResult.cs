using System.Collections.Generic;
using Newtonsoft.Json;

namespace MsdnSpy.Domain
{
    public class GithubApiResult
    {
        [JsonProperty("total_count")]
        public int total_count;
        [JsonProperty("incomplete_results")]
        public bool incomplete_results;
        [JsonProperty("items")]
        public List<Dictionary<string,object>> items;
    }
}
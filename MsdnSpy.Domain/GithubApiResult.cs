using System.Collections.Generic;
using Newtonsoft.Json;

namespace MsdnSpy.Domain
{
    public class GithubApiResult
    {
        [JsonProperty("total_count")]
        public int TotalCount;
        [JsonProperty("incomplete_results")]
        public bool IncompleteResults;
        [JsonProperty("items")]
        public List<Dictionary<string, object>> Items;
    }
}
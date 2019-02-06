

namespace ElasticsearchService.ViewModels
{
    public class ES_Count
    {
        public int count { get; set; }
        public ES_CountShards _shards { get; set; }
        public class ES_CountShards
        {
            public int total { get; set; }
            public int successful { get; set; }
            public int failed { get; set; }
        }
    }
}

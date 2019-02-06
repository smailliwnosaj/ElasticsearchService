
namespace ElasticsearchService.ViewModels
{
    public class ES_StatusInfo
    {
        //public int status { get; set; }
        public string name { get; set; }
        public string cluster_name { get; set; }
        public ES_VersionInfo version { get; set; }
        public string tagline { get; set; }

        public class ES_VersionInfo
        {
            public string number { get; set; }
            public string build_hash { get; set; }
            public string build_timestamp { get; set; }
            public bool build_snapshot { get; set; }
            public string lucene_version { get; set; }
        }

    }

}

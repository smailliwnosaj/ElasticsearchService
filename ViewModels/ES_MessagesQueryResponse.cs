
using System.Collections.Generic;


namespace ElasticsearchService.ViewModels
{
    public class ES_MessagesQueryResponse
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public ES_MessagesQueryResponse_HitGroup hits { get; set; }

        public class ES_MessagesQueryResponse_HitGroup
        {
            public int total { get; set; }
            public double? max_score { get; set; }
            public List<ES_MessagesQueryResponse_Hit> hits { get; set; }

            public class ES_MessagesQueryResponse_Hit
            {
                public string _index { get; set; }
                public string _type { get; set; }
                public string _id { get; set; }
                public double? _score { get; set; }
                public ES_Message _source { get; set; }
            }
        }
    }
}


using System.Collections.Generic;

namespace ES.ViewModels
{
    public class ES_CallsQueryResponse
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public ES_CallsQueryResponse_HitGroup hits { get; set; }

        public class ES_CallsQueryResponse_HitGroup
        {
            public int total { get; set; }
            public double? max_score { get; set; }
            public List<ES_CallsQueryResponse_Hit> hits { get; set; }

            public class ES_CallsQueryResponse_Hit
            {
                public string _index { get; set; }
                public string _type { get; set; }
                public string _id { get; set; }
                public double? _score { get; set; }
                public ES_Call _source { get; set; }
            }
        }
    }
}

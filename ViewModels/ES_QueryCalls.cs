
using ElasticsearchService.Models;
using System;
using System.Collections.Generic;

namespace ElasticsearchService.ViewModels
{
    public class ES_QueryCalls
    {
        protected ES_QueryCalls() { }

        public ES_QueryCalls(ES_Sort.By sortBy, int maxRecords, int pageIndex, DateTime? utcDateTime, DateTime? endUtcDateTime, string transcriptionText)
        {
            query = new ES_QueryCalls_Query() { };
            query.@bool = new ES_QueryCalls_Query.ES_QueryCalls_Bool() { };
            size = maxRecords;
            from = pageIndex;
            sort = ES_Sort.GetQuerySort(sortBy);

            // DateTime Range
            if (utcDateTime != null && endUtcDateTime != null)
            {
                query.@bool.filter = new ES_QueryCalls_Query.ES_QueryCalls_Bool.ES_QueryCalls_Filter()
                {
                    range = new ES_QueryCalls_Query.ES_QueryCalls_Bool.ES_QueryCalls_Filter.ES_QueryCalls_Range()
                    {
                        UtcDateTime = new ES_QueryCalls_Query.ES_QueryCalls_Bool.ES_QueryCalls_Filter.ES_QueryCalls_Range.ES_QueryCalls_StartDateTime() { }
                    }
                };
                query.@bool.filter.range.UtcDateTime.SetGreaterThanDateTime(utcDateTime);
                query.@bool.filter.range.UtcDateTime.SetLessThanDateTime(endUtcDateTime);
            }

            // Transcription Text
            if (transcriptionText != null)
            {
                query.@bool.should = new ES_QueryCalls_Query.ES_QueryCalls_Bool.ES_QueryCalls_Should()
                {
                    multi_match = new ES_QueryCalls_Query.ES_QueryCalls_Bool.ES_QueryCalls_Should.ES_QueryCalls_MultiMatch()
                    {
                        query = transcriptionText,
                        fields = new List<string>() { "Transcriptions" }
                    }
                };
            }
        }

        public ES_QueryCalls_Query query { get; set; }

        public int size { get; set; }
        public int from { get; set; }

        public List<object> sort { get; set; }

        public class ES_QueryCalls_Query
        {
            public ES_QueryCalls_Bool @bool { get; set; }

            public class ES_QueryCalls_Bool
            {
                public ES_QueryCalls_Should should { get; set; }

                public ES_QueryCalls_Filter filter { get; set; }

                public class ES_QueryCalls_Should
                {
                    public ES_QueryCalls_MultiMatch multi_match { get; set; }
                    public class ES_QueryCalls_MultiMatch
                    {
                        public List<string> fields { get; set; }
                        public string query { get; set; }
                    }
                }

                public class ES_QueryCalls_Filter
                {
                    public ES_QueryCalls_Range range { get; set; }

                    public class ES_QueryCalls_Range
                    {
                        public ES_QueryCalls_StartDateTime UtcDateTime { get; set; }

                        public class ES_QueryCalls_StartDateTime
                        {
                            private string _gte { get; set; }
                            public void SetGreaterThanDateTime(DateTime? dt)
                            {
                                _gte = (dt ?? DateTime.UtcNow.AddMonths(-1)).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                            }

                            private string _lte { get; set; }
                            public void SetLessThanDateTime(DateTime? dt)
                            {
                                _lte = (dt ?? DateTime.UtcNow).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                            }

                            public string format = "strict_date_time";

                            public string gte { get { return _gte; } }

                            public string lte { get { return _lte; } }
                        }
                    }
                }
            }
        }
    }
}

//{  
//   "query":{  
//      "bool":{  
//         "should":{  
//            "multi_match":{  
//               "fields":[
//                  "Transcriptions"
//               ],
//               "query":"transcriptioned^2"
//            }
//         },
//         "filter":{  
//            "range":{  
//               "UtcDateTime":{  
//                  "format":"strict_date_time",
//                  "gte":"2019-01-27T11:04:17.992Z",
//                  "lte":"2019-02-01T11:04:17.992Z"
//               }
//            }
//         }
//      }
//   },
//   "size":1000,
//   "from":0,
//   "sort":[
//      {  
//         "_score":"asc"
//      },
//      {  
//         "UtcDateTime":"desc"
//      }
//   ]
//}
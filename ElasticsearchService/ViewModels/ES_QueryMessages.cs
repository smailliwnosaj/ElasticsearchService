
using ElasticsearchService.Models;
using System;
using System.Collections.Generic;

namespace ElasticsearchService.ViewModels
{
    public class ES_QueryMessages
    {
        protected ES_QueryMessages() { }

        public ES_QueryMessages(ES_Sort.By sortBy, int maxRecords, int pageIndex, DateTime? utcDateTime, DateTime? endUtcDateTime, string messageText)
        {
            query = new ES_QueryMessages_Query() { };
            query.@bool = new ES_QueryMessages_Query.ES_QueryMessages_Bool() { };
            size = maxRecords;
            from = pageIndex;
            sort = ES_Sort.GetQuerySort(sortBy);

            // DateTime Range
            if (utcDateTime != null && endUtcDateTime != null)
            {
                query.@bool.filter = new ES_QueryMessages_Query.ES_QueryMessages_Bool.ES_QueryMessages_Filter()
                {
                    range = new ES_QueryMessages_Query.ES_QueryMessages_Bool.ES_QueryMessages_Filter.ES_QueryMessages_Range()
                    {
                        UtcDateTime = new ES_QueryMessages_Query.ES_QueryMessages_Bool.ES_QueryMessages_Filter.ES_QueryMessages_Range.ES_QueryMessages_DateTime() { }
                    }
                };
                query.@bool.filter.range.UtcDateTime.SetGreaterThanDateTime(utcDateTime);
                query.@bool.filter.range.UtcDateTime.SetLessThanDateTime(endUtcDateTime);
            }

            // Transcription Text
            if (messageText != null)
            {
                query.@bool.should = new ES_QueryMessages_Query.ES_QueryMessages_Bool.ES_QueryMessages_Should()
                {
                    multi_match = new ES_QueryMessages_Query.ES_QueryMessages_Bool.ES_QueryMessages_Should.ES_QueryMessages_MultiMatch()
                    {
                        query = messageText,
                        fields = new List<string>() { "Text" }
                    }
                };
            }
        }

        public ES_QueryMessages_Query query { get; set; }

        public int size { get; set; }
        public int from { get; set; }

        public List<object> sort { get; set; }

        public class ES_QueryMessages_Query
        {
            public ES_QueryMessages_Bool @bool { get; set; }

            public class ES_QueryMessages_Bool
            {
                public ES_QueryMessages_Should should { get; set; }

                public ES_QueryMessages_Filter filter { get; set; }

                public class ES_QueryMessages_Should
                {
                    public ES_QueryMessages_MultiMatch multi_match { get; set; }
                    public class ES_QueryMessages_MultiMatch
                    {
                        public List<string> fields { get; set; }
                        public string query { get; set; }
                    }
                }

                public class ES_QueryMessages_Filter
                {
                    public ES_QueryMessages_Range range { get; set; }

                    public class ES_QueryMessages_Range
                    {
                        public ES_QueryMessages_DateTime UtcDateTime { get; set; }

                        public class ES_QueryMessages_DateTime
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
//                  "Text"
//               ],
//               "query":"anothered^2"
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

using ES.Models;
using System;
using System.Collections.Generic;

namespace ES.ViewModels
{
    public class ES_Query
    {
        protected ES_Query() { }

        public ES_Query(ES_Sort.By sortBy, int maxRecords, int pageIndex, DateTime? utcDateTime, DateTime? endUtcDateTime, Models.ES_Query objQuery, List<string> fields, short fuzziness, short slop)
        {
            query = new ES_Query_Query() { };
            query.@bool = new ES_Query_Bool() { };
            size = maxRecords;
            from = pageIndex;
            sort = ES_Sort.GetQuerySort(sortBy);

            query.@bool.minimum_should_match = 0; // Setting 0 will return results with no matches.  But, we increment this later in code.

            // DateTime Range
            if (utcDateTime != null && endUtcDateTime != null)
            {
                query.@bool.filter = new ES_Query_Filter()
                {
                    range = new ES_Query_Filter.ES_Query_Range()
                    {
                        UtcDateTime = new ES_Query_Filter.ES_Query_Range.ES_Query_StartDateTime() { }
                    }
                };
                query.@bool.filter.range.UtcDateTime.SetGreaterThanDateTime(utcDateTime);
                query.@bool.filter.range.UtcDateTime.SetLessThanDateTime(endUtcDateTime);
            }

            // Transcription Text
            if (objQuery != null)
            {
                //var objQuery = new ES_TranscriptionQueryService(textQuery).GetQuery();
                // INCLUDE TERMS
                ProcessList(objQuery.IncludeTerms, true, false, fields, fuzziness, slop);
                // EXCLUDE TERMS
                ProcessList(objQuery.ExcludeTerms, false, false, fields, fuzziness, slop);
                // INCLUDE PHRASES
                ProcessList(objQuery.IncludePhrases, true, true, fields, fuzziness, slop);
                // EXCLUDE PHRASES
                ProcessList(objQuery.ExcludePhrases, false, true, fields, fuzziness, slop);
            }
        }

        private void ProcessList(List<string> list, bool isShould, bool isPhrase, List<string> fields, short fuzziness, short slop)
        {
            if (list != null && list.Count > 0)
            {
                if (isShould)
                {
                    // SHOULD
                    query.@bool.should = query.@bool.should ?? new List<ES_Query_Should>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (isPhrase)
                        {
                            // PHRASE
                            query.@bool.should.Add(new ES_Query_Should()
                            {
                                multi_match = new ES_Query_MultiMatch_Phrase()
                                {
                                    query = list[i],
                                    fields = fields,
                                    slop = slop,
                                    type = "phrase"
                                }
                            });
                        }
                        else
                        {
                            // TERM
                            query.@bool.should.Add(new ES_Query_Should()
                            {
                                multi_match = new ES_Query_MultiMatch_Term()
                                {
                                    query = list[i],
                                    fields = fields,
                                    fuzziness = fuzziness
                                }
                            });
                        }
                        query.@bool.minimum_should_match++; // Requires a match on each term
                    }
                }
                else
                {
                    // MUST NOT
                    query.@bool.must_not = query.@bool.must_not ?? new List<ES_Query_MustNot>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (isPhrase)
                        {
                            // PHRASE
                            query.@bool.must_not.Add(new ES_Query_MustNot()
                            {
                                multi_match = new ES_Query_MultiMatch_Phrase()
                                {
                                    query = list[i],
                                    fields = fields,
                                    slop = slop,
                                    type = "phrase"
                                }
                            });
                        }
                        else
                        {
                            // TERM
                            query.@bool.must_not.Add(new ES_Query_MustNot()
                            {
                                multi_match = new ES_Query_MultiMatch_Term()
                                {
                                    query = list[i],
                                    fields = fields,
                                    fuzziness = fuzziness
                                }
                            });
                        }
                        //query.@bool.minimum_should_match++; // Requires a match on each term
                    }
                }
            }
        }

        public ES_Query_Query query { get; set; }

        public int size { get; set; }
        public int from { get; set; }

        public List<object> sort { get; set; }

        public class ES_Query_Query
        {
            public ES_Query_Bool @bool { get; set; }
        }

        public class ES_Query_Bool
        {
            public List<ES_Query_Should> should { get; set; }
            public ES_Query_Filter filter { get; set; }
            public List<ES_Query_MustNot> must_not { get; set; }

            public short minimum_should_match { get; set; }
        }

        public class ES_Query_Should
        {
            public object multi_match { get; set; }
        }

        public class ES_Query_MustNot
        {
            public object multi_match { get; set; }
        }

        public class ES_Query_MultiMatch_Term
        {
            public List<string> fields { get; set; }
            public string query { get; set; }
            public short fuzziness { get; set; }
        }

        public class ES_Query_MultiMatch_Phrase
        {
            public List<string> fields { get; set; }
            public string query { get; set; }
            public string type { get; set; }
            public short slop { get; set; }
        }

        public class ES_Query_Filter
        {
            public ES_Query_Range range { get; set; }

            public class ES_Query_Range
            {
                public ES_Query_StartDateTime UtcDateTime { get; set; }

                public class ES_Query_StartDateTime
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

//{  
//   "query":{  
//      "bool":{  
//         "should":[
//            {  
//               "multi_match":{  
//                  "fields":[
//                     "Transcriptions"
//                  ],
//                  "query":"Duke",
//                  "fuzziness":1
//               }
//            },
//            {  
//               "multi_match":{  
//                  "fields":[
//                     "Transcriptions"
//                  ],
//                  "query":"Normandy",
//                  "fuzziness":1
//               }
//            },
//            {  
//               "multi_match":{  
//                  "fields":[
//                     "Transcriptions"
//                  ],
//                  "query":"Aquitaine Duke",
//                  "fuzziness":1
//               }
//            },
//            {  
//               "multi_match":{  
//                  "fields":[
//                     "Transcriptions"
//                  ],
//                  "query":"Normandy Duke",
//                  "type":"phrase",
//                  "slop":3
//               }
//            }
//         ],
//         "filter":{  
//            "range":{  
//               "UtcDateTime":{  
//                  "format":"strict_date_time",
//                  "gte":"2019-01-01T00:00:00.000Z",
//                  "lte":"2019-02-21T23:59:59.000Z"
//               }
//            }
//         },
//         "must_not":[
//            {  
//               "multi_match":{  
//                  "fields":[
//                     "Transcriptions"
//                  ],
//                  "query":"television",
//                  "fuzziness":1
//               }
//            },
//            {  
//               "multi_match":{  
//                  "fields":[
//                     "Transcriptions"
//                  ],
//                  "query":"The Tonight Show",
//                  "type":"phrase",
//                  "slop":3
//               }
//            }
//         ],
//         "minimum_should_match":4
//      }
//   },
//   "size":2000,
//   "from":0,
//   "sort":[
//      {  
//         "_score":"desc"
//      },
//      {  
//         "UtcDateTime":"desc"
//      }
//   ]
//}
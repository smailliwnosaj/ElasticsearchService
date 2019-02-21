# ElasticsearchService
Elasticsearch service C# Visual Studio project with two predefined entities: POST mappings, PUT doc async, GET doc(s) by id(s) async, GET docs by query async, and more.

-----------------------------------------------------------------------------------------------------------------------

STEP 1: Download and extract this archive
https://github.com/smailliwnosaj/ElasticsearchService

STEP 2: Open Projects.sln file in Visual Studio

STEP 3: Build solution in Visual Studio

STEP 4: Download and install Elasticsearch 6 (tested on 6.5.1)

STEP 5: Run or Debug all tests from Visual Studio.

STEP 6:  Leave praise on my GitHub project.  

Thanks and enjoy!!!

-------------------------------------------------------------------------------------------------------------------------

Elasticsearch is an open source search engine which is available as a service running on your local machine, as a server running on your network, as a multi-node server running on a high availability network, or as a cloud service on popular platforms like AWS.  It is used by Facebook and Twitter and many other sites looking to get search result data quickly.

When building my project, I used pretty much every advanced programming technique at my disposal.

- Interfaces
- Generic objects
- Polymorphism
- Inheritance
- Asynchronous programming
- Dependency Injection
- Microservices
- Unit Testing
- etc.

Although there are many thousands of Elasticsearch repos on Github, I didn't find any that would do what I wanted my service to do.  So, you may find it interesting.  And, if you haven't used Elasticsearch before, I would recommend learning it.

-------------------------------------------------------------------------------------------------------------------------

Sample Elasticsearch Query:

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



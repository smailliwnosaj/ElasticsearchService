# ElasticsearchService
Elasticsearch service C# Visual Studio project with no 3rd party dependencies (Other than Elasticsearch).  Built in unit tests allow you to run the code and experience using Elasticsearch with 0 code changes.  

- POST Elasticsearch mappings
- PUT Elasticsearch docs sync/async
- GET Elasticsearch doc(s) by id(s) sync/async
- GET Elasticsearch docs by query sync/async 
- and much more.

This version supports combinations of queries with 0 or many terms and 0 or many phrases with contains/not contains for each string.  Each term can have a fuzziness factor applied...covering misspelled words.  Each phrase can have a slop factor applied...covering phrases where the words are out of order.  Each query can be applied to one or many fields of an Elasticsearch entity.

-----------------------------------------------------------------------------------------------------------------------

STEP 1: Download and extract this archive

STEP 2: Open Projects.sln file in Visual Studio

STEP 3: Build solution in Visual Studio

STEP 4: Download and install Elasticsearch 6 (tested on 6.5.1)

STEP 5: Run or Debug all Visual Studio unit tests.

STEP 6:  Leave praise on my GitHub project.  

Thanks and enjoy!!!

-------------------------------------------------------------------------------------------------------------------------

Elasticsearch is an open source search engine available as a service running on your local machine, as a server running on your network, as a multi-node server running on a high availability network, or as a cloud service on popular platforms like Amazon AWS.  It is used by Facebook and Twitter and many other sites looking to get search result data quickly.

When building this project, I used pretty much every advanced programming technique at my disposal.

- Interfaces
- Generic objects
- Polymorphism
- Inheritance
- Asynchronous programming
- Dependency Injection
- Microservices
- Unit Testing
- etc.

Although there are many thousands of Elasticsearch repos on Github, I didn't find any that would do what I wanted this service to do.  So, I built my own.

-------------------------------------------------------------------------------------------------------------------------






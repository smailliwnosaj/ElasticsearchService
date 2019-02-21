using ES.Models;
using ES.ViewModels;
//using Logging;
using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace ES.Base_Classes
{
    public abstract class ES_DataService
    {
        protected ES_Config Config { get; set; }

        protected ES_DataService(ES_ClusterName clusterName)
        {
            initialize(null, clusterName);
        }

        protected ES_DataService(ES_Config config, ES_ClusterName clusterName)
        {
            initialize(config, clusterName);
        }

        private void initialize(ES_Config config, ES_ClusterName clusterName)
        {
            if (config != null)
            {
                this.Config = config;
            }
            else
            {
                this.Config = new ES_Config()
                {
                    BasePath = "http://localhost:9200", // TODO: Get this value from the Config Portal
                    ClusterName = clusterName.ToString(),
                    ClassName = (
                        (clusterName == ES_ClusterName.calls) ? "ES_CallService" : 
                        (clusterName == ES_ClusterName.messages) ? "ES_MessageService" : 
                        "ES_ScreenService"
                    ),
                    PutMappingsPath = "/" + clusterName.ToString() + "_{0}",
                    PutDocumentPath = "/" + clusterName.ToString() + "_{0}/_doc/{1}",
                    GetDocumentByIdPath = "/" + clusterName.ToString() + "_{0}/_doc/{1}",
                    DeleteDocumentByIdPath = "/" + clusterName.ToString() + "_{0}/_doc/{1}",
                    DeleteAllByTenantIdPath = "/" + clusterName.ToString() + "_{0}",
                    GetDocumentsByIdsPath = "/" + clusterName.ToString() + "_{0}/_doc/_mget",
                    GetDocumentsByQueryPath = "/" + clusterName.ToString() + "_{0}/_doc/_search",
                    GetMappingsPath = "/" + clusterName.ToString() + "_{0}/_mapping/_doc"
                };
            }
        }

        #region Get Status with a web request

        protected ES_StatusInfo GetStatusInfo_WebRequest()
        {
            var responseJson = string.Empty;
            try
            {
                responseJson = String_WebRequest(string.Empty, null, ES_Verb.GET);

                if (!string.IsNullOrEmpty(responseJson))
                {
                    try
                    {
                        var statusInfo = new JavaScriptSerializer().Deserialize<ES_StatusInfo>(responseJson);
                        if (statusInfo != null)
                        {
                            return statusInfo;
                        }
                    }
                    catch (System.Exception ex1)
                    {
                        //LogEntry.WriteNLogEntryEx(
                        //    "ES_DataService.GetStatusInfo_WebRequest() Json deserialization failed on default path: .\n\nResponse looked like this:\n\n" + responseJson,
                        //    NLog.LogLevel.Error,
                        //    ex1,
                        //    LogEvent.EventManagerElasticsearchConnectionError
                        //);
                    }
                }
            }
            catch (System.Exception)
            {
                // Request failed exception with the pertinent information was already logged in String_WebRequest()
                //throw;
            }

            return null;
        }

        #endregion

        #region Get Mappings with a web request

        protected bool? IndexHasMappings_WebRequest<M>(int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    "ES_DataService.GetStatusInfo_WebRequest() checking for data mappings on path: " + this.Config.GetMappingsPath + ".",
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            var responseJson = string.Empty;
            try
            {
                responseJson = String_WebRequest(string.Format(this.Config.GetMappingsPath, tenantId), null, ES_Verb.GET);

                if (!string.IsNullOrEmpty(responseJson))
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        var container = serializer.Deserialize<M>(GetJsonInnerObject(responseJson));

                        // Test object mappings
                        if (container is ES_CallsMapping)
                        {
                            try
                            {
                                var callsMapping = container as ES_CallsMapping;
                                var props = callsMapping.mappings._doc.properties;
                                var hasMappings = props.Day.type == "long" &&
                                props.Number.type == "keyword" &&
                                props.Number.fields.keyword.type == "keyword" &&
                                props.FromNumber.type == "keyword" &&
                                props.FromNumber.fields.keyword.type == "keyword" &&
                                props.ToNumber.type == "keyword" &&
                                props.ToNumber.fields.keyword.type == "keyword" &&
                                props.Time.type == "date" &&
                                props.UtcDateTime.type == "date" &&
                                props.UtcDateTime_End.type == "date";

                                if (!hasMappings)
                                {
                                    //LogEntry.WriteNLogEntry(
                                    //    "ES_DataService.GetStatusInfo_WebRequest() checking for data mappings on path: " + this.Config.GetMappingsPath + ".\n\nElasticsearch reported mappings.  However, those mappings did not match what our system expected.  We will attempt to update the mappings.  If this warning persists, a code change will need to be made to support the version of Elasticsearch being used.",
                                    //    NLog.LogLevel.Warn,
                                    //    LogEvent.EventManagerElasticsearchGeneral
                                    //);
                                }
                                return hasMappings;
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                        else if (container is ES_MessagesMapping)
                        {
                            try
                            {
                                var messagesMapping = container as ES_CallsMapping;
                                var props = messagesMapping.mappings._doc.properties;
                                var hasMappings = props.Day.type == "long" &&
                                props.Number.type == "keyword" &&
                                props.Number.fields.keyword.type == "keyword" &&
                                props.FromNumber.type == "keyword" &&
                                props.FromNumber.fields.keyword.type == "keyword" &&
                                props.ToNumber.type == "keyword" &&
                                props.ToNumber.fields.keyword.type == "keyword" &&
                                props.Time.type == "date" &&
                                props.UtcDateTime.type == "date";

                                if (!hasMappings)
                                {
                                    //LogEntry.WriteNLogEntry(
                                    //    "ES_DataService.GetStatusInfo_WebRequest() checking for data mappings on path: " + this.Config.GetMappingsPath + ".\n\nElasticsearch reported mappings.  However, those mappings did not match what our system expected.  We will attempt to update the mappings.  If this warning persists, a code change will need to be made to support the version of Elasticsearch being used.",
                                    //    NLog.LogLevel.Warn,
                                    //    LogEvent.EventManagerElasticsearchGeneral
                                    //);
                                }
                                return hasMappings;
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                        else
                        {
                            //LogEntry.WriteNLogEntry(
                            //    "ES_DataService.GetStatusInfo_WebRequest() Json deserialization failed on checking for data mappings.  It is likely the request was made for an entity which is not supported: .\n\nResponse looked like this:\n\n" + responseJson,
                            //    NLog.LogLevel.Error,
                            //    LogEvent.EventManagerElasticsearchConnectionError
                            //);
                            return null;
                        }
                    }
                    catch (System.Exception ex1)
                    {
                        //LogEntry.WriteNLogEntryEx(
                        //    "ES_DataService.GetStatusInfo_WebRequest() Json deserialization failed on checking for data mappings: .\n\nResponse looked like this:\n\n" + responseJson,
                        //    NLog.LogLevel.Error,
                        //    ex1,
                        //    LogEvent.EventManagerElasticsearchConnectionError
                        //);
                        return null;
                    }
                }
            }
            catch (System.Exception)
            {
                // Request failed exception with the pertinent information was already logged in String_WebRequest()
                //throw;
                return null;
            }

            return false;
        }

        private string GetJsonInnerObject(string source)
        {
            var subObjectStartIndex = source.IndexOf(':');
            if (subObjectStartIndex < 1) return string.Empty;
            source = source.Trim();
            source = source.Substring(subObjectStartIndex + 1, source.Length - 2 - subObjectStartIndex);
            return source;
        }

        #endregion

        #region Get document Count

        protected ES_Count GetDocumentCount_WebRequest(int tenantid)
        {
            var responseJson = string.Empty;
            try
            {
                
                var path = ("/" + Config.ClusterName + "_" + tenantid + "/_doc/_count");
                responseJson = String_WebRequest(path, null, ES_Verb.GET);

                if (!string.IsNullOrEmpty(responseJson))
                {
                    try
                    {
                        var count = new JavaScriptSerializer().Deserialize<ES_Count>(responseJson);
                        if (count != null)
                        {
                            return count;
                        }
                    }
                    catch (System.Exception ex1)
                    {
                        //LogEntry.WriteNLogEntryEx(
                        //    "ES_DataService.GetDocumentCount_WebRequest() Json deserialization failed: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                        //    NLog.LogLevel.Error,
                        //    ex1,
                        //    LogEvent.EventManagerElasticsearchConnectionError
                        //);
                    }
                }
            }
            catch (System.Exception)
            {
                // Request failed exception with the pertinent information was already logged in String_WebRequest()
                //throw;
            }

            return null;
        }

        #endregion

        #region Web Request Methods

        // NOTE: So long as this class is being instantiated by a web request from Event Manager,
        //       we gain nothing by making web requests async.  
        protected string String_WebRequest(string rootRelativePath, string json, ES_Verb verb)
        {
            string responseJson = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.Config.BasePath + (rootRelativePath ?? string.Empty));
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Method = verb.ToString();

                if (verb == ES_Verb.PUT || verb == ES_Verb.POST && !string.IsNullOrEmpty(json))
                {
                    request.ContentType = "application/json";
                    using (var writer = new StreamWriter(request.GetRequestStream()))
                    {
                        json = (json ?? "{}");
                        //request.ContentLength = data.Length;
                        writer.Write(json);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseJson = reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                // NOTE:  If Elasticsearch cannot find a requested Cluster name or requested ID,
                //        it will return an exception.  
                //LogEntry.WriteNLogEntryEx(
                //    "ES_DataService.GetAsString_WebRequest() Request failed: " + this.Config.BasePath + (rootRelativePath ?? string.Empty) + ".\n\nResponse looked like this:\n\n" + responseJson,
                //    NLog.LogLevel.Error,
                //    ex,
                //    LogEvent.EventManagerElasticsearchConnectionError
                //);

                // log, swallow exception, return null
                //throw ex;
                return null;
            }

            return responseJson;
        }

        #endregion

        #region Setup Index

        public bool SetupIndex<M>(int tenantId) // Accepts a Generic object type 'M'
        {
            var statusInfo = GetStatusInfo_WebRequest();
            var path = string.Format(this.Config.PutMappingsPath, tenantId.ToString());

            if (statusInfo == null || string.IsNullOrEmpty(statusInfo.version.number))
            {
                //LogEntry.WriteNLogEntry(
                //    this.Config.ClassName + ".SetupIndex()\n\nError: Elasticsearch server is unreachable.",
                //    NLog.LogLevel.Error,
                //    LogEvent.EventManagerElasticsearchConnectionError
                //);
                return false;
            }

            var hasMappings = IndexHasMappings_WebRequest<M>(tenantId);
            if (hasMappings == true)
            {
                // Index already has mappings
                return true;
            }

            // Index does not have mappings or mappings were incorrect
            // Create mappings

            //LogEntry.WriteNLogEntry(
            //    string.Format(this.Config.ClassName + ".SetupIndex()\n\nBasePath: {0}\nClusterName: {1}\ntenantId: {2}", this.Config.BasePath, this.Config.ClusterName, tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            bool result = false;
            
            var objSource = (M)typeof(M).GetConstructor(new Type[] { }).Invoke(new object[] { });
            var data = new JavaScriptSerializer().Serialize(objSource);

            var responseJson = String_WebRequest(path, data, Base_Classes.ES_Verb.PUT);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var r = new JavaScriptSerializer().Deserialize<ES_Acknowledged>(responseJson);
                    if (r != null && (r.acknowledged == true))
                    {
                        //LogEntry.WriteNLogEntry(
                        //    string.Format(this.Config.ClassName + ".SetupIndex() success\n\nBasePath: {0}\nClusterName: {1}\ntenantId: {2}", this.Config.BasePath, this.Config.ClusterName, tenantId),
                        //    NLog.LogLevel.Trace,
                        //    LogEvent.EventManagerElasticsearchGeneral
                        //);

                        result = true;
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    this.Config.ClassName + ".SetupIndex() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);

                    // log, swallow exception, return false
                    //throw ex1;
                    return false;
                }
            }

            return result;
        }

        #endregion
    }

    public enum ES_ClusterName
    {
        calls,
        messages//,
        //screens
    }

    public enum ES_Verb
    {
        GET,
        PUT,
        DELETE,
        POST
    }

}

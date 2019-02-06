
using ElasticsearchService.BaseClasses;
using ElasticsearchService.Interfaces;
using ElasticsearchService.Models;
using ElasticsearchService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Elasticsearch
{
    public class ES_CallService : ES_DataService, IElesticsearchService<ES_Call, ES_QueryCalls>
    {
        public ES_CallService() : base(ES_ClusterName.calls)
        {
            initialize();
        }

        public ES_CallService(ES_Config config) : base(config, ES_ClusterName.calls)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.Initialize()\n\nBasePath: {0}\nClusterName: {1}", this.Config.BasePath, this.Config.ClusterName),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            initialize();
        }

        private void initialize() {}

        #region GetById

        /// <summary>
        /// This method returns a Task.  You must use the "await" keyword within an "async" method when calling this method.
        /// </summary>
        /// <returns></returns>
        public async Task<ES_Call> GetByIdAsync(long id, int tenantId)
        {
            var call = await Task.Run(() => GetById(id, tenantId));
            return call;
        }

        public ES_Call GetById(long id, int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.GetById()\n\nBasePath: {0}\nClusterName: {1}\ncallId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            var call = new ES_Call();

            var path = string.Format(base.Config.GetDocumentByIdPath, tenantId.ToString(), id.ToString());

            var responseJson = base.String_WebRequest(path, null, ES_Verb.GET);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var callResponse = new JavaScriptSerializer().Deserialize<ES_CallResponse>(responseJson);

                    //LogEntry.WriteNLogEntry(
                    //    string.Format("ES_CallService.GetById() success\n\nBasePath: {0}\nClusterName: {1}\ncallId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
                    //    NLog.LogLevel.Trace,
                    //    LogEvent.EventManagerElasticsearchGeneral
                    //);

                    if (callResponse.found == true)
                    {
                        call = callResponse._source;
                        call.TenantId = tenantId; // TenantId is not part of the elasticsearch document
                        call.CallId = id; // CallId is not part of the elasticsearch document
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_CallService.GetById() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    throw;
                }
            }

            return call;
        }

        #endregion

        #region GetByIds

        /// <summary>
        /// This method returns a Task.  You must use the "await" keyword within an "async" method when calling this method.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ES_Call>> GetByIdsAsync(List<long> ids, int tenantId)
        {
            var call = await Task.Run(() => GetByIds(ids, tenantId));
            return call;
        }

        public List<ES_Call> GetByIds(List<long> ids, int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.GetByIds()\n\nBasePath: {0}\nClusterName: {1}\ncallIds: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, ids.ToString(), tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            var calls = new List<ES_Call>();

            var path = string.Format(base.Config.GetDocumentsByIdsPath, tenantId.ToString());
            var query = new ES_QueryByIds();
            query.ids = new List<string>();
            query.ids.AddRange(ids.Select(x => x.ToString()).ToList());
            var json = new JavaScriptSerializer().Serialize(query);

            var responseJson = base.String_WebRequest(path, json, ES_Verb.POST);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var callResponse = serializer.Deserialize<ES_CallsResponse>(responseJson);
                    if (callResponse == null) throw new Exception("ES_GetCallService.GetByIds() failed to serialize response:" + "\n\n" + responseJson);

                    callResponse.docs = callResponse.docs.Where(x => x.found == true).ToList();

                    //LogEntry.WriteNLogEntry(
                    //    string.Format("ES_CallService.GetByIds() success\n\nBasePath: {0}\nClusterName: {1}\ncallIds: {2}\ntenantId: {3}\ncount: {4}", this.Config.BasePath, this.Config.ClusterName, ids.ToString(), tenantId, callResponse.docs.Count()),
                    //    NLog.LogLevel.Trace,
                    //    LogEvent.EventManagerElasticsearchGeneral
                    //);

                    foreach (var doc in callResponse.docs)
                    {
                        var cm = doc._source;
                        cm.TenantId = tenantId; // TenantId is not part of the elasticsearch document
                        cm.CallId = int.Parse(doc._id); // CallId is not part of the elasticsearch document
                        calls.Add(cm);
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_CallService.GetByIds() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    throw;
                }
            }

            // TODO: Sort by order provided in messageIds

            return calls;
        }

        #endregion

        #region GetByQuery

        /// <summary>
        /// This method returns a Task.  You must use the "await" keyword within an "async" method when calling this method.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ES_Call>> GetByQueryAsync(ES_QueryCalls query, int tenantId)
        {
            var call = await Task.Run(() => GetByQuery(query, tenantId));
            return call;
        }

        public List<ES_Call> GetByQuery(ES_QueryCalls query, int tenantId)
        {
            var json = new JavaScriptSerializer().Serialize(query);
            var path = string.Format(base.Config.GetDocumentsByQueryPath, tenantId.ToString());

            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.GetByQuery()\n\nBasePath: {0}\nClusterName: {1}\nquery: \n{2}\ntenantId: {3}\npath: {4}", this.Config.BasePath, this.Config.ClusterName, json, tenantId, path),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            var calls = new List<ES_Call>();
            var responseJson = base.String_WebRequest(path, json, ES_Verb.POST);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var callResponse = serializer.Deserialize<ES_CallsQueryResponse>(responseJson);
                    if (callResponse == null) throw new Exception("ES_GetCallService.GetByQuery() failed to serialize response:" + "\n\n" + responseJson);

                    //LogEntry.WriteNLogEntry(
                    //    string.Format("ES_CallService.GetByQuery() success\n\nBasePath: {0}\nClusterName: {1}\nquery: \n{2}\ntenantId: {3}\npath: {4}\ncount: {5}", this.Config.BasePath, this.Config.ClusterName, json, tenantId, path, callResponse.hits.hits.Count()),
                    //    NLog.LogLevel.Trace,
                    //    LogEvent.EventManagerElasticsearchGeneral
                    //);

                    foreach (var doc in callResponse.hits.hits)
                    {
                        var cm = doc._source;
                        cm.TenantId = tenantId; // TenantId is not part of the elasticsearch document
                        cm.CallId = int.Parse(doc._id); // CallId is not part of the elasticsearch document
                        calls.Add(cm);
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_CallService.GetByQuery() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    throw;
                }
            }

            return calls;
        }

        #endregion

        #region Get document Count

        public ES_Count GetDocumentCount(int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.GetDocumentCount()\n\nBasePath: {0}\nClusterName: {1}\ntenantId: {2}", this.Config.BasePath, this.Config.ClusterName, tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            return base.GetDocumentCount_WebRequest(tenantId);
        }

        #endregion

        #region Get server Status info

        public ES_StatusInfo GetStatusInfo()
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.GetStatusInfo()\n\nBasePath: {0}\nClusterName: {1}", this.Config.BasePath, this.Config.ClusterName),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            return base.GetStatusInfo_WebRequest();
        }

        #endregion

        #region Put document

        /// <summary>
        /// This method returns a Task.  You must use the "await" keyword within an "async" method when calling this method.
        /// </summary>
        /// <returns></returns>
        public async Task<bool?> PutAsync(ES_Call doc, long id, int tenantId)
        {
            var success = await Task.Run(() => Put(doc, id, tenantId));
            return success;
        }

        private bool? Put(ES_Call doc, long id, int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.Put()\n\nBasePath: {0}\nClusterName: {1}\ncallId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            bool? result = false;

            var path = string.Format(base.Config.PutDocumentPath, tenantId.ToString(), id.ToString());
            var data = new JavaScriptSerializer().Serialize(doc);

            var responseJson = base.String_WebRequest(path, data, ES_Verb.PUT);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var r = new JavaScriptSerializer().Deserialize<ES_Result>(responseJson);
                    if (r != null && (r.result == "created" || r.result == "updated"))
                    {
                        //LogEntry.WriteNLogEntry(
                        //    string.Format("ES_CallService.Put() success\n\nBasePath: {0}\nClusterName: {1}\ncallId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
                        //    NLog.LogLevel.Trace,
                        //    LogEvent.EventManagerElasticsearchGeneral
                        //);

                        result = true;
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_CallService.Put() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    //throw;
                    return null;
                }
            }

            return result;
        }

        #endregion

        #region Put entity mappings

        public bool SetupIndex(int tenantId)
        {
            return base.SetupIndex<ES_CallsMapping>(tenantId);
        }

        #endregion

        #region DeleteById

        /// <summary>
        /// This method returns a Task.  You must use the "await" keyword within an "async" method when calling this method.
        /// </summary>
        /// <returns></returns>
        public async Task<bool?> DeleteByIdAsync(long id, int tenantId)
        {
            var result = await Task.Run(() => DeleteById(id, tenantId));
            return result;
        }

        public bool? DeleteById(long id, int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.DeleteById()\n\nBasePath: {0}\nClusterName: {1}\ncallId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            bool? result = false;

            var path = string.Format(base.Config.DeleteDocumentByIdPath, tenantId.ToString(), id.ToString());

            var responseJson = base.String_WebRequest(path, null, ES_Verb.DELETE);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var r = new JavaScriptSerializer().Deserialize<ES_Result>(responseJson);
                    if (r != null && (r.result == "deleted"))
                    {
                        //LogEntry.WriteNLogEntry(
                        //    string.Format("ES_CallService.DeleteById() success\n\nBasePath: {0}\nClusterName: {1}\ncallId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
                        //    NLog.LogLevel.Trace,
                        //    LogEvent.EventManagerElasticsearchGeneral
                        //);

                        result = true;
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_CallService.DeleteById() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    //throw;
                    return null;
                }
            }

            return result;
        }

        #endregion

        #region DeleteAllByTenantId

        public bool? DeleteAllByTenantId(int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_CallService.DeleteAllByTenantId()\n\nBasePath: {0}\nClusterName: {1}\ntenantId: {2}", this.Config.BasePath, this.Config.ClusterName, tenantId),
            //    NLog.LogLevel.Info,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            bool? result = false;

            var path = string.Format(base.Config.DeleteAllByTenantIdPath, tenantId.ToString());

            var responseJson = base.String_WebRequest(path, null, ES_Verb.DELETE);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var r = new JavaScriptSerializer().Deserialize<ES_Acknowledged>(responseJson);
                    if (r != null && (r.acknowledged == true))
                    {
                        //LogEntry.WriteNLogEntry(
                        //    string.Format("ES_CallService.DeleteAllByTenantId() success\n\nBasePath: {0}\nClusterName: {1}\ntenantId: {2}", this.Config.BasePath, this.Config.ClusterName, tenantId),
                        //    NLog.LogLevel.Trace,
                        //    LogEvent.EventManagerElasticsearchGeneral
                        //);

                        result = true;
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_CallService.DeleteAllByTenantId() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    //throw;
                    return null;
                }
            }

            return result;
        }

        #endregion

    }
}

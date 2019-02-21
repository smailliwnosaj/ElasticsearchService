using ES.Models;
using ES.ViewModels;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Linq;
using System.Collections.Generic;
using System;
//using Logging;

namespace ES
{
    public class ES_MessageService : Base_Classes.ES_DataService , Interfaces.IElesticsearchService<ES_Message, ES.ViewModels.ES_Query>
    {
        public ES_MessageService() : base(Base_Classes.ES_ClusterName.messages)
        {
            initialize();
        }

        public ES_MessageService(ES_Config config) : base(config, Base_Classes.ES_ClusterName.messages)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_MessageService.Initialize()\n\nBasePath: {0}\nClusterName: {1}", this.Config.BasePath, this.Config.ClusterName),
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
        public async Task<ES_Message> GetByIdAsync(long id, int tenantId)
        {
            var message = await Task.Run(() => GetById(id, tenantId));
            return message;
        }

        public ES_Message GetById(long id, int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_MessageService.GetById()\n\nBasePath: {0}\nClusterName: {1}\nmessageId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            var message = new ES_Message();

            var path = string.Format(base.Config.GetDocumentByIdPath, tenantId.ToString(), id.ToString());

            var responseJson = base.String_WebRequest(path, null, Base_Classes.ES_Verb.GET);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var messageResponse = new JavaScriptSerializer().Deserialize<ES_MessageResponse>(responseJson);

                    //LogEntry.WriteNLogEntry(
                    //    string.Format("ES_MessageService.GetById() success\n\nBasePath: {0}\nClusterName: {1}\nmessageId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
                    //    NLog.LogLevel.Trace,
                    //    LogEvent.EventManagerElasticsearchGeneral
                    //);

                    if (messageResponse.found == true)
                    {
                        message = messageResponse._source;
                        message.TenantId = tenantId; // TenantId is not part of the elasticsearch document
                        message.MessageId = id; // MessageId is not part of the elasticsearch document
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_MessageService.GetById() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    throw;
                }
            }

            return message;
        }

        #endregion

        #region GetByIds

        /// <summary>
        /// This method returns a Task.  You must use the "await" keyword within an "async" method when calling this method.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ES_Message>> GetByIdsAsync(List<long> ids, int tenantId)
        {
            var message = await Task.Run(() => GetByIds(ids, tenantId));
            return message;
        }

        public List<ES_Message> GetByIds(List<long> ids, int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_MessageService.GetByIds()\n\nBasePath: {0}\nClusterName: {1}\nmessageIds: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, ids.ToString(), tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            var messages = new List<ES_Message>();

            var path = string.Format(base.Config.GetDocumentsByIdsPath, tenantId.ToString());
            var query = new ES_QueryByIds();
            query.ids = new List<string>();
            query.ids.AddRange(ids.Select(x => x.ToString()).ToList());

            var json = new JavaScriptSerializer().Serialize(query);

            var responseJson = base.String_WebRequest(path, json, Base_Classes.ES_Verb.POST);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var messageResponse = serializer.Deserialize<ES_MessagesResponse>(responseJson);
                    if (messageResponse == null) throw new Exception("ES_MessageService.GetByIds() failed to serialize response:" + "\n\n" + responseJson);

                    messageResponse.docs = messageResponse.docs.Where(x => x.found == true).ToList();

                    //LogEntry.WriteNLogEntry(
                    //    string.Format("ES_MessageService.GetByIds() success\n\nBasePath: {0}\nClusterName: {1}\nmessageIds: {2}\ntenantId: {3}\ncount: {4}", this.Config.BasePath, this.Config.ClusterName, ids.ToString(), tenantId, messageResponse.docs.Count()),
                    //    NLog.LogLevel.Trace,
                    //    LogEvent.EventManagerElasticsearchGeneral
                    //);

                    foreach (var doc in messageResponse.docs)
                    {
                        var em = doc._source;
                        em.TenantId = tenantId; // TenantId is not part of the elasticsearch document
                        em.MessageId = int.Parse(doc._id); // MessageId is not part of the elasticsearch document
                        messages.Add(em);
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_MessageService.GetByIds() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    throw;
                }
            }

            // TODO: Sort by order provided in messageIds

            return messages;
        }

        #endregion

        #region GetByQuery

        /// <summary>
        /// This method returns a Task.  You must use the "await" keyword within an "async" method when calling this method.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ES_Message>> GetByQueryAsync(ViewModels.ES_Query query, int tenantId)
        {
            var message = await Task.Run(() => GetByQuery(query, tenantId));
            return message;
        }

        public List<ES_Message> GetByQuery(ViewModels.ES_Query query, int tenantId)
        {
            var json = new JavaScriptSerializer().Serialize(query);
            var path = string.Format(base.Config.GetDocumentsByQueryPath, tenantId.ToString());

            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_MessageService.GetByQuery()\n\nBasePath: {0}\nClusterName: {1}\nquery: \n{2}\ntenantId: {3}\npath: {4}", this.Config.BasePath, this.Config.ClusterName, json, tenantId, path),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            var messages = new List<ES_Message>();
            var responseJson = base.String_WebRequest(path, json, Base_Classes.ES_Verb.POST);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
                    var messageResponse = serializer.Deserialize<ES_MessagesQueryResponse>(responseJson);
                    if (messageResponse == null) throw new Exception("ES_MessageService.GetByQuery() failed to serialize response:" + "\n\n" + responseJson);

                    //LogEntry.WriteNLogEntry(
                    //    string.Format("ES_MessageService.GetByQuery() success\n\nBasePath: {0}\nClusterName: {1}\nquery: \n{2}\ntenantId: {3}\npath: {4}\ncount: {5}", this.Config.BasePath, this.Config.ClusterName, json, tenantId, path, messageResponse.hits.hits.Count()),
                    //    NLog.LogLevel.Trace,
                    //    LogEvent.EventManagerElasticsearchGeneral
                    //);

                    foreach (var doc in messageResponse.hits.hits)
                    {
                        var em = doc._source;
                        em.TenantId = tenantId; // TenantId is not part of the elasticsearch document
                        em.MessageId = int.Parse(doc._id); // MessageId is not part of the elasticsearch document
                        messages.Add(em);
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_MessageService.GetByQuery() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
                    //    NLog.LogLevel.Error,
                    //    ex1,
                    //    LogEvent.EventManagerElasticsearchConnectionError
                    //);
                    throw;
                }
            }

            return messages;
        }

        #endregion

        #region Get document Count

        public ES_Count GetDocumentCount(int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_MessageService.GetDocumentCount()\n\nBasePath: {0}\nClusterName: {1}\ntenantId: {2}", this.Config.BasePath, this.Config.ClusterName, tenantId),
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
            //    string.Format("ES_MessageService.GetStatusInfo()\n\nBasePath: {0}\nClusterName: {1}", this.Config.BasePath, this.Config.ClusterName),
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
        public async Task<bool?> PutAsync(ES_Message doc, long id, int tenantId)
        {
            var success = await Task.Run(() => Put(doc, id, tenantId));
            return success;
        }

        private bool? Put(ES_Message doc, long id, int tenantId)
        {
            //LogEntry.WriteNLogEntry(
            //    string.Format("ES_MessageService.Put()\n\nBasePath: {0}\nClusterName: {1}\nmessageId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            bool? result = null;

            var path = string.Format(base.Config.PutDocumentPath, tenantId.ToString(), id.ToString());
            var data = new JavaScriptSerializer().Serialize(doc);

            var responseJson = base.String_WebRequest(path, data, Base_Classes.ES_Verb.PUT);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var r = new JavaScriptSerializer().Deserialize<ES_Result>(responseJson);
                    if (r != null && (r.result == "created" || r.result == "updated"))
                    {
                        //LogEntry.WriteNLogEntry(
                        //    string.Format("ES_MessageService.Put() success\n\nBasePath: {0}\nClusterName: {1}\nmessageId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
                        //    NLog.LogLevel.Trace,
                        //    LogEvent.EventManagerElasticsearchGeneral
                        //);

                        result = true;
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_MessageService.Put() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
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
            return base.SetupIndex<ES_MessagesMapping>(tenantId);
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
            //    string.Format("ES_MessageService.DeleteById()\n\nBasePath: {0}\nClusterName: {1}\nmessageId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
            //    NLog.LogLevel.Trace,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            bool? result = false;

            var path = string.Format(base.Config.DeleteDocumentByIdPath, tenantId.ToString(), id.ToString());

            var responseJson = base.String_WebRequest(path, null, Base_Classes.ES_Verb.DELETE);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var r = new JavaScriptSerializer().Deserialize<ES_Result>(responseJson);
                    if (r != null && (r.result == "deleted"))
                    {
                        //LogEntry.WriteNLogEntry(
                        //    string.Format("ES_MessageService.DeleteById() success\n\nBasePath: {0}\nClusterName: {1}\nmessageId: {2}\ntenantId: {3}", this.Config.BasePath, this.Config.ClusterName, id, tenantId),
                        //    NLog.LogLevel.Trace,
                        //    LogEvent.EventManagerElasticsearchGeneral
                        //);

                        result = true;
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_MessageService.DeleteById() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
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
            //    string.Format("ES_MessageService.DeleteAllByTenantId()\n\nBasePath: {0}\nClusterName: {1}\ntenantId: {2}", this.Config.BasePath, this.Config.ClusterName, tenantId),
            //    NLog.LogLevel.Info,
            //    LogEvent.EventManagerElasticsearchGeneral
            //);

            bool? result = false;

            var path = string.Format(base.Config.DeleteAllByTenantIdPath, tenantId.ToString());

            var responseJson = base.String_WebRequest(path, null, Base_Classes.ES_Verb.DELETE);

            if (!string.IsNullOrEmpty(responseJson))
            {
                try
                {
                    var r = new JavaScriptSerializer().Deserialize<ES_Acknowledged>(responseJson);
                    if (r != null && (r.acknowledged == true))
                    {
                        //LogEntry.WriteNLogEntry(
                        //    string.Format("ES_MessageService.DeleteAllByTenantId() success\n\nBasePath: {0}\nClusterName: {1}\ntenantId: {2}", this.Config.BasePath, this.Config.ClusterName, tenantId),
                        //    NLog.LogLevel.Trace,
                        //    LogEvent.EventManagerElasticsearchGeneral
                        //);

                        result = true;
                    }
                }
                catch (System.Exception ex1)
                {
                    //LogEntry.WriteNLogEntryEx(
                    //    "ES_MessageService.DeleteAllByTenantId() Json deserialization failed on: " + path + ".\n\nResponse looked like this:\n\n" + responseJson,
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

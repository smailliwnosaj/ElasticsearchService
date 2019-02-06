

namespace ElasticsearchService.Models
{
    public class ES_Config
    {
        public string BasePath { get; set; }
        public string PutDocumentPath { get; set; }
        public string PutMappingsPath { get; set; }
        public string GetDocumentByIdPath { get; set; }
        public string GetDocumentsByIdsPath { get; set; }
        public string GetDocumentsByQueryPath { get; set; }
        public string DeleteDocumentByIdPath { get; set; }
        public string DeleteAllByTenantIdPath { get; set; }
        public string GetMappingsPath { get; set; }
        public string ClusterName { get; set; }
        public string ClassName { get; set; }
    }
}

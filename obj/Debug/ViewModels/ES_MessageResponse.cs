
namespace ElasticsearchService.ViewModels
{
    public class ES_MessageResponse
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public int _version { get; set; }
        public bool found { get; set; }
        public ES_Message _source { get; set; }
    }
}

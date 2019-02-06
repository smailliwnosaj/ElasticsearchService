
namespace ElasticsearchService.ViewModels
{
    public class ES_CallResponse
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public int _version { get; set; }
        public bool found { get; set; }
        public ES_Call _source { get; set; }
    }
}

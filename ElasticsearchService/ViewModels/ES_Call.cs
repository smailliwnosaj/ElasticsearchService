
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace ElasticsearchService.ViewModels
{
    public class ES_Call
    {
        [ScriptIgnore] // Do not include in Elasticsearch document
        public int TenantId { get; set; }

        [ScriptIgnore] // Do not include in Elasticsearch document
        public long CallId { get; set; }

        public List<string> Transcriptions { get; set; }

        /// <summary>
        /// NOTE: Use the following date format string: "yyyy-MM-ddTHH:mm:ss.fffZ"
        /// </summary>
        public string UtcDateTime { get; set; }

        /// <summary>
        /// NOTE: Use the following date format string: "yyyy-MM-ddTHH:mm:ss.fffZ"
        /// </summary>
        public string UtcDateTime_End { get; set; }

        public short Day { get; set; }

        /// <summary>
        /// NOTE: Use the following date format string: "1970-01-01THH:mm:ss.fffZ"
        /// </summary>
        public string Time { get; set; }

        //public List<string> Categories { get; set; }
        //public string Categories { get; set; } // TODO: Must be able to support multiple Categories

        public int Duration { get; set; }

        public string Number { get; set; }

        public string FromNumber { get; set; }

        public string ToNumber { get; set; }

    }
}

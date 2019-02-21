

using System.Web.Script.Serialization;

namespace ES.ViewModels
{
    public class ES_Message
    {
        [ScriptIgnore] // Do not include in Elasticsearch document
        public int TenantId { get; set; }

        [ScriptIgnore] // Do not include in Elasticsearch document
        public long MessageId { get; set; }

        public string Text { get; set; }

        //public string DialogId { get; set; }

        /// <summary>
        /// NOTE: Use the following date format string: "yyyy-MM-ddTHH:mm:ss.fffZ"
        /// </summary>
        public string UtcDateTime { get; set; }

        public short Day { get; set; }

        /// <summary>
        /// NOTE: Use the following date format string: "1970-01-01THH:mm:ss.fffZ"
        /// </summary>
        public string Time { get; set; }

        //public List<string> Categories { get; set; }
        //public string Categories { get; set; } // TODO: Must be able to support multiple Categories

        public int Duration { get { return 0; } }

        public string Number { get; set; }

        public string FromNumber { get; set; }

        public string ToNumber { get; set; }
    }
}

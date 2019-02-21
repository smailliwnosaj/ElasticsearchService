
using System.Collections.Generic;

namespace ES.Models
{
    public class ES_Query
    {
        public List<string> IncludeTerms { get; set; }
        public List<string> ExcludeTerms { get; set; }
        public List<string> IncludePhrases { get; set; }
        public List<string> ExcludePhrases { get; set; }
    }
}

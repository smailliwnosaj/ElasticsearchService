
namespace ElasticsearchService.ViewModels
{
    public class ES_CallsMapping
    {
        public ES_CallsMapping()
        {
            mappings = new ES_Mappings();
            mappings._doc = new ES_Mappings.ES_Doc();
            mappings._doc.properties = new ES_Mappings.ES_Doc.ES_Properties()
            {
                Transcriptions = new ES_Mappings.ES_Doc.ES_Properties.ES_SearchableType() { type = "text" },
                UtcDateTime = new ES_Mappings.ES_Doc.ES_Properties.ES_DateType() { type = "date", format = "strict_date_time" },
                UtcDateTime_End = new ES_Mappings.ES_Doc.ES_Properties.ES_DateType() { type = "date", format = "strict_date_time" },
                Day = new ES_Mappings.ES_Doc.ES_Properties.ES_SearchableType() { type = "long" },
                Time = new ES_Mappings.ES_Doc.ES_Properties.ES_DateType() { type = "date", format = "strict_date_time" },
                Duration = new ES_Mappings.ES_Doc.ES_Properties.ES_SearchableType() { type = "long" },
                Number = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType()
                {
                    type = "keyword",
                    fields = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType.ES_Fields()
                    {
                        keyword = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType.ES_Fields.ES_Keyword()
                        {
                            type = "keyword"
                        }
                    }
                },
                FromNumber = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType()
                {
                    type = "keyword",
                    fields = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType.ES_Fields()
                    {
                        keyword = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType.ES_Fields.ES_Keyword()
                        {
                            type = "keyword"
                        }
                    }
                },
                ToNumber = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType()
                {
                    type = "keyword",
                    fields = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType.ES_Fields()
                    {
                        keyword = new ES_Mappings.ES_Doc.ES_Properties.ES_SortableType.ES_Fields.ES_Keyword()
                        {
                            type = "keyword"
                        }
                    }
                }
            };
        }

        public ES_Mappings mappings { get; set; }
        public class ES_Mappings
        {
            public ES_Doc _doc { get; set; }
            public class ES_Doc
            {
                public ES_Properties properties { get; set; }
                public class ES_Properties
                {
                    public ES_SearchableType Transcriptions { get; set; }

                    public ES_DateType UtcDateTime { get; set; }

                    public ES_DateType UtcDateTime_End { get; set; }

                    public ES_SearchableType Day { get; set; }

                    public ES_DateType Time { get; set; }

                    public ES_SearchableType Duration { get; set; }

                    public ES_SortableType Number { get; set; }

                    public ES_SortableType FromNumber { get; set; }

                    public ES_SortableType ToNumber { get; set; }

                    public class ES_DateType
                    {
                        public string type { get; set; }
                        public string format { get; set; }
                    }

                    public class ES_SearchableType
                    {
                        public string type { get; set; }
                    }

                    public class ES_SortableType
                    {
                        public string type { get; set; }
                        public ES_Fields fields { get; set; }
                        public class ES_Fields
                        {
                            public ES_Keyword keyword { get; set; }
                            public class ES_Keyword
                            {
                                public string type { get; set; }
                                //public int ignore_above { get; set; }
                            }
                        }
                    }

                }
            }
        }
    }
}

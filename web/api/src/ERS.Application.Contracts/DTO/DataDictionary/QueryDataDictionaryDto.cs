using System;

namespace ERS.DTO.DataDictionary
{
    public class QueryDataDictionaryDto
    {
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string NameZhtw { get; set; }
        public string NameZhcn { get; set; }
        public string NameVn { get; set; }
        public string NameEs { get; set; }
        public string NameCz { get; set; }
        public int SortOrder { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }

    }
}
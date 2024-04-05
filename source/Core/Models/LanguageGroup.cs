namespace GeNSIS.Core.Models
{
    using GeNSIS.Core.Interfaces;
    using LiteDB;
    using System.Collections.Generic;

    /// <summary>
    /// Contains a named group of languages as List, like "Orient" -> {Afghanistan, Iran, Iraq, ..., Turkey.}
    /// </summary>
    public class LanguageGroup : IProvideNameProperty
    {
        [BsonId]
        public string Name { get; set; }
        public List<Language> Languages { get; set; }
    }
}

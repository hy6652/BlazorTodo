using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace BlazorTodo.Shared
{
    public class SearchableBlock
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public string Id { get; set; } = string.Empty;

        [SimpleField(IsFilterable = true)]
        public string SheetName { get; set; } = string.Empty;

        [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.KoMicrosoft)]
        public string Name { get; set; } = string.Empty;

        [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.KoMicrosoft)]
        public string Type1 { get; set; } = string.Empty;

        [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.KoMicrosoft)]
        public string Type2 { get; set; } = string.Empty;

        [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.KoMicrosoft)]
        public string FirstEmenrgence { get; set; } = string.Empty;

        [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.KoMicrosoft)]
        public List<string> Location { get; set; } = new();

        [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.KoMicrosoft)]
        public string IsCaptured { get; set; } = string.Empty; 
    }
}

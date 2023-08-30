using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using BlazorTodo.Shared;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Drawing.Text;

namespace BlazorTodo.Server.Services.Search
{
    public class CognitiveSearchServiceOption
    {
        public string? EndPoint { get; set; }
        public string? CognitiveKey { get; set; }
    }

    public class CognitiveSearchService
    {
        public readonly string endPoint;
        public readonly string cognitiveKey;

        public readonly string semanticConfig;
        public readonly string indexName;

        private readonly SearchIndexClient _indexClient;
        private readonly SearchClient _searchClient;

        public CognitiveSearchService(IOptions<CognitiveSearchServiceOption> options)
        {
            endPoint = options.Value.EndPoint;
            cognitiveKey = options.Value.CognitiveKey;

            semanticConfig = "semantic-config";
            indexName = "hyunyoung-onboarding";  // lowercase only

            AzureKeyCredential credential = new AzureKeyCredential(cognitiveKey);
            _indexClient = new SearchIndexClient(new Uri(endPoint), credential);
            //_searchClient = new SearchClient(new Uri(endPoint), indexName, credential);
            _searchClient = _indexClient.GetSearchClient(indexName);
        }

        // created index with semantic search
        public async Task CreateIndex()
        {
            //var analyzerName = LexicalAnalyzerName.KoMicrosoft;

            SemanticSettings semanticSettings = new SemanticSettings();
            semanticSettings.Configurations.Add(new SemanticConfiguration
                (
                    semanticConfig,
                    new PrioritizedFields()
                    {
                        TitleField = new SemanticField { FieldName = "Name" },
                        ContentFields =
                        {
                            new SemanticField { FieldName = "Type1" },
                            new SemanticField { FieldName = "Type2" },
                            new SemanticField { FieldName = "FirstEmenrgence"  },
                        },
                        KeywordFields =
                        {
                            new SemanticField { FieldName = "Location"}
                        }
                    })
                );

            //SearchIndex index = new SearchIndex(indexName)
            //{
            //    Fields =
            //    {
            //        // IsKey = true field is necessary
            //        new SimpleField("Id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
            //        new SimpleField("SheetName", SearchFieldDataType.String) { IsFilterable = true },
            //        new SearchableField("Name") { IsFilterable = true, AnalyzerName = analyzerName },
            //        new SearchableField("Type1") { IsFilterable = true, AnalyzerName = analyzerName },
            //        new SearchableField("Type2") { IsFilterable = true, AnalyzerName = analyzerName },
            //        new SearchableField("FirstEmenrgence") { IsFilterable = true, AnalyzerName = analyzerName },
            //        new SearchableField("Location", true) { IsFilterable = true, AnalyzerName = analyzerName },
            //        new SearchableField("IsCaptured") { IsFilterable = true, AnalyzerName = analyzerName },
            //    },
            //    SemanticSettings = semanticSettings
            //};

            var fieldBuileder = new FieldBuilder().Build(typeof(SearchableBlock));
            SearchIndex index = new SearchIndex(indexName, fieldBuileder);
            index.SemanticSettings = semanticSettings;

            await _indexClient.CreateIndexAsync(index);
        }

        public async Task PushBlockToSearch(List<SearchableBlock> blocks)
        {
            try
            {
                if (blocks.Count > 0)
                {
                    await _searchClient.MergeOrUploadDocumentsAsync(blocks);
                }
                else
                {
                    Console.WriteLine("No documents to upload");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during uploading documents: " + ex.Message);
            }

        }

        public async Task<List<SearchResult>> SearchKnowledgebase(string question)
        {
            // for filtering, sorting, faceting, paging and other search query behaviors
            var options = new SearchOptions
            {
                QueryType = SearchQueryType.Semantic,
                QueryLanguage = QueryLanguage.KoKr,
                SemanticConfigurationName = semanticConfig,
                QueryCaption = QueryCaptionType.Extractive,
                QueryAnswer = QueryAnswerType.Extractive,
                QueryAnswerCount = 1
            };

            var response = await _searchClient.SearchAsync<SearchableBlock>(question, options);
            var result = response.Value.GetResults();

            List<SearchResult> searchResult = new List<SearchResult>();
            foreach (var item in result)
            {
                var list = new SearchResult();
                list.block = item.Document;
                list.RankerScore = item.RerankerScore ?? 0;
                list.Score = item.Score ?? 0;

                var caption = item.Captions?.FirstOrDefault();
                list.CaptionText = caption?.Text;
                list.CaptionHighlights = caption?.Highlights;

                searchResult.Add(list);
            }
            return searchResult;
        }
    }
}

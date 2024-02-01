using BlazorTodo.Base;
using CsvHelper.Configuration.Attributes;

namespace BlazorTodo.Shared
{
    public class CsvModel
    {
        [Name("id")]
        public int Id { get; set; }
        [Name("movie")]
        public string Movie { get; set; } = string.Empty;
        [Name("rating")]
        public double Rating { get; set; }
    }

    public class CsvDto
    {
        public List<CsvModel> Records { get; set; }
        public string FileName { get; set; }
    }

    public class CsvItem : CommonModel
    {
        public override string ClassType => nameof(CsvItem);
        public string BlobUrl { get; set; }
    }

    public class BlobTitleModel
    {
        public string Title { get; set; }
    }
}

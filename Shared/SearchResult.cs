using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTodo.Shared
{
    public class SearchResult
    {
        public SearchableBlock block { get; set; } = new SearchableBlock();
        public double RankerScore { get; set; }
        public double Score { get; set; }
        public string CaptionText { get; set; } = string.Empty;
        public string CaptionHighlights { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTodo.Shared
{
    public class SearchMessage
    {
        public string Question { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = false;
    }
}

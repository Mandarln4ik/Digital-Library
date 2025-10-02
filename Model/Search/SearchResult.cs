using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Model.Search
{
    internal class SearchResult
    {
        public List<DocumentView> Documents { get; set; } = new List<DocumentView>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}

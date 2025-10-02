using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Model.Search
{
    internal class DocumentFilter
    {
        public string SearchTerm { get; set; }
        public int? LanguageId { get; set; }
        public int? SubjectId { get; set; }
        public int? AuthorId { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

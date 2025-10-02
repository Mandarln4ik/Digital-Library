using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Model
{
    internal class AuthorWithDocuments
    {
        public Author Author { get; set; }
        public List<Document> Documents { get; set; } = new List<Document>();
        public int DocumentCount => Documents.Count;
    }
}

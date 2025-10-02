using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Model
{
    internal class DocumentWithAuthors
    {
        public Document Document { get; set; }
        public List<Author> Authors { get; set; } = new List<Author>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public Language Language { get; set; }
        public List<DocumentSubject> Subjects { get; set; } = new List<DocumentSubject>();
    }
}

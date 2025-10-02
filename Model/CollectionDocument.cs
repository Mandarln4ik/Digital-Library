using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Model
{
    internal class CollectionDocument
    {
        public int CollectionId { get; set; }
        public int DocumentId { get; set; }
        public DateTime DateAdded { get; set; }
    }
}

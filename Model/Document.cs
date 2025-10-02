using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Model
{
    public class Document
    {
        public int DocumentId { get; set; }
        public string? Title { get; set; }
        public string? AlternativeTitle { get; set; }
        public int? PublicationYear { get; set; }
        public string? ISBN { get; set; }
        public string? ISSN { get; set; }
        public string? UDC { get; set; }
        public string? BBK { get; set; }
        public string? Description { get; set; }
        public int? TotalPages { get; set; }
        public string? FilePath { get; set; }
        public string? ImagePath { get; set; }
        public DateTime DateUploaded { get; set; }
        public int? UploadedBy { get; set; }
        public int LanguageId { get; set; }
    }
}

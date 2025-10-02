using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Model
{
    internal class PersonalCollection
    {
        public int CollectionId { get; set; }
        public int UserId { get; set; }
        public string CollectionName { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

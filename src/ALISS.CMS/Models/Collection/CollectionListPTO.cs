using System;

namespace ALISS.CMS.Models.Collection
{
    public class CollectionListPTO
    {
        public Guid CollectionId { get; set; }
        public string Name { get; set; }
        public bool CanDelete { get; set; }
        public int ServiceCount { get; set; }
    }
}
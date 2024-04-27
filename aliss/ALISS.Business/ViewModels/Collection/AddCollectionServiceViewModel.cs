using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Collection
{
    public class AddCollectionServiceViewModel
    {
        public Guid? CollectionId { get; set; }
        public string CollectionName { get; set; }
        public Guid ServiceId { get; set; }
        public int UserProfileId { get; set; }
    }
}

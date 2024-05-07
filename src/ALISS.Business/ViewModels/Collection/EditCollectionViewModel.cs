using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Collection
{
    public class EditCollectionViewModel
    {
        public Guid? CollectionId { get; set; }
        public string Name { get; set; }
        public int UserProfileId { get; set; }
    }
}

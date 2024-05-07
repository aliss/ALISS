using ALISS.Business.PresentationTransferObjects.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Collection
{
    public class EmailCollectionViewModel
    {
        public string Email { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public IEnumerable<EmailCollectionServicePTO> Services { get; set; }
        public string Logo { get; set; }
    }
}

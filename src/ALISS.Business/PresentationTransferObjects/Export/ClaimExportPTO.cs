using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.PresentationTransferObjects.Export
{
    class ClaimExportPTO
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Organisation { get; set; }
    }
}

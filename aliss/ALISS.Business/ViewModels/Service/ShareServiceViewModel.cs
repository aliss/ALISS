using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ALISS.Business.ViewModels.Service
{
    public class ShareServiceViewModel
    {
        public Guid? ServiceId { get; set; }
        public string ServiceSlug { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string ServiceName { get; set; }
        public string OrganisationName { get; set; }
        public string Link { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }
}

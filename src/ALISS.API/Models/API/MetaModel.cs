using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.API
{
    public class MetaModel
    {
        public string licence { get; set; }
        public IEnumerable<Attribution> attribution { get; set; }
    }

    public class Attribution
    {
        public string text { get; set; }
        public string url { get; set; }
    }
}
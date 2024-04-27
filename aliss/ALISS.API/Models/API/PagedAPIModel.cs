using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.API
{
    public class PagedAPIModel<T> : APIModel<T> where T : class
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
    }
}
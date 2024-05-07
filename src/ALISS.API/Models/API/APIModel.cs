using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.API
{
    public class APIModel<T>
    {
        public MetaModel meta { get; set; }
        public T data { get; set; }
        public string error { get; set; }
    }
}
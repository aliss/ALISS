using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.API.Models.External
{
    public class PostcodeResult
    {
        public int status { get; set; }
        public string error { get; set; }
        public Postcode result { get; set; }
    }

    public class Postcode
    {
        public string postcode { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string nhs_ha { get; set; }
        public AdminCodes codes { get; set; }
    }

    public class AdminCodes
    {
        public string admin_district { get; set; }
        public string admin_ward { get; set; }
    }

    public class ValidateResult
    {
        public int status { get; set; }
        public bool result { get; set; }
    }
}
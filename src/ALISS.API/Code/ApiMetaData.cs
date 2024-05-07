using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ALISS.API.Models.API;

namespace ALISS.API.Code
{
    public static class ApiMetaData
    {
        public static MetaModel GetCommonMetaData(bool nationalStatistics = true, bool scottishCharity = true, bool nationalRecords = true, bool alissUsers = true)
        {
            List<Attribution> attributions = new List<Attribution>();

            if (nationalStatistics)
            {
                attributions.Add(new Attribution()
                {
                    text = "Contains National Statistics data © Crown copyright and database right 2018",
                    url = "http://geoportal.statistics.gov.uk/datasets/local-authority-districts-december-2016-generalised-clipped-boundaries-in-the-uk/"
                });
            }
            if (scottishCharity)
            {
                attributions.Add(new Attribution()
                {
                    text = "Contains information from the Scottish Charity Register supplied by the Office of the Scottish Charity Regulator and licensed under the Open Government Licence v2.0",
                    url = "https://www.oscr.org.uk/about-charities/search-the-register/charity-register-download"
                });
            }
            if (nationalRecords)
            {
                attributions.Add(new Attribution()
                {
                    text = "Contains National Records of Scotland data licensed under the Open Government Licence v3.0",
                    url = "https://www.nrscotland.gov.uk/statistics-and-data/geography/nrs-postcode-extract"
                });
            }
            if (alissUsers)
            {
                attributions.Add(new Attribution()
                {
                    text = "Contains contributions from ALISS users",
                    url = "https://www.aliss.org/terms-and-conditions"
                });
            }

            return new MetaModel()
            {
                licence = "https://creativecommons.org/licenses/by/4.0/",
                attribution = attributions
            };
        }
    }
}
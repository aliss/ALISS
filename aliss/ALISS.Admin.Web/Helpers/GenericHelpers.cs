using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Helpers
{
    public static class GenericHelpers
    {
        public static string GetGuidanceContent(string pageName)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"{ConfigurationManager.AppSettings["BasePublicSiteUrl"].TrimEnd('/')}");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("/umbraco/api/guidance/GetGuidanceContent?pageName=" + pageName).Result;
            
            if (response.IsSuccessStatusCode)
            {
                var bodyText = response.Content.ReadAsAsync<dynamic>().Result;
                
                return bodyText;
            }

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace ALISS.CMS.Controllers
{
    public class AlissNestedCategoryPickerController : UmbracoApiController
    {
        private readonly JsonSerializerSettings serialiser;
        private readonly string AlissApiBaseUrl = ConfigurationManager.AppSettings["Settings:ApiBaseUrl"];

        public AlissNestedCategoryPickerController()
        {
            serialiser = new JsonSerializerSettings()
            {
                Culture = CultureInfo.CurrentUICulture,
                DefaultValueHandling = DefaultValueHandling.Include,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        [Route("getcategories")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCategories()
        {
            var client = new HttpClient();
            var ub = new UriBuilder($"{AlissApiBaseUrl}categories");
            var response = await client.GetAsync(ub.Uri);
            response.EnsureSuccessStatusCode();
            string jsonResult = await response.Content.ReadAsStringAsync();

            dynamic dynamicResult = JsonConvert.DeserializeObject(jsonResult);

            return Json(dynamicResult, serialiser);
        }
    }
}
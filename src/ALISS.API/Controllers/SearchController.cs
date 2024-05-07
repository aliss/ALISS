using ALISS.API.Code;
using ALISS.API.Code.Elasticsearch;
using ALISS.API.Code.Mapping;
using ALISS.API.Models.API;
using ALISS.API.Models.Elasticsearch;
using ALISS.API.Models.External;
using Nest;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;

namespace ALISS.API.Controllers
{
    public partial class SearchController : ApiController
    {
        private ServiceSearch _serviceSearch;
        private ServiceMapping _serviceMapping;

        public SearchController()
        {
            _serviceSearch = new ServiceSearch();
            _serviceMapping = new ServiceMapping();
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(PagedAPIModel<IEnumerable<ServiceModel>>))]
        [ResponseType(typeof(PagedAPIModel<IEnumerable<ServiceModel>>))]
        [Route("v4/services")]
        public async Task<HttpResponseMessage> SearchServices(string postcode = null, string q = null, string category = null, string location_type = null, int radius = 5000, int page = 1, int page_size = 10, string sort = null)
        {
            PostcodeResult postcodeResult = new PostcodeResult();
            ValidateResult validateResult = new ValidateResult();
            var validatePostcodeRequest = (HttpWebRequest)WebRequest.Create("http://api.postcodes.io/postcodes/" + postcode + "/validate");
            var request = (HttpWebRequest)WebRequest.Create("http://api.postcodes.io/postcodes/" + postcode);
            try
            {
                if (page < 1)
                {
                    page = 1;
                }
                if (!String.IsNullOrEmpty(postcode))
                {
                    switch (postcode.Replace(" ", "").ToLower())
                    {
                    case "g334rz":
                        postcodeResult = new PostcodeResult
                        {
                            error = null,
                            status = 200,
                            result = new Postcode
                            {
                                postcode = postcode,
                                latitude = 55.857569,
                                longitude = -4.133327,
                                nhs_ha = "Greater Glasgow and Clyde",
                                codes = new AdminCodes { admin_district = "S12000049", admin_ward = "S13002984" }
                            }
                        };
                        break;
                    default:
                        using (HttpWebResponse validateResponse = (HttpWebResponse)validatePostcodeRequest.GetResponse())
                        {
                            using (StreamReader reader = new StreamReader(validateResponse.GetResponseStream()))
                            {
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                string objText = reader.ReadToEnd();
                                validateResult = (ValidateResult)js.Deserialize(objText, typeof(ValidateResult));

                            }
                            if (validateResult.result)
                            {
                                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                                {
                                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                                    {
                                        JavaScriptSerializer js = new JavaScriptSerializer();
                                        string objText = reader.ReadToEnd();
                                        postcodeResult = (PostcodeResult)js.Deserialize(objText, typeof(PostcodeResult));
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
                else
                {
                    validateResult.result = true;
                }
                List<ServiceModel> data = new List<ServiceModel>();
                ISearchResponse<ServiceElasticSearchModel> searchResponse = null;
                if (validateResult.result)
                {
                    searchResponse = await _serviceSearch.SearchServicesAsync(postcodeResult.result, q, category, location_type, radius, page, page_size, sort);

                    foreach (ServiceElasticSearchModel service in searchResponse.Documents)
                    {
                        data.Add(_serviceMapping.MapServiceToOutput(service));
                    }
                }

                PagedAPIModel<IEnumerable<ServiceModel>> model = new PagedAPIModel<IEnumerable<ServiceModel>>()
                {
                    meta = ApiMetaData.GetCommonMetaData(),
                    data = data.Count == 0 ? Enumerable.Empty<ServiceModel>() : data,
                };

                if (!validateResult.result)
                {
                    model.error = "Invalid Postcode";
                }
                else
                {
                    model.previous = page == 1 ? null : "/api/v4/services?postcode=" + BuildQuery(postcode, q, category, location_type, radius, page - 1, page_size);
                    model.next = (Math.Ceiling((double)searchResponse.Total / page_size) == page) ? null : "/api/v4/services?postcode=" + BuildQuery(postcode, q, category, location_type, radius, page + 1, page_size);
                    model.count = Convert.ToInt32(searchResponse?.Total ?? 0);
                }

                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception e)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
        }

        private string BuildQuery(string postcode, string q = null, string category = null, string location_type = null, int radius = 5000, int page = 1, int page_size = 10)
        {
            StringBuilder SB = new StringBuilder();

            SB.Append(postcode);

            if (String.IsNullOrEmpty(q) == false)
            {
                SB.Append("&q=" + q);
            }

            SB.Append("&page=" + page);

            if (page_size != 10)
            {
                SB.Append("&page_size=" + page_size);
            }

            if (location_type != null)
            {
                SB.Append("&location_type=" + location_type);
            }

            if (!String.IsNullOrEmpty(category))
            {
                SB.Append("&category=" + category);
            }

            if (radius != 5000)
            {
                SB.Append("&radius=" + radius);
            }

            return SB.ToString();
        }
    }
}

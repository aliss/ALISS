using ALISS.API.Code;
using ALISS.API.Code.Elasticsearch;
using ALISS.API.Code.Mapping;
using ALISS.API.Models.API;
using ALISS.API.Models.Elasticsearch;
using Nest;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;

namespace ALISS.API.Controllers
{
    public partial class ListController : ApiController
    {
        private ServiceSearch _serviceSearch;
        private OrganisationSearch _organisationSearch;
        private CategorySearch _categorySearch;
        private ServiceAreaSearch _serviceAreaSearch;
        private OrganisationMapping _organisationMapping;
        private ServiceMapping _serviceMapping;
        private CommunityGroupSearch _communityGroupSearch;
        private AccessibilityFeatureSearch _accessibilityFeatureSearch;

        public ListController()
        {
            _organisationSearch = new OrganisationSearch();
            _serviceSearch = new ServiceSearch();
            _categorySearch = new CategorySearch();
            _serviceAreaSearch = new ServiceAreaSearch();
            _organisationMapping = new OrganisationMapping();
            _serviceMapping = new ServiceMapping();
            _communityGroupSearch = new CommunityGroupSearch();
            _accessibilityFeatureSearch = new AccessibilityFeatureSearch();
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(PagedAPIModel<IEnumerable<ServiceModel>>))]
        [ResponseType(typeof(APIModel<IEnumerable<ServiceModel>>))]
        [Route("v4/import")]
        public async Task<HttpResponseMessage> GetAllServices(int page = 1)
        {
            ISearchResponse<ServiceElasticSearchModel> response = await _serviceSearch.GetAllServicesAsync(page);
            List<ServiceModel> data = new List<ServiceModel>();
            foreach (ServiceElasticSearchModel service in response.Documents)
            {
                data.Add(_serviceMapping.MapServiceToOutput(service));
            }

            PagedAPIModel<IEnumerable<ServiceModel>> model = new PagedAPIModel<IEnumerable<ServiceModel>>()
            {
                meta = ApiMetaData.GetCommonMetaData(),
                data = data.Count == 0 ? Enumerable.Empty<ServiceModel>() : data,
                count = Convert.ToInt32(response.Total),
                previous = page == 1 ? null : "/api/v4/import?page=" + (page - 1),
                next = (response.Total / 20 == page) ? null : "/api/v4/import?page=" + (page + 1)
            };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(PagedAPIModel<OrganisationModel>))]
        [ResponseType(typeof(PagedAPIModel<OrganisationModel>))]
        [Route("v4/organisations")]
        public async Task<HttpResponseMessage> GetOrganisations(string searchTerm = "", int page = 1)
        {
            ISearchResponse<OrganisationElasticSearchModel> response = _organisationSearch.GetFilteredOrganisations(searchTerm, true, true, page);
            List<OrganisationModel> data = new List<OrganisationModel>();
            foreach (OrganisationElasticSearchModel organisation in response.Documents)
            {
                data.Add(_organisationMapping.MapOrganisationToOutput(organisation));
            }

            PagedAPIModel<IEnumerable<OrganisationModel>> model = new PagedAPIModel<IEnumerable<OrganisationModel>>()
            {
                meta = ApiMetaData.GetCommonMetaData(),
                data = data.Count == 0 ? Enumerable.Empty<OrganisationModel>() : data,
                count = Convert.ToInt32(response.Total),
                previous = page == 1 ? null : "/api/v4/organisations?searchTerm=" + searchTerm + "page=" + (page - 1),
                next = (response.Total / 20 < page) ? null : "/api/v4/organisations?searchTerm=" + searchTerm + "page=" + (page + 1)
            };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(APIModel<IEnumerable<CategoryElasticSearchModel>>))]
        [ResponseType(typeof(APIModel<IEnumerable<CategoryElasticSearchModel>>))]
        [Route("v4/categories")]
        public async Task<HttpResponseMessage> GetCategories()
        {
            IEnumerable<CategoryElasticSearchModel> categories = await _categorySearch.GetCategories();

            APIModel<IEnumerable<CategoryElasticSearchModel>> model = new APIModel<IEnumerable<CategoryElasticSearchModel>>()
            {
                meta = ApiMetaData.GetCommonMetaData(false, false, false, false),
                data = categories
            };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(APIModel<IEnumerable<ServiceAreaElasticSearchModel>>))]
        [ResponseType(typeof(APIModel<IEnumerable<ServiceAreaElasticSearchModel>>))]
        [Route("v4/service-areas")]
        public async Task<HttpResponseMessage> GetServiceAreas()
        {
            IEnumerable<ServiceAreaElasticSearchModel> serviceAreas = await _serviceAreaSearch.GetServiceAreas();

            APIModel<IEnumerable<ServiceAreaElasticSearchModel>> model = new APIModel<IEnumerable<ServiceAreaElasticSearchModel>>()
            {
                meta = ApiMetaData.GetCommonMetaData(true, true, false, false),
                data = serviceAreas
            };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(object))]
        [ResponseType(typeof(object))]
        [Route("v4/service-area-spatial")]
        public async Task<HttpResponseMessage> GetServiceAreaData(string service_id)
        {
            IList<string> serviceAreaCodes = new List<string>();

            if (Guid.TryParse(service_id, out Guid guidId))
            {
                ServiceElasticSearchModel service = await _serviceSearch.GetByIdAsync(guidId);
                serviceAreaCodes = service != null && service.service_areas.Any() ? service.service_areas.Select(x => x.code).ToList() : new List<string>();
            }

            if (serviceAreaCodes.Any())
            {
                IEnumerable<ServiceAreaElasticSearchModel> serviceAreas = await _serviceAreaSearch.GetServiceAreas();

                IEnumerable<string> geoJsonList = serviceAreas.Where(x => serviceAreaCodes.Contains(x.code)).Select(x => x.geojson);
                string json = $"[{string.Join(",", geoJsonList)}]";

                JavaScriptSerializer js = new JavaScriptSerializer();
                var jObject = js.Deserialize(json, typeof(object));

                return Request.CreateResponse(HttpStatusCode.OK, jObject);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(APIModel<IEnumerable<CommunityGroupElasticSearchModel>>))]
        [ResponseType(typeof(APIModel<IEnumerable<CommunityGroupElasticSearchModel>>))]
        [Route("v4/community-groups")]
        public async Task<HttpResponseMessage> GetCommunityGroups()
        {
            IEnumerable<CommunityGroupElasticSearchModel> communityGroups = await _communityGroupSearch.GetCommunityGroups();
            MetaModel metaModel = new MetaModel()
            {
                licence = "https://www.creativecommons.org/licenses/by/4.0/",
                attribution = new List<Attribution>()
            };

            APIModel<IEnumerable<CommunityGroupElasticSearchModel>> model = new APIModel<IEnumerable<CommunityGroupElasticSearchModel>>()
            {
                meta = metaModel,
                data = communityGroups
            };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(APIModel<IEnumerable<CategoryElasticSearchModel>>))]
        [ResponseType(typeof(APIModel<IEnumerable<CategoryElasticSearchModel>>))]
        [Route("v4/categories-flat")]
        public async Task<HttpResponseMessage> GetCategoriesFlat()
        {
            IEnumerable<CategoryElasticSearchModel> categories = await _categorySearch.GetCategories(true);
            MetaModel metaModel = new MetaModel()
            {
                licence = "https://www.creativecommons.org/licenses/by/4.0/",
                attribution = new List<Attribution>()
            };

            APIModel<IEnumerable<CategoryElasticSearchModel>> model = new APIModel<IEnumerable<CategoryElasticSearchModel>>()
            {
                meta = metaModel,
                data = categories
            };

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(object))]
        [ResponseType(typeof(object))]
        [Route("v4/selected-service-area-spatial")]
        public async Task<HttpResponseMessage> GetSelectedServiceAreaData(string service_areas)
        {
            IList<string> serviceAreaIds = !string.IsNullOrWhiteSpace(service_areas) != null
                ? service_areas.Split(',').ToList()
                : new List<string>();

            if (serviceAreaIds.Any())
            {
                List<int> serviceAreaIdList = service_areas.Split(',').Select(int.Parse).ToList();
                IEnumerable<ServiceAreaElasticSearchModel> serviceAreas = await _serviceAreaSearch.GetServiceAreas(serviceAreaIdList);

                IEnumerable<string> geoJsonList = serviceAreas.Select(x => x.geojson);
                string json = $"[{string.Join(",", geoJsonList)}]";

                JavaScriptSerializer js = new JavaScriptSerializer();
                var jObject = js.Deserialize(json, typeof(object));

                return Request.CreateResponse(HttpStatusCode.OK, jObject);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
        }
    }
}

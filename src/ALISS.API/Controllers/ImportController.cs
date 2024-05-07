using ALISS.API.Code.Elasticsearch;
using ALISS.API.Models.Elasticsearch;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ALISS.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ImportController : ApiController
    {
        private OrganisationSearch _organisationSearch;
        private ServiceSearch _serviceSearch;
        private CategorySearch _categorySearch;
        private ServiceAreaSearch _serviceAreaSearch;

        public ImportController()
        {
            _organisationSearch = new OrganisationSearch();
            _serviceAreaSearch = new ServiceAreaSearch();
            _categorySearch = new CategorySearch();
            _serviceSearch = new ServiceSearch();
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(string))]
        [ResponseType(typeof(string))]
        [Route("import/service")]
        public async Task<HttpResponseMessage> ImportSingleService(ServiceElasticSearchModel service)
        {
            try
            {
                await _serviceSearch.AddServiceAsync(service);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, "Import Succeeded");
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(string))]
        [ResponseType(typeof(string))]
        [Route("import/services")]
        public async Task<HttpResponseMessage> ImportServices(List<ServiceElasticSearchModel> services)
        {
            try
            {
                await _serviceSearch.AddServicesAsync(services);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, "Import Succeeded");
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(string))]
        [ResponseType(typeof(string))]
        [Route("import/organisation")]
        public async Task<HttpResponseMessage> ImportSingleOrganisation(OrganisationElasticSearchModel organisation)
        {
            try
            {
                await _organisationSearch.AddOrganisationAsync(organisation);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, "Import Succeeded");
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(string))]
        [ResponseType(typeof(string))]
        [Route("import/organisations")]
        public async Task<HttpResponseMessage> ImportOrganisations(List<OrganisationElasticSearchModel> organisations)
        {
            try
            {
                await _organisationSearch.AddOrganisationsAsync(organisations);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, "Import Succeeded");
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(string))]
        [ResponseType(typeof(string))]
        [Route("import/categories")]
        public async Task<HttpResponseMessage> ImportCategories(List<CategoryElasticSearchModel> categories)
        {
            try
            {
                await _categorySearch.AddCategories(categories);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, "Import Succeeded");
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(string))]
        [ResponseType(typeof(string))]
        [Route("import/serviceareas")]
        public async Task<HttpResponseMessage> ImportServiceAreas(List<ServiceAreaElasticSearchModel> serviceAreas)
        {
            try
            {
                await _serviceAreaSearch.AddServiceAreas(serviceAreas);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, "Import Succeeded");
        }

    }
}

using ALISS.API.Code;
using ALISS.API.Code.Elasticsearch;
using ALISS.API.Code.Mapping;
using ALISS.API.Models.API;
using ALISS.API.Models.Elasticsearch;
using AutoMapper;
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
    public partial class ViewController : ApiController
    {
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(APIModel<ServiceModel>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "No service found")]
        [ResponseType(typeof(APIModel<ServiceModel>))]
        [Route("v5/services/{id}")]
        public async Task<HttpResponseMessage> GetServiceByIdV5(string id)
        {
            try
            {
                ServiceElasticSearchModel service;
                ServiceModel finalService;
                if (Guid.TryParse(id, out Guid guidId))
                {
                    service = await _serviceSearch.GetByIdAsync(guidId);
                    finalService = service == null ? null : _serviceMapping.MapServiceToOutput(service);
                }
                else
                {
                    service = await _serviceSearch.GetBySlugAsync(id);
                    finalService = service == null ? null : _serviceMapping.MapServiceToOutput(service);
                }

                if (finalService == null)
                {
                    return Request.CreateResponse<APIModel<ServiceModel>>(HttpStatusCode.NotFound, null);
                }
                else
                {
                    APIModel<ServiceModel> model = new APIModel<ServiceModel>()
                    {
                        meta = ApiMetaData.GetCommonMetaData(),
                        data = finalService
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "OK", typeof(APIModel<OrganisationModel>))]
        [SwaggerResponse(HttpStatusCode.NotFound, "No organisation found")]
        [ResponseType(typeof(APIModel<OrganisationModel>))]
        [Route("v5/organisations/{id}")]
        public async Task<HttpResponseMessage> GetOrganisationByIdV5(string id)
        {
            try
            {
                OrganisationElasticSearchModel organisation;
                OrganisationModel finalOrganisation;
                if (Guid.TryParse(id, out Guid guidId))
                {
                    organisation = await _organisationSearch.GetByIdAsync(guidId);
                    finalOrganisation = organisation == null ? null : _organisationMapping.MapOrganisationToOutput(organisation);
                }
                else
                {
                    organisation = await _organisationSearch.GetBySlugAsync(id);
                    finalOrganisation = organisation == null ? null : _organisationMapping.MapOrganisationToOutput(organisation);
                }

                if (finalOrganisation == null)
                {
                    return Request.CreateResponse<APIModel<OrganisationModel>>(HttpStatusCode.NotFound, null);
                }
                else
                {
                    APIModel<OrganisationModel> model = new APIModel<OrganisationModel>()
                    {
                        meta = ApiMetaData.GetCommonMetaData(),
                        data = finalOrganisation
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

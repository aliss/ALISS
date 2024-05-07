using ALISS.API.Code.Elasticsearch;
using ALISS.API.Models.Elasticsearch;
using ALISS.Business;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.User;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ALISS.Admin.Web.ControllerAPI
{
    [RoutePrefix("api/claim")]
    public class ClaimController : ApiController
    {
        private ElasticSearchService _elasticSearchService;
        private UserProfileService _userProfileService;
        private Business.Services.EmailService _emailService;
        private string BaseUrl = ConfigurationManager.AppSettings["BaseSiteUrl"].ToString();
        private string BasePublicUrl = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString();

        public ClaimController()
        {
            _elasticSearchService = new ElasticSearchService();
            _emailService = new Business.Services.EmailService();
        }

        [HttpPost]
        [Route("PostClaim")]
        public string PostClaim(AddClaimViewModel model)
        {
            if (!model.Id.HasValue)
            {
                model.Id = _elasticSearchService.GetOrganisationIdFromSlug(model.Slug);
            }
            ClaimService.AddClaim(model);
            string url = BaseUrl + "Claim";

            OrganisationElasticSearchModel organisation = _elasticSearchService.GetOrganisationById(model.Id.Value);

            using (ALISSContext dc = new ALISSContext())
            {
                if (organisation.is_claimed)
                {
                    OrganisationClaimUser leadClaim = dc.OrganisationClaimUsers.Where(x => x.OrganisationId == model.Id && x.IsLeadClaimant).FirstOrDefault()
                        ?? dc.OrganisationClaimUsers.Include(x => x.Organisation).Where(x => x.OrganisationId == model.Id && x.ClaimedUserId == x.Organisation.ClaimedUserId).FirstOrDefault();

                    if (leadClaim != null)
                    {
                        _emailService.SendClaimSubmittedEmail(leadClaim.ClaimedUser.Email, leadClaim.ClaimedUser.Name, leadClaim.ClaimedUser.Username, organisation.name, $"{BasePublicUrl}/organisations/{leadClaim.Organisation.OrganisationId}", "organisation", url, !organisation.is_claimed || model.RequestLeadClaimant);
                    }
                }
                else
                {
                    _emailService.SendGenericSystemEmail(ConfigurationManager.AppSettings["ALISSAdminContact"], "New Claim Submitted", "A new claim has been submitted for approval. Please log into the system and check this new claim.", url);
                }

                UserProfile userProfile = dc.UserProfiles.Find(model.ClaimedUserId);
                _emailService.SendClaimPendingEmail(userProfile.Email, userProfile.Name, userProfile.Username, organisation.name, !organisation.is_claimed || model.RequestLeadClaimant);
            }

            return "Claim successfully submitted. A member of the ALISS team will review this soon.";
        }

        [HttpPost]
        [Route("PostServiceClaim")]
        public string PostServiceClaim(AddClaimViewModel model)
        {
            if (!model.Id.HasValue)
            {
                model.Id = _elasticSearchService.GetServiceIdFromSlug(model.Slug);
            }
            ServiceClaimService.AddClaim(model);
            string organisationUrl = BaseUrl + "Claim";
            string serviceUrl = BaseUrl + "ServiceClaim";

            ServiceElasticSearchModel service = _elasticSearchService.GetServiceById(model.Id.Value);

            using (ALISSContext dc = new ALISSContext())
            {
                if (service.is_claimed)
                {
                    ServiceClaimUser leadClaim = dc.ServiceClaimUsers.Where(x => x.ServiceId == model.Id && x.IsLeadClaimant).FirstOrDefault()
                        ?? dc.ServiceClaimUsers.Include(x => x.Service).Where(x => x.ServiceId == model.Id && x.ClaimedUserId == x.Service.ClaimedUserId).FirstOrDefault();

                    if (leadClaim != null)
                    {
                        _emailService.SendClaimSubmittedEmail(leadClaim.ClaimedUser.Email, leadClaim.ClaimedUser.Name, leadClaim.ClaimedUser.Username, leadClaim.Service.Name, $"{BasePublicUrl}/services/{leadClaim.Service.ServiceId}", "service", serviceUrl, !service.is_claimed || model.RequestLeadClaimant);
                    }
                }
                else
                {
                    _emailService.SendGenericSystemEmail(ConfigurationManager.AppSettings["ALISSAdminContact"], "New Claim Submitted", "A new claim has been submitted for approval. Please log into the system and check this new claim.", serviceUrl);
                }

				if (service.organisation.is_claimed)
                {
                	OrganisationClaimUser leadOrgClaim = dc.OrganisationClaimUsers.Where(x => x.OrganisationId == service.organisation.id && x.IsLeadClaimant == true).FirstOrDefault();
                    _emailService.SendClaimSubmittedEmail(leadOrgClaim.ClaimedUser.Email, leadOrgClaim.ClaimedUser.Name, leadOrgClaim.ClaimedUser.Username, service.name, $"{BasePublicUrl}/services/{service.id}", "organisation's service", serviceUrl, !service.is_claimed || model.RequestLeadClaimant);
                }

                UserProfile userProfile = dc.UserProfiles.Find(model.ClaimedUserId);
                _emailService.SendClaimPendingEmail(userProfile.Email, userProfile.Name, userProfile.Username, service.name, !service.is_claimed || model.RequestLeadClaimant);
            }

            return "Claim successfully submitted. A member of the ALISS team will review this soon.";
        }
    }
}

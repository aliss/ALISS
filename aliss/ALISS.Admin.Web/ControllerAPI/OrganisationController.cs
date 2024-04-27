using ALISS.Business;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.User;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ALISS.Admin.Web.ControllerAPI
{
    [RoutePrefix("api/organisation")]
    public class OrganisationController : ApiController
    {
        private OrganisationService _organisationService;
        private UserProfileService _userProfileService;

        public OrganisationController()
        {
            _organisationService = new OrganisationService();
            _userProfileService = new UserProfileService();
        }

        // GET: Organisation
        [HttpGet]
        [Route("GetOrganisationLogo")]
        public string GetOrganisationLogo(Guid organisationId)
        {
            string logoUrl = _organisationService.GetOrganisationForEdit(organisationId).Logo;
            return logoUrl;
        }

        [HttpGet]
        [Route("GetCanUserEditOrganisation")]
        public bool GetCanUserEditOrganisation(int userProfileId, Guid organisationId)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            EditUserViewModel userProfile = _userProfileService.GetUserForEdit(userProfileId, userManager);
            bool canUserEdit = _organisationService.CanUserEditOrganisation(userManager, userProfile.Username, organisationId);
            return canUserEdit;
        }

    }
}
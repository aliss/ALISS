using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Business.Enums;
using ALISS.Business.Services;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize]
    public class DataInputBaseController : ALISSBaseController
    {
        public readonly OrganisationService _organisationService;
        public readonly ServiceService _serviceService;
        public readonly DataInputService _dataInputService;
        public readonly CategoryService _categoryService;
        public readonly CommunityGroupService _communityGroupService;
        public readonly Business.Services.EmailService _emailService;
        public BlobStorageService _blobStorageService;
        public readonly UserProfileService _userProfileService;
        public DataInputBaseController()
        {
            _organisationService = new OrganisationService();
            _serviceService = new ServiceService();
            _dataInputService = new DataInputService();
            _categoryService = new CategoryService();
            _communityGroupService = new CommunityGroupService();
            _emailService = new Business.Services.EmailService();
            _blobStorageService = new BlobStorageService();
            _userProfileService = new UserProfileService();

            ViewBag.TotalSteps = DataInputStepsEnum.TotalSteps;
        }
    }
}
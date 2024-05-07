using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Service;
using ALISS.Models.Models;

namespace ALISS.Business.Services
{
    public partial class DataInputService
    {
		private LocationService _locationService = new LocationService();
		private ServiceAreaService _serviceAreaService = new ServiceAreaService();
        private CategoryService _categoryService = new CategoryService();
		private CommunityGroupService _communityGroupService = new CommunityGroupService();
		private AccessibilityFeatureService _accessibilityFeatureService = new AccessibilityFeatureService();
		private ServiceService _serviceService = new ServiceService();
		private OrganisationService _organisationService = new OrganisationService();
		private ElasticSearchService _elasticSearchService = new ElasticSearchService();
		private BlobStorageService _blobStorageService = new BlobStorageService();

		private Business.Services.EmailService _emailService = new Business.Services.EmailService();

		public DataInputService()
		{

		}
    }
}

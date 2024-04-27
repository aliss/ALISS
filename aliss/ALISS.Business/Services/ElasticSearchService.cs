using ALISS.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ALISS.Business.Enums;
using Tactuum.Core.Extensions;
using System.Configuration;
using ALISS.API.Models.Elasticsearch;
using ALISS.API.Code.Elasticsearch;
using Nest;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Web.Script.Serialization;

namespace ALISS.Business.Services
{
    public class ElasticSearchService
    {
        private ServiceSearch _serviceSearch;
        private OrganisationSearch _organisationSearch;
        private AccessibilityFeatureSearch _accessibilityFeatureSearch;
        private CategorySearch _categorySearch;
        private CommunityGroupSearch _communityGroupSearch;
		private ServiceAreaSearch _serviceAreaSearch;
        private string _elasticURL = ConfigurationManager.AppSettings["ElasticSearch:Host"].ToString();

        public ElasticSearchService()
        {
            _serviceSearch = new ServiceSearch();
            _organisationSearch = new OrganisationSearch();
            _accessibilityFeatureSearch = new AccessibilityFeatureSearch();
            _categorySearch = new CategorySearch();
            _communityGroupSearch = new CommunityGroupSearch();
            _serviceAreaSearch = new ServiceAreaSearch();
        }

        public Guid GetOrganisationIdFromSlug(string slug)
        {
            Guid organisationId = Task.Run(async () => await _organisationSearch.GetBySlugAsync(slug)).Result.id;
            return organisationId;
        }

        public OrganisationElasticSearchModel GetOrganisationById(Guid organisationId)
        {
            return _organisationSearch.GetById(organisationId);
        }

        public Guid GetServiceIdFromSlug(string slug)
        {
            Guid serviceId = Task.Run(async () => await _serviceSearch.GetBySlugAsync(slug)).Result.id;
            return serviceId;
        }

        public ServiceElasticSearchModel GetServiceById(Guid serviceId)
        {
            return _serviceSearch.GetById(serviceId);
        }

        public void AddServiceToElasticSearch(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);
                ServiceElasticSearchModel serviceToAdd = GetServiceToAddToElastic(dc, service);
                if (serviceToAdd != null && service.Published)
                {
                    Task.Run(async () => await _serviceSearch.AddServiceAsync(serviceToAdd));
                }
            }
        }

        public void AddServicesToElasticSearch(int from, int to)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = dc.Services
                    .Where(s => s.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted)
                    .OrderBy(x => x.CreatedOn)
                    .Skip(from - 1)
                    .Take(to - from + 1)
                    .ToList();
                List<ServiceElasticSearchModel> servicesToAdd = new List<ServiceElasticSearchModel>();
                foreach (Service service in services)
                {
                    if (service.Organisation.Published && service.Published)
                    {
                        ServiceElasticSearchModel serviceToAdd = GetServiceToAddToElastic(dc, service);

                        if (serviceToAdd != null)
                        {
                            servicesToAdd.Add(serviceToAdd);
                        }
                    }
                }

                _serviceSearch.AddServices(servicesToAdd);
            }
        }

        public void AddListOfServicesToElasticSearch(List<Service> services)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceElasticSearchModel> servicesToAdd = new List<ServiceElasticSearchModel>();
                foreach(Service service in services.OrderBy(s => s.CreatedOn))
                {
                    if (service.Organisation.Published && service.Published)
                    {
                        ServiceElasticSearchModel serviceToAdd = GetServiceToAddToElastic(dc, service);

                        if (serviceToAdd != null)
                        {
                            servicesToAdd.Add(serviceToAdd);
                        }
                    }
                }

                int groupSize = 500;
                for (int i = 0; i < servicesToAdd.Count; i+=groupSize)
                {
                    _serviceSearch.AddServices(servicesToAdd.Skip(i).Take(groupSize));
                }
            }
        }

        public void AddAllServicesToElasticSearch()
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = dc.Services.Where(s => s.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && !s.Suggested).ToList();
                List<ServiceElasticSearchModel> servicesToAdd = new List<ServiceElasticSearchModel>();
                foreach (Service service in services)
                {
                    if (service.Organisation.Published && service.Published)
                    {
                        ServiceElasticSearchModel serviceToAdd = GetServiceToAddToElastic(dc, service);

                        if (serviceToAdd != null)
                        {
                            servicesToAdd.Add(serviceToAdd);
                        }
                    }
                }

                Task.Run(async () => await _serviceSearch.AddServicesAsync(servicesToAdd));
            }
        }

        public void AddOrganisationToElasticSearch(Guid organisationId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(organisationId);
                if (organisation.Published)
                {
                    OrganisationElasticSearchModel orgToAdd = GetOrganisationToAddToElastic(dc, organisation);

                    Task.Run(async () => await _organisationSearch.AddOrganisationAsync(orgToAdd));
                }
            }
        }

        public void AddOrganisationsToElasticSearch(int from, int to)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Organisation> organisations = dc.Organisations
                    .Where(p => p.Published)
                    .OrderBy(x => x.CreatedOn)
                    .Skip(from - 1)
                    .Take(to - from + 1)
                    .ToList();
                List<OrganisationElasticSearchModel> organisationsToAdd = new List<OrganisationElasticSearchModel>();
                foreach (Organisation organisation in organisations)
                {
                    OrganisationElasticSearchModel orgToAdd = GetOrganisationToAddToElastic(dc, organisation);

                    organisationsToAdd.Add(orgToAdd);
                }

                _organisationSearch.AddOrganisations(organisationsToAdd);
            }
        }

        public void AddAllOrganisationsToElasticSearch()
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Organisation> organisations = dc.Organisations.Where(p => p.Published).ToList();
                List<OrganisationElasticSearchModel> organisationsToAdd = new List<OrganisationElasticSearchModel>();
                foreach (Organisation organisation in organisations)
                {
                    OrganisationElasticSearchModel orgToAdd = GetOrganisationToAddToElastic(dc, organisation);

                    organisationsToAdd.Add(orgToAdd);
                }

                Task.Run(async () => await _organisationSearch.AddOrganisationsAsync(organisationsToAdd));
            }
        }

        public void AddAllCategoriesToElasticSearch()
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Category> categories = dc.Categories
                    .Where(p => p.ParentCategoryId == null)
                    .OrderBy(n => n.Name)
                    .ToList();
                List<CategoryElasticSearchModel> categoriesToAdd = new List<CategoryElasticSearchModel>();

                foreach (Category category in categories)
                {
                    List<CategoryElasticSearchModel> levelTwoCategories = new List<CategoryElasticSearchModel>();
                    var levelOneItem = new CategoryElasticSearchModel()
                    {
                        id = category.CategoryId,
                        name = category.Name,
                        slug = category.Slug,
                        icon = category.Icon,
                    };

                    List<Category> levelTwoCats = dc.Categories
                        .Where(p => p.ParentCategoryId == category.CategoryId)
                        .OrderBy(n => n.Name)
                        .ToList();
                    foreach (var levelTwo in levelTwoCats)
                    {
                        List<CategoryElasticSearchModel> levelThreeCategories = new List<CategoryElasticSearchModel>();
                        CategoryElasticSearchModel levelTwoItem = new CategoryElasticSearchModel()
                        {
                            id = levelTwo.CategoryId,
                            name = levelTwo.Name,
                            slug = levelTwo.Slug
                        };

                        List<Category> levelThreeCats = dc.Categories
                            .Where(p => p.ParentCategoryId == levelTwo.CategoryId)
                            .OrderBy(n => n.Name)
                            .ToList();
                        foreach (Category levelThree in levelThreeCats)
                        {
                            CategoryElasticSearchModel levelThreeItem = new CategoryElasticSearchModel()
                            {
                                id = levelThree.CategoryId,
                                name = levelThree.Name,
                                slug = levelThree.Slug
                            };
                            levelThreeCategories.Add(levelThreeItem);
                        }
                        levelTwoItem.sub_categories = levelThreeCategories;
                        levelTwoCategories.Add(levelTwoItem);
                    }
                    levelOneItem.sub_categories = levelTwoCategories;
                    categoriesToAdd.Add(levelOneItem);
                }

                Task.Run(async () => await _categorySearch.AddCategories(categoriesToAdd));

            }
        }

        public void DeleteCategory(int categoryId)
        {
            _categorySearch.DeleteCategory(categoryId);
        }

        public void AddAllAccessibilityFeaturesToElasticSearch()
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<AccessibilityFeature> accessibilityFeatures = dc.AccessibilityFeatures
                    .Where(p => p.ParentAccessibilityFeatureId == null)
                    .OrderBy(d => d.DisplayOrder)
                    .ThenBy(n => n.Name)
                    .ToList();
                List<AccessibilityFeatureElasticSearchModel> accessibilityFeatureToAdd = new List<AccessibilityFeatureElasticSearchModel>();

                foreach (AccessibilityFeature accessibilityFeature in accessibilityFeatures)
                {
                    List<AccessibilityFeatureElasticSearchModel> levelTwoAccessibilityFeatures = new List<AccessibilityFeatureElasticSearchModel>();
                    AccessibilityFeatureElasticSearchModel levelOneItem = new AccessibilityFeatureElasticSearchModel()
                    {
                        id = accessibilityFeature.AccessibilityFeatureId,
                        name = accessibilityFeature.Name,
                        promptquestions = accessibilityFeature.PromptQuestions,
                        icon = accessibilityFeature.Icon,
                        slug = accessibilityFeature.Slug,
                        displayorder = accessibilityFeature.DisplayOrder,
                        is_virtual = accessibilityFeature.Virtual,
                        is_physical = accessibilityFeature.Physical
                    };

                    accessibilityFeatureToAdd.Add(levelOneItem);
                }

                Task.Run(async () => await _accessibilityFeatureSearch.AddAccessibilityFeatures(accessibilityFeatureToAdd));
            }
        }

        public void DeleteAccessibilityFeature(int featureId)
        {
            _accessibilityFeatureSearch.DeleteAccessibilityFeature(featureId);
        }

        public void AddAllCommunityGroupsToElasticSearch()
        {
			using (ALISSContext dc = new ALISSContext())
			{
				List<CommunityGroup> communityGroups = dc.CommunityGroups
                    .Where(p => p.ParentCommunityGroupId == null)
                    .OrderBy(o => o.DisplayOrder)
                    .ThenBy(n => n.Name)
                    .ToList();
				List<CommunityGroupElasticSearchModel> communityGroupToAdd = new List<CommunityGroupElasticSearchModel>();

				foreach (CommunityGroup communityGroup in communityGroups)
				{
					List<CommunityGroupElasticSearchModel> levelTwoCommunityGroups = new List<CommunityGroupElasticSearchModel>();
					CommunityGroupElasticSearchModel levelOneItem = new CommunityGroupElasticSearchModel()
					{
						id = communityGroup.CommunityGroupId,
						name = communityGroup.Name,
						slug = communityGroup.Slug,
                        displayorder = communityGroup.DisplayOrder,
                        isrange = communityGroup.IsMinMax
					};

					List<CommunityGroup> levelTwoCats = dc.CommunityGroups
                        .Where(p => p.ParentCommunityGroupId == communityGroup.CommunityGroupId)
                        .OrderBy(o => o.DisplayOrder)
                        .ThenBy(n => n.Name)
                        .ToList();

                    foreach (CommunityGroup levelTwo in levelTwoCats)
					{
						List<CommunityGroupElasticSearchModel> levelThreeCommunityGroups = new List<CommunityGroupElasticSearchModel>();
						CommunityGroupElasticSearchModel levelTwoItem = new CommunityGroupElasticSearchModel()
						{
							id = levelTwo.CommunityGroupId,
							name = levelTwo.Name,
							slug = levelTwo.Slug,
                            displayorder = levelTwo.DisplayOrder
						};

						List<CommunityGroup> levelThreeCats = dc.CommunityGroups
                            .Where(p => p.ParentCommunityGroupId == levelTwo.CommunityGroupId)
                            .OrderBy(o => o.DisplayOrder)
                            .ThenBy(n => n.Name)
                            .ToList();
						foreach (CommunityGroup levelThree in levelThreeCats)
						{
							CommunityGroupElasticSearchModel levelThreeItem = new CommunityGroupElasticSearchModel()
							{
								id = levelThree.CommunityGroupId,
								name = levelThree.Name,
								slug = levelThree.Slug,
                                displayorder = levelThree.DisplayOrder
							};
							levelThreeCommunityGroups.Add(levelThreeItem);
						}
						levelTwoItem.sub_communitygroups = levelThreeCommunityGroups;
						levelTwoCommunityGroups.Add(levelTwoItem);
					}
					levelOneItem.sub_communitygroups = levelTwoCommunityGroups;
					communityGroupToAdd.Add(levelOneItem);
				}

				Task.Run(async () => await _communityGroupSearch.AddCommunityGroups(communityGroupToAdd));
			}
		}

        public void DeleteCommunityGroup(int communityGroupId)
        {
            _communityGroupSearch.DeleteCommunityGroup(communityGroupId);
        }

        public void AddAllServiceAreasToElasticSearch()
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceArea> serviceAreas = dc.ServiceAreas.ToList();
                List<ServiceAreaElasticSearchModel> serviceAreasToAdd = new List<ServiceAreaElasticSearchModel>();

                foreach (ServiceArea serviceArea in serviceAreas)
                {
                    ServiceAreaElasticSearchModel serviceAreaToAdd = new ServiceAreaElasticSearchModel()
                    {
                        id = serviceArea.ServiceAreaId,
                        name = serviceArea.Name,
                        code = serviceArea.Code,
                        type = ((ServiceAreaTypeEnum)serviceArea.Type).GetDisplayName(),
                        geojson = serviceArea.GeoJson
                    };

                    serviceAreasToAdd.Add(serviceAreaToAdd);
                }

                Task.Run(async () => await _serviceAreaSearch.AddServiceAreas(serviceAreasToAdd));
            }
        }

        public void DeleteOrganisation(Guid organisationId)
        {
            _organisationSearch.DeleteOrganisation(organisationId);
        }

        public void DeleteService(Guid serviceId)
        {
            _serviceSearch.DeleteService(serviceId);
        }

        public void DeleteOrganisationIndex()
        {
            _organisationSearch.DeleteAllOrganisations();
        }

        public void DeleteServiceIndex()
        {
            _serviceSearch.DeleteAllServices();
        }

        private OrganisationElasticSearchModel GetOrganisationToAddToElastic(ALISSContext dc, Organisation organisation)
        {
            List<LocationModel> orgLocations = new List<LocationModel>();
            List<Location> locations = dc.ServiceLocations
                .Where(s => s.Service.OrganisationId == organisation.OrganisationId && s.Service.Published)
                .Select(l => l.Location)
                .Distinct()
                .ToList();
            List<Service> services = dc.Services.Where(o => o.OrganisationId == organisation.OrganisationId && o.Published).ToList();

            List<OrganisationServiceModel> orgServices = new List<OrganisationServiceModel>();
            OrganisationElasticSearchModel orgToAdd = new OrganisationElasticSearchModel()
            {
                id = organisation.OrganisationId,
                name = organisation.Name,
                created_on = organisation.CreatedOn,
                last_edited = organisation.UpdatedOn.HasValue ? organisation.UpdatedOn.Value : organisation.CreatedOn,
                description = organisation.Description,
                is_claimed = organisation.ClaimedUserId.HasValue,
                published = organisation.Published,
                slug = organisation.Slug,
                facebook = organisation.Facebook,
                twitter = organisation.Twitter,
                instagram = organisation.Instagram,
                url = organisation.Url,
                phone = organisation.PhoneNumber,
                email = organisation.Email,
            };

            foreach (Location location in locations)
            {
                LocationModel locationToAdd = new LocationModel()
                {
                    id = location.LocationId,
                    street_address = location.Address,
                    name = location.Name,
                    postal_code = location.Postcode,
                    locality = location.City,
                    country = "GB",
                    formatted_address = String.IsNullOrEmpty(location.Name) ? $"{location.Address}, {location.City}, {location.Postcode}" : $"{location.Name}, {location.Address}, {location.City}, {location.Postcode}",
                    point = new GeoLocation(location.Latitude, location.Longitude),
                };

                orgLocations.Add(locationToAdd);
            }
            orgToAdd.locations = orgLocations.ToList();

            foreach (Service service in services)
            {

                List<ServiceAreaElasticSearchModel> serviceAreas = new List<ServiceAreaElasticSearchModel>();
                List<ServiceCategoryModel> serviceCategories = new List<ServiceCategoryModel>();
                OrganisationServiceModel serviceToAdd = new OrganisationServiceModel()
                {
                    id = service.ServiceId,
                    name = service.Name,
                    slug = service.Slug,
                    description = service.Description,
                    summary = service.Summary,
                    email = service.Email,
                    instagram = service.Instagram,
                    facebook = service.Facebook,
                    twitter = service.Twitter,
                    phone = service.Phone,
                    url = service.Url,
                    referral_url = service.ReferralUrl,
                    is_claimed = service.ClaimedUserId.HasValue
                };

                foreach (ServiceArea serviceArea in dc.ServiceServiceAreas.Include(s => s.ServiceArea).Where(s => s.ServiceId == service.ServiceId).Select(s => s.ServiceArea).ToList())
                {
                    serviceAreas.Add(new ServiceAreaElasticSearchModel()
                    {
                        name = serviceArea.Name,
                        code = serviceArea.Code,
                        id = serviceArea.ServiceAreaId,
                        type = serviceArea.Type.ToString()
                    });
                }
                serviceToAdd.service_areas = serviceAreas;

                foreach (Category category in dc.ServiceCategories.Include(c => c.Category).Where(s => s.ServiceId == service.ServiceId).Select(c => c.Category).ToList())
                {
                    serviceCategories.Add(new ServiceCategoryModel()
                    {
                        id = category.CategoryId,
                        name = category.Name,
                        slug = category.Slug,
                    });
                }
                serviceToAdd.categories = serviceCategories;
                orgServices.Add(serviceToAdd);
            }
            orgToAdd.services = orgServices;

            return orgToAdd;
        }

        private ServiceElasticSearchModel GetServiceToAddToElastic(ALISSContext dc, Service service)
        {

            List<ServiceAreaElasticSearchModel> serviceAreas = new List<ServiceAreaElasticSearchModel>();
            List<ServiceCategoryModel> serviceCategories = new List<ServiceCategoryModel>();
            List<ServiceAccessibilityFeatureModel> serviceAccessibilityFeatures = new List<ServiceAccessibilityFeatureModel>();
            List<ServiceCommunityGroupModel> serviceCommunityGroups = new List<ServiceCommunityGroupModel>();
            List<LocationModel> serviceLocations = new List<LocationModel>();
            List<ServiceMedia> serviceMediaGallery = new List<ServiceMedia>();
            ServiceReview serviceReview = dc.ServiceReviews.FirstOrDefault(r => r.ServiceId == service.ServiceId);
            if (serviceReview == null)
            {
                serviceReview = new ServiceReview();
            }

            if (service.Organisation.Published)
            {
                ServiceElasticSearchModel serviceToAdd = new ServiceElasticSearchModel()
                {
                    id = service.ServiceId,
                    created_on = service.CreatedOn,
                    description = service.Description,
                    summary = service.Summary,
                    email = service.Email,
                    last_edited = service.UpdatedOn.HasValue ? service.UpdatedOn.Value : service.CreatedOn,
                    last_reviewed = serviceReview.LastReviewedDate.HasValue && serviceReview.LastReviewedDate.Value != DateTime.MinValue && serviceReview.LastReviewedUserId.HasValue ? serviceReview.LastReviewedDate.Value : service.UpdatedOn ?? service.CreatedOn,
                    name = service.Name,
                    phone = service.Phone,
                    slug = service.Slug,
                    instagram = service.Instagram,
                    facebook = service.Facebook,
                    twitter = service.Twitter,
                    url = service.Url,
                    referral_url = service.ReferralUrl,
                    is_claimed = service.ClaimedUserId.HasValue,
                    is_deprioritised = service.Deprioritised,
                    organisation = new ServiceOrganisationModel()
                    {
                        id = service.Organisation.OrganisationId,
                        name = service.Organisation.Name,
                        slug = service.Organisation.Slug,
                        is_claimed = service.Organisation.ClaimedUserId.HasValue
                    },
                };

                foreach (ServiceArea serviceArea in dc.ServiceServiceAreas.Include(s => s.ServiceArea).Where(s => s.ServiceId == service.ServiceId).Select(s => s.ServiceArea).ToList())
                {
                    serviceAreas.Add(new ServiceAreaElasticSearchModel()
                    {
                        name = serviceArea.Name,
                        code = serviceArea.Code,
                        id = serviceArea.ServiceAreaId,
                        type = ((ServiceAreaTypeEnum)serviceArea.Type).GetDisplayName()
                    });
                }
                serviceToAdd.service_areas = serviceAreas;

                foreach (Category category in dc.ServiceCategories.Include(c => c.Category).Where(s => s.ServiceId == service.ServiceId).Select(c => c.Category).ToList())
                {
                    serviceCategories.Add(new ServiceCategoryModel()
                    {
                        id = category.CategoryId,
                        name = category.Name,
                        slug = category.Slug,
                        selected = !serviceCategories.Any(sc => sc.id == category.CategoryId && sc.selected == true),
                    });

                    if (category.ParentCategoryId != null && !(serviceCategories.Select(i => i.id).ToList().Contains(category.ParentCategoryId.Value)))
                    {
                        Category parentCategory = dc.Categories.Find(category.ParentCategoryId);
                        serviceCategories.Add(new ServiceCategoryModel()
                        {
                            id = parentCategory.CategoryId,
                            name = parentCategory.Name,
                            slug = parentCategory.Slug,
                            selected = dc.ServiceCategories.Where(s => s.ServiceId == service.ServiceId).Select(c => c.CategoryId).ToList().Contains(parentCategory.CategoryId) && !serviceCategories.Any(sc => sc.id == parentCategory.CategoryId && sc.selected == true),
                        });

                        if (parentCategory.ParentCategoryId != null)
                        {
                            Category topLevelCategory = dc.Categories.Find(parentCategory.ParentCategoryId);
                            serviceCategories.Add(new ServiceCategoryModel()
                            {
                                id = topLevelCategory.CategoryId,
                                name = topLevelCategory.Name,
                                slug = topLevelCategory.Slug,
                                selected = dc.ServiceCategories.Where(s => s.ServiceId == service.ServiceId).Select(c => c.CategoryId).ToList().Contains(topLevelCategory.CategoryId),
                            });
                        }
                    }
                }
                serviceToAdd.categories = serviceCategories;

                foreach (ServiceAccessibilityFeature accessibilityFeature in dc.ServiceAccessibilityFeatures.Include(c => c.AccessibilityFeature).Where(s => s.ServiceId == service.ServiceId).ToList())
                {
                    serviceAccessibilityFeatures.Add(new ServiceAccessibilityFeatureModel()
                    {
                        id = accessibilityFeature.AccessibilityFeatureId,
                        slug = accessibilityFeature.AccessibilityFeature.Slug,
                        name = accessibilityFeature.AccessibilityFeature.Name,
                        additional_info = accessibilityFeature.AdditionalInfo,
                        icon = accessibilityFeature.AccessibilityFeature.Icon,
                        location_id = accessibilityFeature.LocationId
                    });
                }
                serviceToAdd.accessibility_features = serviceAccessibilityFeatures;

                foreach (ServiceCommunityGroup communityGroup in dc.ServiceCommunityGroups.Include(c => c.CommunityGroup).Where(s => s.ServiceId == service.ServiceId).ToList())
                {
                    serviceCommunityGroups.Add(new ServiceCommunityGroupModel()
                    {
                        id = communityGroup.CommunityGroupId,
                        name = communityGroup.CommunityGroup.Name,
                        slug = communityGroup.CommunityGroup.Slug,
                        is_range = communityGroup.CommunityGroup.IsMinMax,
                        min_value = communityGroup.MinValue,
                        max_value = communityGroup.MaxValue,
                        selected = !serviceCommunityGroups.Any(sc => sc.id == communityGroup.CommunityGroupId && sc.selected),
                    });

                    if (communityGroup.CommunityGroup.ParentCommunityGroupId != null && !(serviceCommunityGroups.Select(i => i.id).ToList().Contains(communityGroup.CommunityGroup.ParentCommunityGroupId.Value)))
                    {
                        CommunityGroup parentCommunityGroup = dc.CommunityGroups.Find(communityGroup.CommunityGroup.ParentCommunityGroupId);
                        ServiceCommunityGroup parentServiceCommunityGroup = dc.ServiceCommunityGroups.Find(communityGroup.ServiceId, communityGroup.CommunityGroup.ParentCommunityGroupId);
                        ServiceCommunityGroupModel parentGroup = new ServiceCommunityGroupModel()
                        {
                            id = parentCommunityGroup.CommunityGroupId,
                            name = parentCommunityGroup.Name,
                            slug = parentCommunityGroup.Slug,
                            is_range = parentCommunityGroup.IsMinMax,
                            selected = dc.ServiceCommunityGroups.Where(s => s.ServiceId == service.ServiceId).Select(c => c.CommunityGroupId).ToList().Contains(parentCommunityGroup.CommunityGroupId) && !serviceCommunityGroups.Any(sc => sc.id == parentCommunityGroup.CommunityGroupId && sc.selected == true),
                        };

                        if(parentServiceCommunityGroup != null)
                        {
                            parentGroup.max_value = parentServiceCommunityGroup.MaxValue;
                            parentGroup.min_value = parentServiceCommunityGroup.MinValue;
                        }

                        serviceCommunityGroups.Add(parentGroup);
                    }
                }
                serviceToAdd.community_groups = serviceCommunityGroups;

                foreach (MediaGallery media in dc.MediaGallery.Where(s => s.ServiceId == service.ServiceId).ToList())
                {
                    if (media.Approved)
                    {
                        string thumbnail = "";
                        if (media.Type == "Video")
                        {
                            thumbnail = media.MediaUrl.Contains("vimeo") ? GetVimeoThumbnail(media.MediaUrl) : GetYoutubeThumbnail(media.MediaUrl);
                        }

                        serviceMediaGallery.Add(new ServiceMedia()
                        {
                            id = media.MediaGalleryId,
                            url = media.MediaUrl,
                            caption = media.Caption,
                            alt_text = media.AltText,
                            type = media.Type,
                            thumbnail = thumbnail,
                        });
                    }
                }
                serviceToAdd.media_gallery = serviceMediaGallery;

                foreach (Location location in dc.ServiceLocations.Include(l => l.Location).Where(s => s.ServiceId == service.ServiceId).Select(l => l.Location).ToList())
                {
                    serviceLocations.Add(new LocationModel()
                    {
                        id = location.LocationId,
                        street_address = location.Address,
                        name = location.Name,
                        postal_code = location.Postcode,
                        locality = location.City,
                        country = "GB",
                        formatted_address = String.IsNullOrEmpty(location.Name) ? $"{location.Address}, {location.City}, {location.Postcode}" : $"{location.Name}, {location.Address}, {location.City}, {location.Postcode}",
                        point = new GeoLocation(location.Latitude, location.Longitude),
                    });
                }
                serviceToAdd.locations = serviceLocations;

                serviceToAdd.location_score = GetLocationScore(
                    serviceAreas.Select(t => t.type).Any(c => c == "Country"),
                    serviceAreas.Select(t => t.type).Any(c => c != "Country"),
                    serviceLocations.Any());

                return serviceToAdd;
            }
            else
            {
                return null;
            }
        }

        public class VimeoJson
        {
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public string upload_date { get; set; }
            public string thumbnail_small { get; set; }
            public string thumbnail_medium { get; set; }
            public string thumbnail_large { get; set; }
            public int user_id { get; set; }
            public string user_name { get; set; }
            public string user_url { get; set; }
            public string user_portrait_small { get; set; }
            public string user_portrait_medium { get; set; }
            public string user_portrait_large { get; set; }
            public string user_portrait_huge { get; set; }
            public int stats_number_of_likes { get; set; }
            public int stats_number_of_plays { get; set; }
            public int stats_number_of_comments { get; set; }
            public int duration { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string tags { get; set; }
            public string embed_privacy { get; set; }
        }


        class VimeoThumbnailModel
        {
            public string thumbnail_large { get; set; }
        }

        public string GetVimeoThumbnail(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://vimeo.com/api/v2/video/" + url.Split('/')[url.Split('/').Length - 1] + ".json");

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        var video = js.Deserialize<List<VimeoJson>>(objText);

                        return video.First().thumbnail_large;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    return "";
                }
            }

            return "";
        }


        public class Rootobject
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public Item[] items { get; set; }
            public Pageinfo pageInfo { get; set; }
        }

        public class Pageinfo
        {
            public int totalResults { get; set; }
            public int resultsPerPage { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public Snippet snippet { get; set; }
        }

        public class Snippet
        {
            public DateTime publishedAt { get; set; }
            public string channelId { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public Thumbnails thumbnails { get; set; }
            public string channelTitle { get; set; }
            public string categoryId { get; set; }
            public string liveBroadcastContent { get; set; }
            public Localized localized { get; set; }
        }

        public class Thumbnails
        {
            public Default _default { get; set; }
            public Medium medium { get; set; }
            public High high { get; set; }
            public Standard standard { get; set; }
            public Maxres maxres { get; set; }
        }

        public class Default
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Medium
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class High
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Standard
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Maxres
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Localized
        {
            public string title { get; set; }
            public string description { get; set; }
        }


        public string GetYoutubeThumbnail(string url)
        {
            string googleApiKey = ConfigurationManager.AppSettings["Settings:GoogleApiKey"] ?? "";
            string youtubeId = "";

            if (url.Contains("youtube.com"))
            {
                youtubeId = url.Split('=')[1];
            }
            else if (url.Contains("youtu.be"))
            {
                youtubeId = url.Substring(url.LastIndexOf('/') + 1);
                if (youtubeId.Contains("?"))
                {
                    youtubeId = youtubeId.Split('?')[0];
                }
            }

            if (string.IsNullOrEmpty(youtubeId))
            {
                return "";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={youtubeId}&key={googleApiKey}");

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        var video = (Rootobject)js.Deserialize(objText, typeof(Rootobject));

                        return video.items[0].snippet.thumbnails.standard.url;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    return "";
                }
            }

            return "";
        }
        private int GetLocationScore(bool country, bool region, bool address)
        {
            /**
             * 1: service with address (NO SERVICE AREA) 
             * 2: service with address + regions (AT LEAST ONE NOT SCOTLAND/UK)
             * 3: service with address + Scotland/UK (NO REGIONS EXCEPT SCOTLAND/UK)
             * 4: service with regions (NO ADDRESS)
             * 5: service with Scotland/UK (NO ADDRESS)
             * 6: anything else (shouldn't ever be the case)
             */
            if (address)
            {
                if (!region && !country)
                {
                    return 1;
                }
                else if (region)
                {
                    return 2;
                }
                else if (country)
                {
                    return 3;
                }
            }
            else if (region)
            {
                return 4;
            }
            else if (country)
            {
                return 5;
            }

            return 6;
        }

        public void ReindexServicesOnCategoryChange(int categoryId)
        {
            Category changedCategory = null;
            using (ALISSContext dc = new ALISSContext())
            {
                changedCategory = dc.Categories.Find(categoryId);
            }

            List<ServiceElasticSearchModel> services = Task.Run(async () => await _serviceSearch.SearchServicesByCategoryIdAsync(categoryId)).Result;

            foreach (ServiceElasticSearchModel service in services)
            {
                foreach (ServiceCategoryModel category in service.categories.Where(c => c.id == categoryId))
                {
                    category.slug = changedCategory.Slug;
                    category.name = changedCategory.Name;
                }
            }

            _serviceSearch.AddServices(services);
        }

        public void ReindexServicesOnCommunityGroupChange(int communityGroupId)
        {
            CommunityGroup changedCommunityGroup = null;
            using (ALISSContext dc = new ALISSContext())
            {
                changedCommunityGroup = dc.CommunityGroups.Find(communityGroupId);
            }

            List<ServiceElasticSearchModel> services = Task.Run(async () => await _serviceSearch.SearchServicesByCommunityGroupIdAsync(communityGroupId)).Result;

            foreach (ServiceElasticSearchModel service in services)
            {
                foreach (ServiceCommunityGroupModel communityGroup in service.community_groups.Where(c => c.id == communityGroupId))
                {
                    communityGroup.slug = changedCommunityGroup.Slug;
                    communityGroup.name = changedCommunityGroup.Name;
                }
            }

            _serviceSearch.AddServices(services);
        }

        public void ReindexServicesOnAccessibilityFeatureChange(int accessibilityFeatureId)
        {
            AccessibilityFeature changedAccessibilityFeature = null;
            using (ALISSContext dc = new ALISSContext())
            {
                changedAccessibilityFeature = dc.AccessibilityFeatures.Find(accessibilityFeatureId);
            }

            List<ServiceElasticSearchModel> services = Task.Run(async () => await _serviceSearch.SearchServicesByAccessibilityFeatureIdAsync(accessibilityFeatureId)).Result;

            foreach (ServiceElasticSearchModel service in services)
            {
                foreach (ServiceAccessibilityFeatureModel accessibilityFeature in service.accessibility_features.Where(c => c.id == accessibilityFeatureId))
                {
                    accessibilityFeature.slug = changedAccessibilityFeature.Slug;
                    accessibilityFeature.name = changedAccessibilityFeature.Name;
                }
            }

            _serviceSearch.AddServices(services);
        }
    }
}

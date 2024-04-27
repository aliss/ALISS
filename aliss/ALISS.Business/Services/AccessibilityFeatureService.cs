using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.AccessibilityFeature;
using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.ViewModels.AccessibilityFeature;
using ALISS.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Windows.Media;

namespace ALISS.Business.Services
{
    public class AccessibilityFeatureService
    {

        private ElasticSearchService _elasticSearchService;

        public AccessibilityFeatureService()
        {
            _elasticSearchService = new ElasticSearchService();
        }

        public AccessibilityFeatureListingViewModel ListAccessibilityFeatures(string searchTerm)
        {
			AccessibilityFeatureListingViewModel accessibilityFeatureList = new AccessibilityFeatureListingViewModel()
            {
                SearchTerm = searchTerm,
				AccessibilityFeatures = new List<AccessibilityFeaturePTO>(),
                TotalResults = 0
            };
            
            using (ALISSContext dc = new ALISSContext())
            {
                List<AccessibilityFeature> levelOneAccessibilityFeatures = dc.AccessibilityFeatures.Where(p => p.ParentAccessibilityFeatureId == null).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name).ToList();
                foreach (AccessibilityFeature accessibilityFeature in levelOneAccessibilityFeatures)
                {
                    bool matchesSearch = true;
                    int levelTwoCount = 0;
                    int levelThreeCount = 0;
                    List<ServiceAccessibilityFeature> accessibilityFeatures = dc.ServiceAccessibilityFeatures.Where(c => c.AccessibilityFeatureId == accessibilityFeature.AccessibilityFeatureId && c.Service.Published && c.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted).ToList();
                    List<Guid> serviceIds = new List<Guid>();
                    int ServiceCount = 0;

                    foreach(var feature in accessibilityFeatures)
                    {
                        if (!serviceIds.Contains(feature.ServiceId))
                        {
                            ServiceCount++;
                            serviceIds.Add(feature.ServiceId);
                        }
                    }

                    AccessibilityFeaturePTO accessibilityFeatureListItem = new AccessibilityFeaturePTO()
                    {
						AccessibilityFeatureId = accessibilityFeature.AccessibilityFeatureId,
                        Name = accessibilityFeature.Name,
                        DisplayOrder = accessibilityFeature.DisplayOrder,
                        ServiceCount = ServiceCount,
                    };

                    if (!String.IsNullOrEmpty(searchTerm) && !accessibilityFeatureListItem.Name.ToLower().Contains(searchTerm.ToLower()))
                    {
                        matchesSearch = false;
                    }

                    if (matchesSearch || (!matchesSearch && (levelTwoCount > 0 || levelThreeCount > 0)))
                    {
                        accessibilityFeatureList.AccessibilityFeatures.Add(accessibilityFeatureListItem);
                    }
                }
                accessibilityFeatureList.TotalResults += accessibilityFeatureList.AccessibilityFeatures.Count;
                accessibilityFeatureList.AccessibilityFeatures = accessibilityFeatureList.AccessibilityFeatures.OrderBy(d => d.DisplayOrder).ThenBy(n => Regex.Replace(n.Name.ToLower(), "[$&+,:;=?@#.\"|'\\-<>.^*()%!]", string.Empty)).ToList();
            }
            
            return accessibilityFeatureList;
        }

        public string AddTopLevelAccessibilityFeature(EditTopLevelAccessibilityFeatureViewModel model)
        {
            if (DoesAccessibilityFeatureExist(model.Name))
            {
                return $"Error: The accessibility feature {model.Name} already exists, please choose another name.";
            }

            using (ALISSContext dc = new ALISSContext())
            {
				AccessibilityFeature accessibilityFeatureToAdd = new AccessibilityFeature()
                {
                    Name = model.Name,
                    Slug = model.Slug,
                    PromptQuestions = model.PromptQuestions,
                    Icon = model.Icon,
                    DisplayOrder = model.DisplayOrder,
                    Physical = model.AvailableFor == "Physical" || model.AvailableFor == "Both",
                    Virtual = model.AvailableFor == "Virtual" || model.AvailableFor == "Both"
                };

                dc.AccessibilityFeatures.Add(accessibilityFeatureToAdd);

                dc.SaveChanges();
            }

            _elasticSearchService.AddAllAccessibilityFeaturesToElasticSearch();

            return "Accessibility Feature added successfully";
        }

        public EditTopLevelAccessibilityFeatureViewModel GetTopLevelAccessibilityFeatureForEdit(int accessibilityFeatureId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                AccessibilityFeature accessibilityFeatureToEdit = dc.AccessibilityFeatures.Find(accessibilityFeatureId);
                return new EditTopLevelAccessibilityFeatureViewModel()
                {
                    AccessibilityFeatureId = accessibilityFeatureToEdit.AccessibilityFeatureId,
                    Name = accessibilityFeatureToEdit.Name,
                    Slug = accessibilityFeatureToEdit.Slug,
                    PromptQuestions = accessibilityFeatureToEdit.PromptQuestions,
                    Icon = accessibilityFeatureToEdit.Icon,
                    DisplayOrder = accessibilityFeatureToEdit.DisplayOrder,
                    AvailableFor = accessibilityFeatureToEdit.Virtual && accessibilityFeatureToEdit.Physical
                        ? "Both"
                        : accessibilityFeatureToEdit.Virtual
                            ? "Virtual"
                            : "Physical"
                };
            }
        }

        public string EditTopLevelAccessibilityFeature(EditTopLevelAccessibilityFeatureViewModel model)
        {
            bool nameOrSlugChanged = false;

            using (ALISSContext dc = new ALISSContext())
            {
				AccessibilityFeature accessibilityFeatureToEdit = dc.AccessibilityFeatures.Find(model.AccessibilityFeatureId);
                if (model.Name != accessibilityFeatureToEdit.Name && DoesAccessibilityFeatureExist(model.Name))
                {
                    return $"Error: The accessibility feature {model.Name} already exists, please choose another name.";
                }

                nameOrSlugChanged = !accessibilityFeatureToEdit.Name.Equals(model.Name) || !accessibilityFeatureToEdit.Slug.Equals(model.Slug);

                accessibilityFeatureToEdit.Name = model.Name;
                accessibilityFeatureToEdit.Slug = model.Slug;
                accessibilityFeatureToEdit.PromptQuestions = model.PromptQuestions;
                accessibilityFeatureToEdit.Icon = model.Icon;
                accessibilityFeatureToEdit.DisplayOrder = model.DisplayOrder;
                accessibilityFeatureToEdit.Physical = model.AvailableFor == "Physical" || model.AvailableFor == "Both";
                accessibilityFeatureToEdit.Virtual = model.AvailableFor == "Virtual" || model.AvailableFor == "Both";

                dc.SaveChanges();
            }

            _elasticSearchService.AddAllAccessibilityFeaturesToElasticSearch();

            if (nameOrSlugChanged)
            {
                _elasticSearchService.ReindexServicesOnAccessibilityFeatureChange(model.AccessibilityFeatureId);
            }

            return "Accessibility Feature edited successfully";
        }

        public DeleteAccessibilityFeatureViewModel GetAccessibilityFeatureForDelete(int accessibilityFeatureId)
        {
			DeleteAccessibilityFeatureViewModel model = new DeleteAccessibilityFeatureViewModel()
            {
				AccessibilityFeatureId = accessibilityFeatureId,
                CanDelete = true,
                RelatedServices = new List<RelatedServicePTO>()
            };

            using (ALISSContext dc = new ALISSContext())
            {
				AccessibilityFeature accessibilityFeatureToDelete = dc.AccessibilityFeatures.Find(accessibilityFeatureId);
                model.AccessibilityFeatureName = accessibilityFeatureToDelete.Name;

                if (dc.AccessibilityFeatures.Count(p => p.ParentAccessibilityFeatureId == accessibilityFeatureId) > 0)
                {
                    model.CanDelete = false;
                    return model;
                }
                else
                {
                    var relatedServices = dc.ServiceAccessibilityFeatures
                        .Include(a => a.Service)
                        .Where(a => a.AccessibilityFeatureId == accessibilityFeatureId && a.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted)
                        .Select(a => a.Service)
                        .ToList();
                    foreach (Service service in relatedServices.Distinct())
                    {
                        model.RelatedServices.Add(new RelatedServicePTO()
                        {
                            ServiceId = service.ServiceId,
                            ServiceName = service.Name,
                            OrganisationId = service.OrganisationId
                        });
                    }
                }
            }

            return model;
        }

        public void DeleteAccessibilityFeature(int accessibilityFeatureId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var relatedServices = dc.ServiceAccessibilityFeatures.Where(c => c.AccessibilityFeatureId == accessibilityFeatureId);
                var serviceList = relatedServices.ToList();
                if (relatedServices.Count() > 0)
                {
                    dc.ServiceAccessibilityFeatures.RemoveRange(relatedServices);
                }

				AccessibilityFeature accessibilityFeatureToDelete = dc.AccessibilityFeatures.Find(accessibilityFeatureId);
                dc.AccessibilityFeatures.Remove(accessibilityFeatureToDelete);
                dc.SaveChanges();

                List<Guid> serviceReIndexed = new List<Guid>();
                foreach (var service in serviceList)
                {
                    if (!serviceReIndexed.Contains(service.ServiceId))
                    {
                        _elasticSearchService.AddServiceToElasticSearch(service.ServiceId);
                        serviceReIndexed.Add(service.ServiceId);
                    }
                }
            }

            _elasticSearchService.DeleteAccessibilityFeature(accessibilityFeatureId);
        }

        public List<SelectListItem> GetAccessibilityFeatureDropdown(int parentAccessibilityFeatureId, int currentAccessibilityFeatureId = 0)
        {
            List<SelectListItem> accessibilityFeatures = new List<SelectListItem>();

            using (ALISSContext dc = new ALISSContext())
            {
                List<AccessibilityFeature> levelOneAccessibilityFeatures = dc.AccessibilityFeatures.Where(p => p.ParentAccessibilityFeatureId == null).ToList();

                foreach (AccessibilityFeature levelOneAccessibilityFeature in levelOneAccessibilityFeatures)
                {
                    SelectListItem accessibilityFeatureToAdd = new SelectListItem()
                    {
                        Value = levelOneAccessibilityFeature.AccessibilityFeatureId.ToString(),
                        Text = levelOneAccessibilityFeature.Name,
                        Selected = levelOneAccessibilityFeature.AccessibilityFeatureId == parentAccessibilityFeatureId
                    };
                    accessibilityFeatures.Add(accessibilityFeatureToAdd);

                    List<AccessibilityFeature> levelTwoAccessibilityFeatures = dc.AccessibilityFeatures.Where(p => p.ParentAccessibilityFeatureId == levelOneAccessibilityFeature.AccessibilityFeatureId).ToList();

                    foreach (AccessibilityFeature levelTwoAccessibilityFeature in levelTwoAccessibilityFeatures)
                    {
                        accessibilityFeatureToAdd = new SelectListItem()
                        {
                            Value = levelTwoAccessibilityFeature.AccessibilityFeatureId.ToString(),
                            Text = $"-- {levelTwoAccessibilityFeature.Name}",
                            Selected = levelTwoAccessibilityFeature.AccessibilityFeatureId == parentAccessibilityFeatureId
                        };

                        if (currentAccessibilityFeatureId == 0 || (currentAccessibilityFeatureId != 0 && levelTwoAccessibilityFeature.AccessibilityFeatureId != currentAccessibilityFeatureId))
                        {
                            accessibilityFeatures.Add(accessibilityFeatureToAdd);
                        }
                    }
                }
            }

            return accessibilityFeatures;
        }

        public List<ServiceAccessibilityFeaturePTO> GetAccessibilityFeatureListForService(Guid? serviceId, List<string> selectedAccessibilityFeatures, Guid? locationId = null)
        {
            List<ServiceAccessibilityFeaturePTO> accessibilityFeatureList = new List<ServiceAccessibilityFeaturePTO>();
            
            using (ALISSContext dc = new ALISSContext())
            {
                List<AccessibilityFeature> levelOneAccessibilityFeatures = locationId.HasValue ? 
                    dc.AccessibilityFeatures.Where(p => p.ParentAccessibilityFeatureId == null && p.Physical).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name).ToList() : 
                    dc.AccessibilityFeatures.Where(p => p.ParentAccessibilityFeatureId == null && p.Virtual).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name).ToList();
                
                foreach (AccessibilityFeature accessibilityFeature in levelOneAccessibilityFeatures)
                {
                    ServiceAccessibilityFeature selectedFeature = dc.ServiceAccessibilityFeatures.SingleOrDefault(c => c.AccessibilityFeatureId == accessibilityFeature.AccessibilityFeatureId && c.ServiceId == serviceId.Value && c.LocationId == locationId.Value);
                    bool selected = selectedFeature != null;
                    string additionalInfo = selected ? selectedFeature.AdditionalInfo : "";

                    ServiceAccessibilityFeaturePTO accessibilityFeatureListItem = new ServiceAccessibilityFeaturePTO()
                    {
						AccessibilityFeatureId = accessibilityFeature.AccessibilityFeatureId,
                        Name = accessibilityFeature.Name,
                        PromptQuestions = accessibilityFeature.PromptQuestions,
                        Icon = accessibilityFeature.Icon,
                        Selected = selected,
                        AdditionalInfo = additionalInfo
                    };

                    accessibilityFeatureList.Add(accessibilityFeatureListItem);
                }
            }

            return accessibilityFeatureList;
        }

        private bool DoesAccessibilityFeatureExist(string accessibilityFeatureName)
        {
            using (ALISSContext dc = new ALISSContext())
            {
				AccessibilityFeature accessibilityFeature = dc.AccessibilityFeatures.Where(n => n.Name.ToLower() == accessibilityFeatureName.ToLower()).FirstOrDefault();

                return accessibilityFeature != null;
            }
        }
    }
}

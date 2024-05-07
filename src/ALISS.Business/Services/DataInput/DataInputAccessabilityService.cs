using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.AccessibilityFeature;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;
using System.Data.Entity;
using System.Web.UI.WebControls;
using ALISS.Business.PresentationTransferObjects.DataInput;
using System.Text.RegularExpressions;

namespace ALISS.Business.Services
{
    partial class DataInputService
    {
        public AccessibilityViewModel GetAccessibilityForEdit(Guid serviceId)
        {
            AccessibilityViewModel model;
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.FirstOrDefault(o => o.ServiceId == serviceId);

                model = new AccessibilityViewModel()
                {
                    OrganisationId = service.OrganisationId,
                    ServiceId = serviceId,
                    ServiceLocations = new List<ServiceLocationAccessibilityFeaturePTO>()
                };

                model.ServiceLocations = GetServiceAccessibilityLocations(serviceId, service.HowServiceAccessed);
            }

            return model;
        }

        public List<ServiceLocationAccessibilityFeaturePTO> GetServiceAccessibilityLocations(Guid serviceId, string howServiceAccessed)
        {
            List<ServiceLocationAccessibilityFeaturePTO> ServiceAccessibilityLocations = new List<ServiceLocationAccessibilityFeaturePTO>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceLocation> serviceLocations = dc.ServiceLocations.Where(s => s.ServiceId == serviceId).ToList();
                List<ServiceServiceArea> serviceAreas = dc.ServiceServiceAreas.Where(s => s.ServiceId == serviceId).ToList();


                if (howServiceAccessed.ToLower().Equals("hybrid") || howServiceAccessed.ToLower().Equals("inperson"))
                {
                    foreach (ServiceLocation location in serviceLocations)
                    {
                        string title = string.IsNullOrEmpty(location.Location.Name) ? "" : location.Location.Name + ", ";
                        title += location.Location.Address + ", " + location.Location.City + ", " + location.Location.Postcode;
                        ServiceAccessibilityLocations.Add(new ServiceLocationAccessibilityFeaturePTO
                        {
                            Title = title,
                            LocationId = location.LocationId,
                            AccessibilityFeatures = _accessibilityFeatureService.GetAccessibilityFeatureListForService(serviceId, GetSelectedPhysicalLocationAccessibilityFeatures(serviceId, location.LocationId), location.LocationId)
                        });
                    }
                }

                if (howServiceAccessed.ToLower().Equals("hybrid") || howServiceAccessed.ToLower().Equals("remote"))
                {
                    if (serviceAreas.Count > 0)
                    {
                        ServiceAccessibilityLocations.Add(new ServiceLocationAccessibilityFeaturePTO
                        {
                            Title = "Virtual",
                            AccessibilityFeatures = _accessibilityFeatureService.GetAccessibilityFeatureListForService(serviceId, GetSelectedVirtualLocationAccessibilityFeatures(serviceId))
                        });
                    }
                }
            }

            return ServiceAccessibilityLocations;
        }

        public string EditAccessibility(AccessibilityViewModel model, int userProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(model.ServiceId);

                if (serviceToEdit.LastEditedStep < (int)DataInputStepsEnum.AccessibilityTestStep)
                {
                    serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.AccessibilityTestStep;
                }
                serviceToEdit.UpdatedUserId = userProfileId;
                serviceToEdit.UpdatedOn = DateTime.UtcNow.Date;

                var selectedAccessibility = dc.ServiceAccessibilityFeatures.Where(s => s.ServiceId == serviceToEdit.ServiceId);
                dc.ServiceAccessibilityFeatures.RemoveRange(selectedAccessibility);
                if (!String.IsNullOrEmpty(model.SelectedAccessibilityFeatures))
                {
                    List<string> serviceAccessibilityFeatures = model.SelectedAccessibilityFeatures.Split('¬').ToList();

                    foreach (var accessibilityFeature in serviceAccessibilityFeatures)
                    {
                        string accessibilityFeatureLocation = System.Text.RegularExpressions.Regex.Split(accessibilityFeature, @"\|\|")[0];
                        string accessibilityFeatureId = System.Text.RegularExpressions.Regex.Split(accessibilityFeature, @"\|\|")[1].Split('|')[0];
                        string accessibilityFeatureMoreInfo = System.Text.RegularExpressions.Regex.Split(accessibilityFeature, @"\|\|")[1].Split('|')[1];
                        ServiceAccessibilityFeature accessibilityToAdd;
                        if (!accessibilityFeatureLocation.ToLower().Equals("virtual"))
                        {
                            accessibilityToAdd = new ServiceAccessibilityFeature()
                            {
                            ServiceId = serviceToEdit.ServiceId,
                            AccessibilityFeatureId = Convert.ToInt32(accessibilityFeatureId),
                                AdditionalInfo = accessibilityFeatureMoreInfo,
                                LocationId = Guid.Parse(accessibilityFeatureLocation),
                                Virtual = false
                            };
                        }
                        else
                        {
                            accessibilityToAdd = new ServiceAccessibilityFeature()
                            {
                                ServiceId = serviceToEdit.ServiceId,
                                AccessibilityFeatureId = Convert.ToInt32(accessibilityFeatureId),
                                AdditionalInfo = accessibilityFeatureMoreInfo,
                                Virtual = true
                        };
                        }

                        dc.ServiceAccessibilityFeatures.Add(accessibilityToAdd);
                    }
                }

                var serviceAudit = new ServiceAudit()
                {
                    ServiceAuditId = Guid.NewGuid(),
                    ServiceId = model.ServiceId,
                    UserProfileId = userProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.ServiceAudits.Add(serviceAudit);

                dc.SaveChanges();

                if (serviceToEdit.Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(serviceToEdit.ServiceId);
                }
            }

            return "Accessibility Edited Successfully";
        }

        public List<string> GetSelectedPhysicalLocationAccessibilityFeatures(Guid serviceId, Guid locationId)
        {
            List<string> selectedAccessibility = new List<string>();

            using (ALISSContext dc = new ALISSContext())
            {
                List<AccessibilityFeature> accessibilityFeatures = dc.AccessibilityFeatures.Where(af => af.ParentAccessibilityFeatureId == null).ToList();
                
                foreach (AccessibilityFeature accessibilityFeature in accessibilityFeatures)
                {
                    if (dc.ServiceAccessibilityFeatures.Count(af => af.AccessibilityFeatureId == accessibilityFeature.AccessibilityFeatureId && af.ServiceId == serviceId && af.LocationId == locationId) == 1)
                    {
                        selectedAccessibility.Add(accessibilityFeature.Name);
                    }
                }
            }

            return selectedAccessibility.OrderBy(n => Regex.Replace(n, "[$&+,:;=?@#.\"|'\\-<>.^*()%!]", string.Empty)).ToList();
        }

        public List<string> GetSelectedVirtualLocationAccessibilityFeatures(Guid serviceId)
        {
            List<string> selectedAccessibility = new List<string>();

            using (ALISSContext dc = new ALISSContext())
            {
                List<AccessibilityFeature> accessibilityFeatures = dc.AccessibilityFeatures.Where(af => af.ParentAccessibilityFeatureId == null).ToList();

                foreach (AccessibilityFeature accessibilityFeature in accessibilityFeatures)
                {
                    if (dc.ServiceAccessibilityFeatures.Count(af => af.AccessibilityFeatureId == accessibilityFeature.AccessibilityFeatureId && af.ServiceId == serviceId && af.LocationId == null && af.Virtual) == 1)
                    {
                        selectedAccessibility.Add(accessibilityFeature.Name);
                    }
                }
            }

            return selectedAccessibility.OrderBy(n => Regex.Replace(n, "[$&+,:;=?@#.\"|'\\-<>.^*()%!]", string.Empty)).ToList();
        }

        public List<LocationAccessibilityFeaturePTO> GetServiceAccessibilityFeatureObjects(Guid serviceId, string howServiceAccessed)
        {
            List<ServiceAccessibilityFeature> selectedAccessibility = new List<ServiceAccessibilityFeature>();
            List<ServiceLocation> locations = new List<ServiceLocation>();
            List<LocationAccessibilityFeaturePTO> selectedLocationAccessibility = new List<LocationAccessibilityFeaturePTO>();

            using (ALISSContext dc = new ALISSContext())
            {
                selectedAccessibility = dc.ServiceAccessibilityFeatures.Include(s => s.AccessibilityFeature).Where(s => s.ServiceId == serviceId).ToList();
                locations = dc.ServiceLocations.Where(s => s.ServiceId == serviceId).ToList();

                if (howServiceAccessed.ToLower().Equals("hybrid") || howServiceAccessed.ToLower().Equals("inperson"))
                {
                    foreach (ServiceLocation location in locations)
                    {
                        string title = string.IsNullOrEmpty(location.Location.Name) ? "" : location.Location.Name + ", ";
                        title += location.Location.Address + ", " + location.Location.City + ", " + location.Location.Postcode;
                        selectedLocationAccessibility.Add(new LocationAccessibilityFeaturePTO
                        {
                            Title = title,
                            accessibilityFeatures = selectedAccessibility.Where(l => l.LocationId == location.LocationId).ToList()
                        });
                    }
            }
                if (howServiceAccessed.ToLower().Equals("hybrid") || howServiceAccessed.ToLower().Equals("remote"))
                {
                    selectedLocationAccessibility.Add(new LocationAccessibilityFeaturePTO
                    {
                        Title = "Virtual",
                        accessibilityFeatures = selectedAccessibility.Where(v => v.Virtual).ToList()
                    });
                }
            }

            return selectedLocationAccessibility;
        }

        public AccessibilityViewModel RepopulateAccessibilityModelForError(AccessibilityViewModel model)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                string howServiceAccessed = dc.Services.Where(s => s.ServiceId == model.ServiceId).Select(s => s.HowServiceAccessed).FirstOrDefault() ?? "";
                model.ServiceLocations = GetServiceAccessibilityLocations(model.ServiceId, howServiceAccessed);
            }

            foreach (var location in model.ServiceLocations)
            {
            List<string> selectedAccessibiltyFeatures = String.IsNullOrEmpty(model.SelectedAccessibilityFeatures)
                ? new List<string>()
                : model.SelectedAccessibilityFeatures.Split('¬').ToList();
                List<string> selectedLocationAccessibilityFeatures = new List<string>();
                foreach (var accessibilityFeature in selectedAccessibiltyFeatures)
                {
                    string accessibilityLocationId = System.Text.RegularExpressions.Regex.Split(accessibilityFeature, @"\|\|")[0];
                    if ((accessibilityLocationId.Equals("virtual") && location.Title.Equals("Virtual")) || (location.LocationId.HasValue && location.LocationId.Value.ToString().Equals(accessibilityLocationId)))
                    {
                        selectedLocationAccessibilityFeatures.Add(accessibilityFeature);
                    }
                }

                location.AccessibilityFeatures = _accessibilityFeatureService.GetAccessibilityFeatureListForService(model.ServiceId, selectedLocationAccessibilityFeatures, location.LocationId);

            if (!String.IsNullOrEmpty(model.SelectedAccessibilityFeatures))
            {
                    foreach (var accessibilityFeature in selectedLocationAccessibilityFeatures)
                {
                        string accessibilityFeatureId = System.Text.RegularExpressions.Regex.Split(accessibilityFeature, @"\|\|")[1].Split('|')[0];
                        string additionalInfo = System.Text.RegularExpressions.Regex.Split(accessibilityFeature, @"\|\|")[1].Split('|')[1];
                        location.AccessibilityFeatures.FirstOrDefault(c => c.AccessibilityFeatureId.ToString() == accessibilityFeatureId).Selected = true;
                        location.AccessibilityFeatures.FirstOrDefault(c => c.AccessibilityFeatureId.ToString() == accessibilityFeatureId).AdditionalInfo = additionalInfo;
                    }
                }
            }

            return model;
        }
    }
}

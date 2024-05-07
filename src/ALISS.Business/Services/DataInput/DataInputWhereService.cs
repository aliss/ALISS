using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.DataInput;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Service;
using ALISS.Models.Models;
using System.Data.Entity;
using ALISS.Business.Migrations;

namespace ALISS.Business.Services
{
    public partial class DataInputService
    {
        public WhereViewModel GetWhereToEdit(EditServiceViewModel serviceViewModel)
        {
            return new WhereViewModel()
            {
                OrganisationId = serviceViewModel.OrganisationId,
                ServiceId = serviceViewModel.ServiceId,
				HowServiceAccessed = serviceViewModel.HowServiceAccessed,
                SelectedLocations = serviceViewModel.SelectedLocations,
                Locations = serviceViewModel.Locations,
                SelectedServiceAreas = serviceViewModel.SelectedServiceAreas,
                ServiceServiceAreas = serviceViewModel.ServiceServiceAreas
			};
		}

		public WhereViewModel GetWhereToEdit(Guid serviceId)
		{
			using (ALISSContext dc = new ALISSContext())
			{
				Service service = dc.Services.Find(serviceId);
				return new WhereViewModel()
				{
					ServiceId = service.ServiceId,
					OrganisationId = service.OrganisationId,
					HowServiceAccessed = service.HowServiceAccessed,
					Locations = _locationService.GetLocationListForServices(service.OrganisationId, service.ServiceId),
					ServiceServiceAreas = _serviceAreaService.GetServiceAreaListForServices(service.ServiceId, new List<string>()),
					SelectedLocations = String.Join(",", dc.ServiceLocations.Where(s => s.ServiceId == serviceId).Select(l => l.LocationId).ToList()),
					SelectedServiceAreas = String.Join(",", dc.ServiceServiceAreas.Where(s => s.ServiceId == serviceId).Select(s => s.ServiceAreaId).ToList())
				};
			}
		}

		public WhereViewModel RepopulateWhereModelForError(WhereViewModel model)
		{
			List<string> selectedServiceAreas = String.IsNullOrEmpty(model.SelectedServiceAreas)
				? new List<string>()
				: model.SelectedServiceAreas.Split(',').ToList();

			model.Locations = _locationService.GetLocationListForServices(model.OrganisationId, null);
			model.ServiceServiceAreas = _serviceAreaService.GetServiceAreaListForServices(null, selectedServiceAreas);

			if (!String.IsNullOrEmpty(model.SelectedLocations))
			{
				List<string> serviceLocations = model.SelectedLocations.Split(',').ToList();
				foreach (var serviceLocation in serviceLocations)
				{
					model.Locations.FirstOrDefault(c => c.Value == serviceLocation).Selected = true;
				}
			}

			return model;
		}
        
		public WhereViewModel GetEmptyWhereModel(Guid serviceId)
		{
            Guid organisationId = new Guid();

			using (ALISSContext dc = new ALISSContext())
			{
				Service service = dc.Services.FirstOrDefault(o => o.ServiceId == serviceId);
                
                organisationId = service.OrganisationId;
			}

			WhereViewModel model = new WhereViewModel()
			{
				OrganisationId = organisationId,
				ServiceId = serviceId,
				Locations = _locationService.GetLocationListForServices(organisationId, null),
				ServiceServiceAreas = _serviceAreaService.GetServiceAreaListForServices(null, new List<string>())
			};

			return model;
		}

		public string EditWhere(WhereViewModel model, int userProfileId)
		{
			try
			{
				using (ALISSContext dc = new ALISSContext())
				{
					Service serviceToEdit = dc.Services.Find(model.ServiceId);

					var selectedLocations = dc.ServiceLocations.Where(s => s.ServiceId == serviceToEdit.ServiceId);
					dc.ServiceLocations.RemoveRange(selectedLocations);
					var selectedAreas = dc.ServiceServiceAreas.Where(s => s.ServiceId == serviceToEdit.ServiceId);
					dc.ServiceServiceAreas.RemoveRange(selectedAreas);

                    if (serviceToEdit.LastEditedStep < (int)DataInputStepsEnum.WhereTestStep)
                    {
                        serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.WhereTestStep;
                    }

					serviceToEdit.HowServiceAccessed = model.HowServiceAccessed;

                    List<string> serviceLocations = new List<string>();
                    if (!serviceToEdit.HowServiceAccessed.Equals("remote", StringComparison.OrdinalIgnoreCase) && !String.IsNullOrEmpty(model.SelectedLocations))
					{
                        serviceLocations = model.SelectedLocations.Split(',').ToList();
                        foreach (string serviceLocation in serviceLocations)
						{
							dc.ServiceLocations.Add(new ServiceLocation()
							{
								ServiceId = serviceToEdit.ServiceId,
								LocationId = Guid.Parse(serviceLocation)
							});
						}
					}

                    if (!serviceLocations.Any())
					{
                        var allAccessibilityFeatures = dc.ServiceAccessibilityFeatures.Where(s => s.ServiceId == serviceToEdit.ServiceId && s.LocationId != null);
                        if (allAccessibilityFeatures.Any())
						{
							dc.ServiceAccessibilityFeatures.RemoveRange(allAccessibilityFeatures);
						}
					}
					else
					{
                        var accessibilityFeatures = dc.ServiceAccessibilityFeatures
                            .Where(s =>
                                s.ServiceId == serviceToEdit.ServiceId
                                && !serviceLocations.Contains(s.LocationId.ToString())
                                && s.LocationId != null
                            );
                        if (accessibilityFeatures.Any())
                        {
                            dc.ServiceAccessibilityFeatures.RemoveRange(accessibilityFeatures);
                        }
                    }

					if (!String.IsNullOrEmpty(model.SelectedServiceAreas))
					{
						List<string> serviceServiceAreas = model.SelectedServiceAreas.Split(',').ToList();
						foreach (string serviceServiceArea in serviceServiceAreas)
						{
							dc.ServiceServiceAreas.Add(new ServiceServiceArea()
							{
								ServiceId = serviceToEdit.ServiceId,
								ServiceAreaId = Convert.ToInt32(serviceServiceArea)
							});
						}

						if (serviceToEdit.HowServiceAccessed.Equals("inperson", StringComparison.OrdinalIgnoreCase))
						{
							var accessibilityFeatures = dc.ServiceAccessibilityFeatures.Where(s => s.ServiceId == serviceToEdit.ServiceId && s.LocationId == null);
							if (accessibilityFeatures.Any())
							{
								dc.ServiceAccessibilityFeatures.RemoveRange(accessibilityFeatures);
							}
						}
                    }
					else
					{
                        var accessibilityFeatures = dc.ServiceAccessibilityFeatures.Where(s => s.ServiceId == serviceToEdit.ServiceId && s.LocationId == null);
                        if (accessibilityFeatures.Any())
                        {
                            dc.ServiceAccessibilityFeatures.RemoveRange(accessibilityFeatures);
                        }
                    }

                    dc.ServiceAudits.Add(new ServiceAudit()
					{
						ServiceAuditId = Guid.NewGuid(),
						ServiceId = model.ServiceId,
						UserProfileId = userProfileId,
						DateOfAction = DateTime.UtcNow.Date,
					});

					dc.SaveChanges();

                    if (dc.Organisations.Find(model.OrganisationId).Published)
                    {
                        _elasticSearchService.AddServiceToElasticSearch(serviceToEdit.ServiceId);
                        _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);
                    }
                }
			}
			catch (Exception ex)
			{
				return $"Error: {ex.Message}";
			}

			return "Where Edited Successfully";
		}

        public List<WhereLocationPTO> GetSelectedLocations(Guid serviceId)
        {
            List<WhereLocationPTO> selectedLocations = new List<WhereLocationPTO>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<Location> Locations = dc.ServiceLocations.Where(l => l.ServiceId == serviceId).Select(l => l.Location).ToList();
				foreach (Location location in Locations)
				{
					selectedLocations.Add(new WhereLocationPTO()
					{
						title = location.Name,
						address = location.Address + ", " + location.City + ", " + location.Postcode 
					});
				}
            }
            return selectedLocations;
        }

		public List<string> GetSelectedServiceAreas(Guid serviceId)
		{
            List<ServiceServiceArea> selectedServiceAreas = new List<ServiceServiceArea>();

            using (ALISSContext dc = new ALISSContext())
			{
                List<ServiceServiceArea> countryList = dc.ServiceServiceAreas
					.Include(s => s.ServiceArea)
					.Where(s => s.ServiceId == serviceId && s.ServiceArea.Type == 3)
                    .OrderBy(n => n.ServiceArea.Name)
                    .ToList();
                List<ServiceServiceArea> nonCountryList = dc.ServiceServiceAreas
                    .Include(s => s.ServiceArea)
                    .Where(s => s.ServiceId == serviceId && s.ServiceArea.Type != 3)
                    .OrderBy(n => n.ServiceArea.Name)
                    .ToList();

				selectedServiceAreas = countryList;
                selectedServiceAreas.AddRange(nonCountryList);
            }

			return selectedServiceAreas
				.Select(s => s.ServiceArea.Name)
				.ToList();
        }

		public List<Location> GetServiceLocations(Guid serviceId)
		{
			List<Location> selectedLocations = null;

			using (ALISSContext dc = new ALISSContext())
			{
                selectedLocations = dc.ServiceLocations
					.Include(l => l.Location)
					.Where(s => s.ServiceId == serviceId)
					.Select(l => l.Location)
					.ToList();

            }

			return selectedLocations;
		}

        public List<ServiceArea> GetServiceServiceAreas(Guid serviceId)
        {
            List<ServiceArea> selectedServiceAreas;

            using (ALISSContext dc = new ALISSContext())
            {
                selectedServiceAreas = dc.ServiceServiceAreas
                    .Include(a => a.ServiceArea)
                    .Where(s => s.ServiceId == serviceId)
                    .Select(s => s.ServiceArea)
                    .ToList();
            }

            return selectedServiceAreas;
        }

        public bool ShowAcessibility(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
			{
                Service service = dc.Services.Find(serviceId);
				if (service.HowServiceAccessed?.ToLower() == "inperson")
				{
                    List<ServiceLocation> serviceLocations = dc.ServiceLocations.Where(s => s.ServiceId == serviceId).ToList();

                    return serviceLocations.Any();
				}
			}

			return true;
        }
    }
}

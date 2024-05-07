using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Category;
using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.PresentationTransferObjects.ServiceArea;
using ALISS.Business.ViewModels.ServiceArea;
using ALISS.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tactuum.Core.Extensions;

namespace ALISS.Business.Services
{
    public class ServiceAreaService
    {

        private ElasticSearchService _elasticSearchService;

        public ServiceAreaService()
        {
            _elasticSearchService = new ElasticSearchService();
        }

        public ServiceAreaListingViewModel ListServiceAreas()
        {
            ServiceAreaListingViewModel serviceAreaList = new ServiceAreaListingViewModel()
            {
                ServiceAreas = new List<ServiceAreaPTO>()
            };

            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceArea> serviceAreas = dc.ServiceAreas.ToList();

                foreach (ServiceArea ServiceArea in serviceAreas)

                {
                    ServiceAreaPTO serviceAreaListItem = new ServiceAreaPTO()
                    {

                        ServiceAreaId = ServiceArea.ServiceAreaId,
                        Name = ServiceArea.Name,
                        Code = ServiceArea.Code,
                        Type = ((ServiceAreaTypeEnum)ServiceArea.Type).GetDisplayName(),
                        ServiceAreaCount = dc.ServiceServiceAreas.Count(c => c.ServiceAreaId == ServiceArea.ServiceAreaId)
                    };

                    serviceAreaList.ServiceAreas.Add(serviceAreaListItem);
                }
                
                serviceAreaList.ServiceAreas = serviceAreaList.ServiceAreas.OrderBy(t => t.Type).ThenBy(n => n.Name).ToList();
            }
            return serviceAreaList;
        }


        public string AddServiceArea(EditServiceAreaViewModel model)
        {
            if (DoesServiceAreaExist(model.Name, model.Type))
            {
                return $"Error: The Service area {model.Name} already exists, please choose another name.";
            }

            using (ALISSContext dc = new ALISSContext())
            {
                ServiceArea serviceAreaToAdd = new ServiceArea()
                {
                    Name = model.Name,
                    Slug = model.Slug,
                    Code = model.Code,
                    Type = Convert.ToInt32(model.Type),
                    GeoJson = model.GeoJson
                };

                dc.ServiceAreas.Add(serviceAreaToAdd);
                dc.SaveChanges();
            }

            _elasticSearchService.AddAllServiceAreasToElasticSearch();

            return "Service area added successfully";
        }

        //GET: ServiceArea
        public EditServiceAreaViewModel GetServiceAreaForEdit(int ServiceAreaId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var serviceAreaToEdit = dc.ServiceAreas.Find(ServiceAreaId);

                return new EditServiceAreaViewModel()
                {
                    ServiceAreaId = serviceAreaToEdit.ServiceAreaId,
                    Name = serviceAreaToEdit.Name,
                    Code = serviceAreaToEdit.Code,
                    Slug = serviceAreaToEdit.Slug,
                    Type = (ServiceAreaTypeEnum)Convert.ToInt32(serviceAreaToEdit.Type),
                    ServiceAreaType = GetServiceAreaDropdownList(serviceAreaToEdit.Type),
                    GeoJson= serviceAreaToEdit.GeoJson,
                };
            }
        }

        //POST: ServiceArea
        public string EditServiceArea(EditServiceAreaViewModel model)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                ServiceArea serviceAreaToEdit = dc.ServiceAreas.Find(model.ServiceAreaId);
                if ((model.Name != serviceAreaToEdit.Name || Convert.ToInt32(model.Type) != serviceAreaToEdit.Type) && DoesServiceAreaExist(model.Name, model.Type))
                {
                    return $"Error: The Service Area {model.Name} already exists as a {((ServiceAreaTypeEnum)model.Type).GetDisplayName()}, please choose another name.";
                }

                serviceAreaToEdit.Name = model.Name;
                serviceAreaToEdit.Slug = model.Slug;
                serviceAreaToEdit.Code = model.Code;
                serviceAreaToEdit.Type = Convert.ToInt32(model.Type);
                serviceAreaToEdit.GeoJson = model.GeoJson;

                dc.SaveChanges();
            }

            _elasticSearchService.AddAllServiceAreasToElasticSearch();
            return "Service Area edited successfully";
        }

        public DeleteServiceAreaModel GetServiceAreaForDelete(int serviceAreaId)
        {
            DeleteServiceAreaModel model = new DeleteServiceAreaModel()
            {
                ServiceAreaId = serviceAreaId,
                CanDelete = true,
                RelatedServices = new List<RelatedServicePTO>()
            };

            using (ALISSContext dc = new ALISSContext())
            {
                ServiceArea serviceArea = dc.ServiceAreas.Find(serviceAreaId);
                model.ServiceAreaName = serviceArea.Name;
                var relatedServices = dc.ServiceServiceAreas.Where(c => c.ServiceAreaId == serviceAreaId).Select(s => s.ServiceId).ToList();
                var singleServiceAreaServices = dc.ServiceServiceAreas.GroupBy(s => s.ServiceId).Where(s => s.Count() == 1 && relatedServices.Contains(s.Key)).Select(s => s.FirstOrDefault()).ToList();

                foreach (var service in singleServiceAreaServices)
                {
                    if (service.ServiceAreaId == serviceAreaId && dc.ServiceLocations.Count(s => s.ServiceId == service.ServiceId) == 0)
                    {
                        model.CanDelete = false;
                        Service services = dc.Services.Find(service.ServiceId);
                        model.RelatedServices.Add(new RelatedServicePTO()
                        {
                            ServiceId = service.ServiceId,
                            ServiceName = services.Name,
                            OrganisationId = services.OrganisationId
                        });
                    }
                    else if (service.ServiceAreaId == serviceAreaId)
                    {
                        Service services = dc.Services.Find(service.ServiceId);
                        model.RelatedServices.Add(new RelatedServicePTO()
                        {
                            ServiceId = service.ServiceId,
                            ServiceName = services.Name,
                            OrganisationId = services.OrganisationId
                        });
                    }
                }
            }

            return model;
        }

        public void DeleteServiceArea(int serviceAreaId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var serviceServiceAreas = dc.ServiceServiceAreas.Where(l => l.ServiceAreaId == serviceAreaId);
                if (serviceServiceAreas.Count() > 0)
                {
                    dc.ServiceServiceAreas.RemoveRange(serviceServiceAreas);
                }

                var serviceAreaToRemove = dc.ServiceAreas.Find(serviceAreaId);
                dc.ServiceAreas.Remove(serviceAreaToRemove);
                dc.SaveChanges();
            }

            _elasticSearchService.AddAllServiceAreasToElasticSearch();
        }

        public List<ServiceServiceAreaPTO> GetServiceAreaListForServices(Guid? serviceId, List<string> selectedServiceAreas)
        {
            List<ServiceServiceAreaPTO> serviceAreaDropdown = new List<ServiceServiceAreaPTO>();

            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceArea> serviceAreas = dc.ServiceAreas.Where(p => p.ParentServiceAreaId == null && p.Type == 3).OrderBy(n => n.Name).ToList();
                serviceAreas.AddRange(dc.ServiceAreas.Where(p => p.ParentServiceAreaId == null && p.Type != 3).OrderBy(n => n.Name).ThenBy(t => t.Type).ToList());

                foreach (ServiceArea serviceArea in serviceAreas)
                {
                    ServiceServiceAreaPTO serviceAreaListItem = new ServiceServiceAreaPTO()
                    {
                        ServiceAreaId = serviceArea.ServiceAreaId,
                        Name = $"{serviceArea.Name} ({((ServiceAreaTypeEnum)serviceArea.Type).GetDisplayName()})",
                        Code = serviceArea.Code,
                        Type = ((ServiceAreaTypeEnum)serviceArea.Type).GetDisplayName(),
                        Selected = serviceId.HasValue
                            ? dc.ServiceServiceAreas.Count(s => s.ServiceId == serviceId.Value && s.ServiceAreaId == serviceArea.ServiceAreaId) > 0
                            : selectedServiceAreas.Count() > 0
                                ? selectedServiceAreas.Contains(serviceArea.ServiceAreaId.ToString())
                                : false,
                        NextLevelServiceAreas = new List<ServiceServiceAreaPTO>()
                    };

                    List<ServiceArea> levelTwoServiceAreas = dc.ServiceAreas.Where(p => p.ParentServiceAreaId == serviceArea.ServiceAreaId).OrderBy(n => n.Name).ThenBy(t => t.Type).ToList();

                    foreach (ServiceArea levelTwo in levelTwoServiceAreas)
                    {
                        ServiceServiceAreaPTO levelTwoServiceAreaListItem = new ServiceServiceAreaPTO()
                        {
                            ServiceAreaId = levelTwo.ServiceAreaId,
                            Name = $"{levelTwo.Name} ({((ServiceAreaTypeEnum)levelTwo.Type).GetDisplayName()})",
                            Code = serviceArea.Code,
                            Type = ((ServiceAreaTypeEnum)serviceArea.Type).GetDisplayName(),
                            Selected = serviceId.HasValue
                                ? dc.ServiceServiceAreas.Count(s => s.ServiceAreaId == levelTwo.ServiceAreaId && s.ServiceId == serviceId.Value) == 1
                                : selectedServiceAreas.Count() > 0
                                    ? selectedServiceAreas.Contains(levelTwo.ServiceAreaId.ToString())
                                    : false,
                            NextLevelServiceAreas = new List<ServiceServiceAreaPTO>()
                        };

                        List<ServiceArea> levelThreeServiceAreas = dc.ServiceAreas.Where(p => p.ParentServiceAreaId == levelTwo.ServiceAreaId).OrderBy(n => n.Name).ThenBy(t => t.Type).ToList();

                        foreach (ServiceArea levelThree in levelThreeServiceAreas)
                        {
                            ServiceServiceAreaPTO levelThreeServiceAreaListItem = new ServiceServiceAreaPTO()
                            {
                                ServiceAreaId = levelThree.ServiceAreaId,
                                Name = $"{levelThree.Name} ({((ServiceAreaTypeEnum)levelThree.Type).GetDisplayName()})",
                                Code = serviceArea.Code,
                                Type = ((ServiceAreaTypeEnum)serviceArea.Type).GetDisplayName(),
                                Selected = serviceId.HasValue ? dc.ServiceServiceAreas.Count(s => s.ServiceAreaId == levelTwo.ServiceAreaId && s.ServiceId == serviceId.Value) == 1 : selectedServiceAreas.Count() > 0 ? selectedServiceAreas.Contains(levelTwo.ServiceAreaId.ToString()) : false
                            };
                            levelTwoServiceAreaListItem.NextLevelServiceAreas.Add(levelThreeServiceAreaListItem);
                        }

                        levelTwoServiceAreaListItem.NextLevelServiceAreas = levelTwoServiceAreaListItem.NextLevelServiceAreas.OrderBy(n => n.NextLevelServiceAreas).ToList();
                        serviceAreaListItem.NextLevelServiceAreas.Add(levelTwoServiceAreaListItem);
                    }

                    serviceAreaDropdown.Add(serviceAreaListItem);
                }
            }

            return serviceAreaDropdown;
        }

        public List<SelectListItem> GetServiceAreaDropdownForServices(Guid? serviceId)
        {
            List<SelectListItem> serviceAreaDropdown = new List<SelectListItem>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<ServiceArea> serviceAreas = dc.ServiceAreas.OrderBy(t => t.Type).ThenBy(n => n.Name).ToList();
                foreach (var serviceArea in serviceAreas)
                {
                    SelectListItem serviceAreaToAdd = new SelectListItem()
                    {
                        Text = $"{serviceArea.Name} ({((ServiceAreaTypeEnum)serviceArea.Type).GetDisplayName()})",
                        Value = serviceArea.ServiceAreaId.ToString(),
                        Selected = serviceId.HasValue ? dc.ServiceServiceAreas.Count(s => s.ServiceId == serviceId.Value && s.ServiceAreaId == serviceArea.ServiceAreaId) > 0 : false
                    };
                    serviceAreaDropdown.Add(serviceAreaToAdd);
                }
            }

            return serviceAreaDropdown;
        }

        //Check AreaService
        private bool DoesServiceAreaExist(string ServiceAreaName, ServiceAreaTypeEnum ServiceAreaType)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var serviceType = Convert.ToInt32(ServiceAreaType);

                ServiceArea servicearea = dc.ServiceAreas.Where(n => n.Name.ToLower() == ServiceAreaName.ToLower() && n.Type == serviceType).FirstOrDefault();

                if (servicearea != null)
                {
                    return true;
                }

                return false;
            }
        }

        //DropdownList AreaService Type
        public List<SelectListItem> GetServiceAreaDropdownList(int id = 0)
        {
            List<SelectListItem> serviceAreaTypes = new List<SelectListItem>();
            foreach (var s in Enum.GetValues(typeof(ServiceAreaTypeEnum)))
            {
                var type = new SelectListItem()
                {
                    Value = Convert.ToInt32(s).ToString(),
                    Text = s.GetDisplayName(),
                    Selected = (int)s == id
                };
                serviceAreaTypes.Add(type);
            }
            return serviceAreaTypes.ToList();
        }

    }
}


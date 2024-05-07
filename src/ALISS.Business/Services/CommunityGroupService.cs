using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.CommunityGroup;
using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.ViewModels.CommunityGroup;
using ALISS.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ALISS.Business.Services
{
    public class CommunityGroupService
    {

        private ElasticSearchService _elasticSearchService;

        public CommunityGroupService()
        {
            _elasticSearchService = new ElasticSearchService();
        }

        public CommunityGroupListingViewModel ListCommunityGroups(string searchTerm)
        {
			CommunityGroupListingViewModel communityGroupList = new CommunityGroupListingViewModel()
            {
                SearchTerm = searchTerm,
				CommunityGroups = new List<CommunityGroupPTO>(),
                TotalResults = 0
            };
            
            using (ALISSContext dc = new ALISSContext())
            {
                List<CommunityGroup> levelOneCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == null).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name).ToList();
                foreach (CommunityGroup communityGroup in levelOneCommunityGroups)
                {
                    bool matchesSearch = true;
                    int levelTwoCount = 0;
                    int levelThreeCount = 0;
                    List<Guid> servicesWithLevelOneCommunityGroupSelected = dc.ServiceCommunityGroups.Where(c => c.CommunityGroupId == communityGroup.CommunityGroupId && c.Service.Published && c.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted).Select(c => c.ServiceId).ToList();

                    CommunityGroupPTO communityGroupListItem = new CommunityGroupPTO()
                    {
						CommunityGroupId = communityGroup.CommunityGroupId,
                        Name = communityGroup.Name,
                        DisplayOrder = communityGroup.DisplayOrder,
                        ServiceCount = servicesWithLevelOneCommunityGroupSelected.Count,
						NextLevelCommunityGroups = new List<CommunityGroupPTO>(),
                        IsMinMax = communityGroup.IsMinMax,
                    };
                    List<CommunityGroup> levelTwoCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == communityGroup.CommunityGroupId).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name).ToList();
                    foreach (CommunityGroup levelTwoCat in levelTwoCommunityGroups)
                    {
                        List<ServiceCommunityGroup> levelTwoServices = dc.ServiceCommunityGroups.Where(c => c.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && c.Service.Published && c.CommunityGroupId == levelTwoCat.CommunityGroupId).ToList();
                        List<ServiceCommunityGroup> levelTwoAddToCount = levelTwoServices.Where(c => !servicesWithLevelOneCommunityGroupSelected.Contains(c.ServiceId)).ToList();
                        List<Guid> servicesWithLevelTwoCommunityGroupSelected = levelTwoServices.Select(c => c.ServiceId).ToList();
                        CommunityGroupPTO levelTwoCommunityGroupListItem = new CommunityGroupPTO()
                        {
							CommunityGroupId = levelTwoCat.CommunityGroupId,
                            Name = levelTwoCat.Name,
                            DisplayOrder = levelTwoCat.DisplayOrder,
                            ServiceCount = levelTwoServices.Count,
							NextLevelCommunityGroups = new List<CommunityGroupPTO>()
                        };

                        communityGroupListItem.ServiceCount += levelTwoAddToCount.Count;
                        servicesWithLevelOneCommunityGroupSelected.AddRange(levelTwoAddToCount.Select(c => c.ServiceId));

                        List<CommunityGroup> levelThreeCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == levelTwoCat.CommunityGroupId).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name).ToList();
                        foreach (CommunityGroup levelThreeCat in levelThreeCommunityGroups)
                        {
                            List<ServiceCommunityGroup> levelThreeServices = dc.ServiceCommunityGroups.Where(c => c.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && c.Service.Published && c.CommunityGroupId == levelThreeCat.CommunityGroupId).ToList();

                            List<ServiceCommunityGroup> levelThreeAddToCount = levelThreeServices.Where(c => !servicesWithLevelOneCommunityGroupSelected.Contains(c.ServiceId)).ToList();
                            communityGroupListItem.ServiceCount += levelThreeAddToCount.Count;
                            servicesWithLevelOneCommunityGroupSelected.AddRange(levelThreeAddToCount.Select(c => c.ServiceId));

                            List<ServiceCommunityGroup> levelThreeSubCategoryAddToCount = levelThreeServices.Where(c => !servicesWithLevelTwoCommunityGroupSelected.Contains(c.ServiceId)).ToList();
                            levelTwoCommunityGroupListItem.ServiceCount += levelThreeSubCategoryAddToCount.Count;
                            servicesWithLevelTwoCommunityGroupSelected.AddRange(levelThreeSubCategoryAddToCount.Select(c => c.ServiceId));

                            if (!String.IsNullOrEmpty(searchTerm) && !levelThreeCat.Name.ToLower().Contains(searchTerm.ToLower()))
                            {
                               continue;
                            }
                            CommunityGroupPTO levelThreeCommunityGroupListItem = new CommunityGroupPTO()
                            {
                                CommunityGroupId = levelThreeCat.CommunityGroupId,
                                Name = levelThreeCat.Name,
                                DisplayOrder = levelThreeCat.DisplayOrder,
                                ServiceCount = levelThreeServices.Count,
                            };
                            levelTwoCommunityGroupListItem.NextLevelCommunityGroups.Add(levelThreeCommunityGroupListItem);
                        }
                        levelThreeCount = levelTwoCommunityGroupListItem.NextLevelCommunityGroups.Count;

                        if (!String.IsNullOrEmpty(searchTerm) && !levelTwoCat.Name.ToLower().Contains(searchTerm.ToLower()) && levelThreeCount == 0)
                        {
                            continue;
                        }

                        levelTwoCommunityGroupListItem.NextLevelCommunityGroups = levelTwoCommunityGroupListItem.NextLevelCommunityGroups.OrderBy(n => n.NextLevelCommunityGroups).ToList();
                        communityGroupList.TotalResults += levelThreeCount;
                        communityGroupListItem.NextLevelCommunityGroups.Add(levelTwoCommunityGroupListItem);
                    }
                    levelTwoCount = communityGroupListItem.NextLevelCommunityGroups.Count;

                    communityGroupList.TotalResults += levelTwoCount;
                    communityGroupListItem.NextLevelCommunityGroups = communityGroupListItem.NextLevelCommunityGroups.OrderBy(n => n.Name).ToList();

                    if (!String.IsNullOrEmpty(searchTerm) && !communityGroupListItem.Name.ToLower().Contains(searchTerm.ToLower()))
                    {
                        matchesSearch = false;
                    }

                    if (matchesSearch || (!matchesSearch && (levelTwoCount > 0 || levelThreeCount > 0)))
                    {
                        communityGroupList.CommunityGroups.Add(communityGroupListItem);
                    }
                }
                communityGroupList.TotalResults += communityGroupList.CommunityGroups.Count;
                communityGroupList.CommunityGroups = communityGroupList.CommunityGroups.OrderBy(d => d.DisplayOrder).ThenBy(n => Regex.Replace(n.Name.ToLower(), "[$&+,:;=?@#.\"|'\\-<>.^*()%!]", string.Empty)).ToList();
            }
            
            return communityGroupList;
        }

        public string AddTopLevelCommunityGroup(EditTopLevelCommunityGroupViewModel model)
        {
            if (DoesCommunityGroupExist(model.Name))
            {
                return $"Error: The community group {model.Name} already exists, please choose another name.";
            }

            using (ALISSContext dc = new ALISSContext())
            {
				CommunityGroup communityGroupToAdd = new CommunityGroup()
                {
                    Name = model.Name,
                    Slug = model.Slug,
                    Icon = model.Icon,
                    DisplayOrder = model.DisplayOrder
                };

                dc.CommunityGroups.Add(communityGroupToAdd);

                dc.SaveChanges();
            }

            _elasticSearchService.AddAllCommunityGroupsToElasticSearch();

            return "Community Group added successfully";
        }

        public string AddLowerLevelCommunityGroup(EditLowerLevelCommunityGroupViewModel model)
        {
            if (DoesCommunityGroupExist(model.Name))
            {
                return $"Error: The community group {model.Name} already exists, please choose another name.";
            }

            using (ALISSContext dc = new ALISSContext())
            {
				CommunityGroup communityGroupToAdd = new CommunityGroup()
                {
                    Name = model.Name,
                    Slug = model.Slug,
                    DisplayOrder = model.DisplayOrder,
					ParentCommunityGroupId = model.ParentCommunityGroupId
				};

                dc.CommunityGroups.Add(communityGroupToAdd);
                dc.SaveChanges();
            }

            _elasticSearchService.AddAllCommunityGroupsToElasticSearch();

            return "Community Group added successfully";
        }

        public EditTopLevelCommunityGroupViewModel GetTopLevelCommunityGroupForEdit(int communityGroupId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
				CommunityGroup communityGroupToEdit = dc.CommunityGroups.Find(communityGroupId);
                return new EditTopLevelCommunityGroupViewModel()
                {
                    CommunityGroupId = communityGroupToEdit.CommunityGroupId,
                    Name = communityGroupToEdit.Name,
                    Slug = communityGroupToEdit.Slug,
                    Icon = communityGroupToEdit.Icon,
                    DisplayOrder = communityGroupToEdit.DisplayOrder,
                    IsMinMax = communityGroupToEdit.IsMinMax,
                };
            }
        }

        public EditLowerLevelCommunityGroupViewModel GetLowerLevelCommunityGroupForEdit(int communityGroupId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
				CommunityGroup communityGroupToEdit = dc.CommunityGroups.Find(communityGroupId);
                EditLowerLevelCommunityGroupViewModel model = new EditLowerLevelCommunityGroupViewModel()
                {
                    CommunityGroupId = communityGroupToEdit.CommunityGroupId,
                    Name = communityGroupToEdit.Name,
                    Slug = communityGroupToEdit.Slug,
                    DisplayOrder = communityGroupToEdit.DisplayOrder,
					ParentCommunityGroupId = communityGroupToEdit.ParentCommunityGroupId.Value,
					CommunityGroups = GetCommunityGroupDropdown(communityGroupToEdit.ParentCommunityGroupId.Value, communityGroupId)
                };

                return model;
            }
        }

        public string EditTopLevelCommunityGroup(EditTopLevelCommunityGroupViewModel model)
        {
            bool nameOrSlugChanged = false;

            using (ALISSContext dc = new ALISSContext())
            {
				CommunityGroup communityGroupToEdit = dc.CommunityGroups.Find(model.CommunityGroupId);
                if (model.Name != communityGroupToEdit.Name && DoesCommunityGroupExist(model.Name))
                {
                    return $"Error: The community group {model.Name} already exists, please choose another name.";
                }

                nameOrSlugChanged = !communityGroupToEdit.Name.Equals(model.Name) || !communityGroupToEdit.Slug.Equals(model.Slug);

                communityGroupToEdit.Name = model.Name;
                communityGroupToEdit.Slug = model.Slug;
                communityGroupToEdit.Icon = model.Icon;
                communityGroupToEdit.DisplayOrder = model.DisplayOrder;
                communityGroupToEdit.IsMinMax = model.IsMinMax;

                dc.SaveChanges();
            }

            _elasticSearchService.AddAllCommunityGroupsToElasticSearch();

            if (nameOrSlugChanged)
            {
                _elasticSearchService.ReindexServicesOnCommunityGroupChange(model.CommunityGroupId);
            }

            return "Community Group edited successfully";
        }

        public string EditLowerLevelCommunityGroup(EditLowerLevelCommunityGroupViewModel model)
        {
            bool nameOrSlugChanged = false;

            using (ALISSContext dc = new ALISSContext())
            {
				CommunityGroup communityGroupToEdit = dc.CommunityGroups.Find(model.CommunityGroupId);
                if (model.Name != communityGroupToEdit.Name && DoesCommunityGroupExist(model.Name))
                {
                    return $"Error: The community group {model.Name} already exists, please choose another name.";
                }

                nameOrSlugChanged = !communityGroupToEdit.Name.Equals(model.Name) || !communityGroupToEdit.Slug.Equals(model.Slug);

                communityGroupToEdit.Name = model.Name;
                communityGroupToEdit.Slug = model.Slug;
                communityGroupToEdit.DisplayOrder = model.DisplayOrder;
                communityGroupToEdit.ParentCommunityGroupId = model.ParentCommunityGroupId;

                dc.SaveChanges();
            }

            _elasticSearchService.AddAllCommunityGroupsToElasticSearch();

            if (nameOrSlugChanged)
            {
                _elasticSearchService.ReindexServicesOnCommunityGroupChange(model.CommunityGroupId);
            }

            return "Community Group edited successfully";
        }

        public DeleteCommunityGroupViewModel GetCommunityGroupForDelete(int communityGroupId)
        {
			DeleteCommunityGroupViewModel model = new DeleteCommunityGroupViewModel()
            {
				CommunityGroupId = communityGroupId,
                CanDelete = true,
                RelatedServices = new List<RelatedServicePTO>()
            };

            using (ALISSContext dc = new ALISSContext())
            {
				CommunityGroup communityGroupToDelete = dc.CommunityGroups.Find(communityGroupId);
                model.CommunityGroupName = communityGroupToDelete.Name;

                if (dc.CommunityGroups.Count(p => p.ParentCommunityGroupId == communityGroupId) > 0)
                {
                    model.CanDelete = false;
                    return model;
                }
                else
                {
                    var relatedServices = dc.ServiceCommunityGroups
                        .Include(a => a.Service)
                        .Where(a => a.CommunityGroupId == communityGroupId && a.Service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted)
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

        public void DeleteCommunityGroup(int communityGroupId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var relatedServices = dc.ServiceCommunityGroups.Where(c => c.CommunityGroupId == communityGroupId);
                var serviceList = relatedServices.ToList();
                if (relatedServices.Count() > 0)
                {
                    dc.ServiceCommunityGroups.RemoveRange(relatedServices);
                }

				CommunityGroup communityGroupToDelete = dc.CommunityGroups.Find(communityGroupId);
                dc.CommunityGroups.Remove(communityGroupToDelete);
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

            _elasticSearchService.DeleteCommunityGroup(communityGroupId);
        }

        public List<SelectListItem> GetCommunityGroupDropdown(int parentCommunityGroupId, int currentCommunityGroupId = 0)
        {
            List<SelectListItem> communityGroups = new List<SelectListItem>();

            using (ALISSContext dc = new ALISSContext())
            {
                List<CommunityGroup> levelOneCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == null).ToList();

                foreach (CommunityGroup levelOneCommunityGroup in levelOneCommunityGroups)
                {
                    SelectListItem communityGroupToAdd = new SelectListItem()
                    {
                        Value = levelOneCommunityGroup.CommunityGroupId.ToString(),
                        Text = levelOneCommunityGroup.Name,
                        Selected = levelOneCommunityGroup.CommunityGroupId == parentCommunityGroupId
                    };
                    communityGroups.Add(communityGroupToAdd);

                    List<CommunityGroup> levelTwoCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == levelOneCommunityGroup.CommunityGroupId).ToList();

                    foreach (CommunityGroup levelTwoCommunityGroup in levelTwoCommunityGroups)
                    {
                        communityGroupToAdd = new SelectListItem()
                        {
                            Value = levelTwoCommunityGroup.CommunityGroupId.ToString(),
                            Text = $"-- {levelTwoCommunityGroup.Name}",
                            Selected = levelTwoCommunityGroup.CommunityGroupId == parentCommunityGroupId
                        };

                        if (currentCommunityGroupId == 0 || (currentCommunityGroupId != 0 && levelTwoCommunityGroup.CommunityGroupId != currentCommunityGroupId))
                        {
                            communityGroups.Add(communityGroupToAdd);
                        }
                    }
                }
            }

            return communityGroups;
        }

        public List<ServiceCommunityGroupPTO> GetCommunityGroupListForService(Guid? serviceId, List<string> selectedCommunityGroups)
        {
            List<ServiceCommunityGroupPTO> communityGroupList = new List<ServiceCommunityGroupPTO>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<CommunityGroup> levelOneCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == null).ToList();
                foreach (CommunityGroup communityGroup in levelOneCommunityGroups)
                {
					ServiceCommunityGroupPTO communityGroupListItem = new ServiceCommunityGroupPTO()
                    {
						CommunityGroupId = communityGroup.CommunityGroupId,
                        Name = communityGroup.Name,
                        Icon = communityGroup.Icon,
                        DisplayOrder = communityGroup.DisplayOrder,
                        Selected = serviceId.HasValue 
                                        ? dc.ServiceCommunityGroups.Count(c => c.CommunityGroupId == communityGroup.CommunityGroupId && c.ServiceId == serviceId.Value) == 1 
                                        : selectedCommunityGroups.Count() > 0 
                                            ? selectedCommunityGroups.Contains(communityGroup.CommunityGroupId.ToString()) 
                                            : false,
						IsMinMax = communityGroup.IsMinMax,
                        MinValue = serviceId.HasValue && communityGroup.IsMinMax 
                                        ? dc.ServiceCommunityGroups.FirstOrDefault(c => c.CommunityGroupId == communityGroup.CommunityGroupId && c.ServiceId == serviceId.Value)?.MinValue.ToString() 
                                            ?? "0"
                                        : "0",
                        MaxValue = serviceId.HasValue && communityGroup.IsMinMax
                                        ? dc.ServiceCommunityGroups.FirstOrDefault(c => c.CommunityGroupId == communityGroup.CommunityGroupId && c.ServiceId == serviceId.Value)?.MaxValue.ToString() 
                                            ?? "0"
                                        : "0",

                        NextLevelCommunityGroups = new List<ServiceCommunityGroupPTO>()
                    };

                    List<CommunityGroup> levelTwoCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == communityGroup.CommunityGroupId).ToList();

                    foreach (CommunityGroup levelTwoCat in levelTwoCommunityGroups)
                    {
						ServiceCommunityGroupPTO levelTwoCommunityGroupListItem = new ServiceCommunityGroupPTO()
                        {
                            CommunityGroupId = levelTwoCat.CommunityGroupId,
                            Name = levelTwoCat.Name,
                            Selected = serviceId.HasValue ? dc.ServiceCommunityGroups.Count(c => c.CommunityGroupId == levelTwoCat.CommunityGroupId && c.ServiceId == serviceId.Value) == 1 : selectedCommunityGroups.Count() > 0 ? selectedCommunityGroups.Contains(levelTwoCat.CommunityGroupId.ToString()) : false,
                            DisplayOrder = levelTwoCat.DisplayOrder,
                            NextLevelCommunityGroups = new List<ServiceCommunityGroupPTO>()
                        };
                        
                        List<CommunityGroup> levelThreeCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == levelTwoCat.CommunityGroupId).ToList();
                        
                        foreach (CommunityGroup levelThreeCat in levelThreeCommunityGroups)
                        {
							ServiceCommunityGroupPTO levelThreeCommunityGroupListItem = new ServiceCommunityGroupPTO()
                            {
                                CommunityGroupId = levelThreeCat.CommunityGroupId,
                                Name = levelThreeCat.Name,
                                DisplayOrder = levelThreeCat.DisplayOrder,
                                Selected = serviceId.HasValue ? dc.ServiceCommunityGroups.Count(c => c.CommunityGroupId == levelThreeCat.CommunityGroupId && c.ServiceId == serviceId.Value) == 1 : selectedCommunityGroups.Count() > 0 ? selectedCommunityGroups.Contains(levelThreeCat.CommunityGroupId.ToString()) : false,
                            };
                            levelTwoCommunityGroupListItem.NextLevelCommunityGroups.Add(levelThreeCommunityGroupListItem);
                        }
                        levelTwoCommunityGroupListItem.NextLevelCommunityGroups = levelTwoCommunityGroupListItem.NextLevelCommunityGroups.OrderBy(n => n.NextLevelCommunityGroups).ToList();
                        communityGroupListItem.NextLevelCommunityGroups.Add(levelTwoCommunityGroupListItem);
                    }
                    communityGroupListItem.NextLevelCommunityGroups = communityGroupListItem.NextLevelCommunityGroups.OrderBy(n => n.DisplayOrder).ToList();
                    communityGroupList.Add(communityGroupListItem);
                }
            }
            communityGroupList = communityGroupList.OrderBy(n => n.DisplayOrder).ToList();
            return communityGroupList;
        }

        private bool DoesCommunityGroupExist(string communityGroupName)
        {
            using (ALISSContext dc = new ALISSContext())
            {
				CommunityGroup communityGroup = dc.CommunityGroups.Where(n => n.Name.ToLower() == communityGroupName.ToLower()).FirstOrDefault();

                return communityGroup != null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.CommunityGroup;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;

namespace ALISS.Business.Services
{
    public partial class DataInputService
    {
        public WhoViewModel GetWhoForEdit(Guid serviceId) 
        {
            WhoViewModel model;
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.FirstOrDefault(o => o.ServiceId == serviceId);

                model = new WhoViewModel()
                {
                    OrganisationId = service.OrganisationId,
                    ServiceId = serviceId,
                    ServiceCommunityGroups = _communityGroupService.GetCommunityGroupListForService(serviceId, GetSelectedCommunityGroups(serviceId))
                };
            }

            return model;
        }

        public string EditWho(WhoViewModel model, int userProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(model.ServiceId);

                if (serviceToEdit.LastEditedStep < (int)DataInputStepsEnum.WhoTestStep)
                {
                    serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.WhoTestStep;
                }
                serviceToEdit.UpdatedUserId = userProfileId;
                serviceToEdit.UpdatedOn = DateTime.UtcNow.Date;

                var selectedCommunityGroups = dc.ServiceCommunityGroups.Where(s => s.ServiceId == serviceToEdit.ServiceId);
                dc.ServiceCommunityGroups.RemoveRange(selectedCommunityGroups);

                if (!String.IsNullOrEmpty(model.SelectedCommunityGroups))
                {
                    List<string> serviceCommunityGroups = model.SelectedCommunityGroups.Split(',').ToList();
                    foreach (var serviceCommGroup in serviceCommunityGroups)
                    {
                        int commGroupId;
                        int minValue = 0;
                        int maxValue = 0;

                        if (serviceCommGroup.Split('|').Length == 3)
                        {
                            commGroupId = Convert.ToInt32(serviceCommGroup.Split('|')[0]);
                            minValue = string.IsNullOrEmpty(serviceCommGroup.Split('|')[1]) ? 0 : Convert.ToInt32(serviceCommGroup.Split('|')[1]);
                            maxValue = string.IsNullOrEmpty(serviceCommGroup.Split('|')[2]) ? 0 : Convert.ToInt32(serviceCommGroup.Split('|')[2]);
                        }
                        else
                        {
                            commGroupId = Convert.ToInt32(serviceCommGroup);
                        }

                        var commGroupToAdd = new ServiceCommunityGroup()
                        {
                            ServiceId = serviceToEdit.ServiceId,
                            CommunityGroupId = Convert.ToInt32(commGroupId),
                            MinValue = minValue,
                            MaxValue = maxValue
                        };
                        
                        dc.ServiceCommunityGroups.Add(commGroupToAdd);
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

            return "Service Edited Successfully";
        }

        public List<string> GetSelectedCommunityGroups(Guid serviceId)
        {
            List<string> selectedCommunityGroups = new List<string>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<CommunityGroup> levelOneCommunityGroups = dc.CommunityGroups.Where(cg => cg.ParentCommunityGroupId == null).ToList();
                foreach (CommunityGroup community in levelOneCommunityGroups)
                {
                    if (dc.ServiceCommunityGroups.Count(c => c.CommunityGroupId == community.CommunityGroupId && c.ServiceId == serviceId) == 1)
                    {
                        if (community.IsMinMax)
                        {
                            ServiceCommunityGroup serviceCommunityGroup = dc.ServiceCommunityGroups.FirstOrDefault(scg => scg.CommunityGroupId == community.CommunityGroupId && scg.ServiceId == serviceId);
                            selectedCommunityGroups.Add(community.Name+'|'+serviceCommunityGroup.MinValue+'|'+serviceCommunityGroup.MaxValue);
                        }
                        else
                        {
                            selectedCommunityGroups.Add(community.Name);
                        }
                    }

                    List<CommunityGroup> levelTwoCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == community.CommunityGroupId).ToList();
                    foreach (CommunityGroup levelTwoCommGroup in levelTwoCommunityGroups)
                    {
                        if (dc.ServiceCommunityGroups.Count(c => c.CommunityGroupId == levelTwoCommGroup.CommunityGroupId && c.ServiceId == serviceId) == 1)
                        {
                            selectedCommunityGroups.Add(levelTwoCommGroup.Name);
                        }

                        List<CommunityGroup> levelThreeCommunityGroups = dc.CommunityGroups.Where(p => p.ParentCommunityGroupId == levelTwoCommGroup.CommunityGroupId).ToList();
                        foreach (CommunityGroup levelThreeCommGroup in levelThreeCommunityGroups)
                        {
                            if (dc.ServiceCommunityGroups.Count(c => c.CommunityGroupId == levelThreeCommGroup.CommunityGroupId && c.ServiceId == serviceId) == 1)
                            {
                                selectedCommunityGroups.Add(levelThreeCommGroup.Name);
                            }
                        }
                    }
                }
            }

            return selectedCommunityGroups.OrderBy(n => Regex.Replace(n, "[$&+,:;=?@#.\"|'\\-<>.^*()%!]", string.Empty)).ToList();
        }

        public WhoViewModel RepopulateWhoModelForError(WhoViewModel model)
        {
            List<string> selectedCommunityGroups = String.IsNullOrEmpty(model.SelectedCommunityGroups)
                ? new List<string>()
                : model.SelectedCommunityGroups.Split(',').ToList();

            model.ServiceCommunityGroups = _communityGroupService.GetCommunityGroupListForService(null, selectedCommunityGroups);

            if (!String.IsNullOrEmpty(model.SelectedCommunityGroups))
            {
                List<string> communityGroups = model.SelectedCommunityGroups.Split(',').ToList();
                foreach (var communityGroup in communityGroups)
                {
                    if(communityGroup.Split('|').Length == 3)
                    {
                        model.ServiceCommunityGroups.FirstOrDefault(c => c.CommunityGroupId.ToString() == communityGroup.Split('|')[0]).Selected = true;
                        model.ServiceCommunityGroups.FirstOrDefault(c => c.CommunityGroupId.ToString() == communityGroup.Split('|')[0]).MinValue = communityGroup.Split('|')[1];
                        model.ServiceCommunityGroups.FirstOrDefault(c => c.CommunityGroupId.ToString() == communityGroup.Split('|')[0]).MaxValue = communityGroup.Split('|')[2];
                    }
                }
            }

            return model;
        }
    }
}

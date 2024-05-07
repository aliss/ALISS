using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.DataInput;
using ALISS.Business.ViewModels.Claim;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.Service;
using ALISS.Business.ViewModels.User;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using Tactuum.Core.Util;

namespace ALISS.Business.Services
{
    public partial class DataInputService
    {
        public ServiceViewModel GetEmptyServiceModel(Guid serviceId, bool newService = false)
        {
            string serviceName = null;
            using (var dc = new ALISSContext())
            {
                if (!newService)
                {
                    var service = dc.Services.FirstOrDefault(o => o.ServiceId == serviceId);
                    serviceName = service.Name;
                }
            }

            ServiceViewModel model = new ServiceViewModel()
            {
                ServiceId = serviceId,
                ServiceName = serviceName
            };

            return model;
        }

        public string EditService(EditServiceViewModel model, int userProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(model.ServiceId);
                serviceToEdit.Name = model.Name;
                serviceToEdit.Summary = model.Summary;
                serviceToEdit.Description = model.Description;
                serviceToEdit.Phone = !String.IsNullOrEmpty(model.PhoneNumber) ? model.PhoneNumber.Replace(" ", "") : model.PhoneNumber;
                serviceToEdit.Email = model.Email;
                serviceToEdit.Url = model.Url;
                serviceToEdit.ReferralUrl = model.ReferralUrl;
                serviceToEdit.Facebook = model.Facebook;
                serviceToEdit.Instagram = model.Instagram;
                serviceToEdit.Twitter = model.Twitter;
                serviceToEdit.Slug = String.IsNullOrEmpty(model.Slug) || model.Slug.StartsWith("-") ? SlugGenerator.GenerateSlug(model.Name) : model.Slug;
                serviceToEdit.UpdatedUserId = userProfileId;
                serviceToEdit.UpdatedOn = DateTime.UtcNow.Date;

                if (serviceToEdit.LastEditedStep < (int)DataInputStepsEnum.ServiceTestStep)
                {
                    serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.ServiceTestStep;
                }

                int slugAppend = 0;
                while (_serviceService.DoesServiceSlugExist(serviceToEdit.Slug, serviceToEdit.ServiceId))
                {
                    slugAppend++;
                    serviceToEdit.Slug = $"{model.Slug}-{slugAppend}";
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

                if (dc.Organisations.Find(model.OrganisationId).Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(serviceToEdit.ServiceId);
                    _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);
                }
            }

            return "Service Edited Successfully";
        }

        public string AddService(EditServiceViewModel model, int userProfileId, int currentTestStep = (int)DataInputStepsEnum.ServiceTestStep, DataInputSummaryTypeEnum summaryType = DataInputSummaryTypeEnum.NotSubmitted)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToAdd = new Service()
                {
                    ServiceId = model.ServiceId,
                    Name = model.Name,
                    Summary = model.Summary,
                    Description = model.Description,
                    Phone = !String.IsNullOrEmpty(model.PhoneNumber) ? model.PhoneNumber.Replace(" ", "") : model.PhoneNumber,
                    Email = model.Email,
                    Facebook = model.Facebook,
                    Twitter = model.Twitter,
                    Instagram = model.Instagram,
                    CreatedUserId = userProfileId,
                    CreatedOn = DateTime.UtcNow.Date,
                    OrganisationId = model.OrganisationId,
                    Slug = String.IsNullOrEmpty(model.Slug) || model.Slug.StartsWith("-") ? SlugGenerator.GenerateSlug(model.Name) : model.Slug,
                    Url = model.Url,
                    ReferralUrl = model.ReferralUrl,
                    LastEditedStep = currentTestStep,
                    Suggested = summaryType == DataInputSummaryTypeEnum.SuggestedService,
                };

                int slugAppend = 0;
                while (_serviceService.DoesServiceSlugExist(serviceToAdd.Slug, serviceToAdd.ServiceId))
                {
                    slugAppend++;
                    serviceToAdd.Slug = $"{model.Slug}-{slugAppend}";
                }

                dc.Services.Add(serviceToAdd);

                var serviceAudit = new ServiceAudit()
                {
                    ServiceAuditId = Guid.NewGuid(),
                    ServiceId = serviceToAdd.ServiceId,
                    UserProfileId = userProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.ServiceAudits.Add(serviceAudit);

                dc.SaveChanges();

                if (model.ServiceRepresentative)
                {
                    AddClaimViewModel claimToAdd = new AddClaimViewModel()
                    {
                        ClaimedUserId = userProfileId,
                        Id = serviceToAdd.ServiceId,
                        RepresentativeName = model.ServiceRepresentativeName,
                        RepresentativePhone = model.ServiceRepresentativePhone,
                        RequestLeadClaimant = true,
                        RepresentativeRole = model.ServiceRepresentativeRole
                    };

                    ServiceClaimService.AddClaim(claimToAdd);
                }

                if (dc.Organisations.Find(model.OrganisationId).Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(serviceToAdd.ServiceId);
                    _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);
                }
            }

            return "Service added successfully";
        }
        public ServiceViewModel GetServiceForEdit(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(serviceId);
                Organisation organisation = dc.Organisations.Find(serviceToEdit.OrganisationId);
                ServiceViewModel service = new ServiceViewModel()
                {
                    OrganisationId = serviceToEdit.OrganisationId,
                    ServiceId = serviceToEdit.ServiceId,
                    ServiceName = serviceToEdit.Name,
                    ServiceSummary = serviceToEdit.Summary,
                    ServiceDescription = serviceToEdit.Description,
                    PhoneNumber = serviceToEdit.Phone,
                    Email = serviceToEdit.Email,
                    Url = serviceToEdit.Url,
                    ReferralUrl = serviceToEdit.ReferralUrl,
                    Facebook = serviceToEdit.Facebook,
                    Twitter = serviceToEdit.Twitter,
                    Instagram = serviceToEdit.Instagram,
                    Slug = serviceToEdit.Slug,
                };

                if(!string.IsNullOrEmpty(organisation.Name) && 
                   !string.IsNullOrEmpty(service.ServiceName) && 
                   service.ServiceName.Equals(organisation.Name))
                {
                    service.UseOrganisationName = true;
                }
                if (!string.IsNullOrEmpty(organisation.Description) &&
                    !string.IsNullOrEmpty(service.ServiceDescription) &&
                    service.ServiceDescription.Equals(organisation.Description))
                {
                    service.UseOrganisationDescription = true;
                }
                if (!string.IsNullOrEmpty(organisation.PhoneNumber) && 
                    !string.IsNullOrEmpty(service.PhoneNumber) && 
                    service.PhoneNumber.Equals(organisation.PhoneNumber))
                {
                    service.UseOrganisationPhoneNumber = true;
                }
                if (!string.IsNullOrEmpty(organisation.Email) && 
                    !string.IsNullOrEmpty(service.Email) && 
                    service.Email.Equals(organisation.Email))
                {
                    service.UseOrganisationEmail = true;
                }
                if (!string.IsNullOrEmpty(organisation.Url) && 
                    !string.IsNullOrEmpty(service.Url) && 
                    service.Url.Equals(organisation.Url))
                {
                    service.UseOrganisationUrl = true;
                }
                if (!string.IsNullOrEmpty(organisation.Facebook) && 
                    !string.IsNullOrEmpty(service.Facebook) && 
                    service.Facebook.Equals(organisation.Facebook))
                {
                    service.UseOrganisationFacebook = true;
                }
                if (!string.IsNullOrEmpty(organisation.Twitter) && 
                    !string.IsNullOrEmpty(service.Twitter) && 
                    service.Twitter.Equals(organisation.Twitter))
                {
                    service.UseOrganisationTwitter = true;
                }
                if (!string.IsNullOrEmpty(organisation.Instagram) && 
                    !string.IsNullOrEmpty(service.Instagram) && 
                    service.Instagram.Equals(organisation.Instagram))
                {
                    service.UseOrganisationInstagram = true;
                }

                return service;
            }
        }

        public ServiceViewModel UseOrganisationValues(ServiceViewModel service, OrganisationViewModel organisation)
        {
            if (service.UseOrganisationName)
            {
                service.ServiceName = organisation.OrganisationName;
            }
            if (service.UseOrganisationDescription)
            {
                service.ServiceDescription = organisation.OrganisationDescription;
            }

            if (!service.UseAllOrganisationContactDetails)
            {
                if (service.UseOrganisationPhoneNumber)
                {
                    service.PhoneNumber = organisation.PhoneNumber;
                }
                if (service.UseOrganisationUrl)
                {
                    service.Url = organisation.Url;
                }
                if (service.UseOrganisationEmail)
                {
                    service.Email = organisation.Email;
                }
                if (service.UseOrganisationFacebook)
                {
                    service.Facebook = organisation.Facebook;
                }
                if (service.UseOrganisationTwitter)
                {
                    service.Twitter = organisation.Twitter;
                }
                if (service.UseOrganisationInstagram)
                {
                    service.Instagram = organisation.Instagram;
                }
            }
            else
            {
                service.PhoneNumber = organisation.PhoneNumber;
                service.Url = organisation.Url;
                service.Email = organisation.Email;
                service.Facebook = organisation.Facebook;
                service.Twitter = organisation.Twitter;
                service.Instagram = organisation.Instagram;
            }

            return service;
        }

        public ContinueAddingViewModel getUsersUnfinishedServices(ApplicationUserManager userManager, string username)
        {
            List<ContinueAddingPTO> unfinishedServices = new List<ContinueAddingPTO>();
            unfinishedServices = convertToContinueAddingPTO(_serviceService.GetUserServices(userManager, username, out bool isAdmin));

            if (isAdmin)
            {
                using (ALISSContext dc = new ALISSContext())
                {
                    var userId = dc.UserProfiles.Where(u => u.Username == username).FirstOrDefault().UserProfileId;
                    unfinishedServices = convertToContinueAddingPTO(dc.Services.Where(s => s.ClaimedUserId == null && s.CreatedUserId == userId).Select(s => s.ServiceId).ToList());
                }
            }

            return new ContinueAddingViewModel()
            {
                 UnsubmittedServices = unfinishedServices 
            };
        }

        private List<ContinueAddingPTO> convertToContinueAddingPTO(List<Guid> serviceIds)
        {
            List<ContinueAddingPTO> continueAddingServices = new List<ContinueAddingPTO>();
            Service currentService;
            Organisation currentOrganisation;
            foreach (Guid serviceId in serviceIds)
            {
                using (ALISSContext dc = new ALISSContext())
                {
                    currentService = dc.Services.Find(serviceId);
                    currentOrganisation = dc.Organisations.Find(currentService.OrganisationId);
                }

                if (currentService.LastEditedStep <= (int)DataInputStepsEnum.TotalSteps)
                {
                    continueAddingServices.Add(new ContinueAddingPTO()
                    {
                        ServiceId = serviceId,
                        ServiceName = currentService.Name,
                        OrganisationId = currentOrganisation.OrganisationId,
                        OrganisationName = currentOrganisation.Name,
                        CurrentStep = currentService.LastEditedStep,
                        LastUpdated = currentService.UpdatedOn != null ? (DateTime)currentService.UpdatedOn : (DateTime)currentService.CreatedOn,
                    });
                }
            }

            return continueAddingServices;
        }

        public bool hasServiceBeenToSummary(string id)
        {
            Service currentService;
            using (ALISSContext dc = new ALISSContext())
            {
                currentService = dc.Services.Find(Guid.Parse(id));
            }

            return currentService != null && (currentService.LastEditedStep == (int)DataInputStepsEnum.SummaryTestStep || currentService.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted);
        }

        public bool isServiceSubmitted(Guid id)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(id);
                if(service == null)
                {
                    return false;
                }
                return service.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted;
            }
        }

        public bool IsServiceSuggested(Guid id)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(id);
                return service.Suggested;
            }
        }

        public CancelApplicationViewModel CancelServiceApplication(Guid id, int prevStep)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service currentService = dc.Services.Find(id);
                if(currentService == null)
                {
                    return new CancelApplicationViewModel
                    {
                        PreviousStep = prevStep,
                    };
                }
                return new CancelApplicationViewModel
                {
                    ServiceId = currentService.ServiceId,
                    ServiceName = currentService.Name,
                    OrganisationId = currentService.OrganisationId,
                    PreviousStep = prevStep,
                };
            }
        }
    }
}

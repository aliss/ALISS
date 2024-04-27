using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using ALISS.Business.Enums;
using ALISS.Business.Migrations;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Business.ViewModels.User;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using Nest;

namespace ALISS.Business.Services
{
    public partial class DataInputService
    {
        public void updateServiceStepToSummary(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(serviceId);
                serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.SummaryTestStep;
                dc.SaveChanges();
            }
        }

        public void submitServiceAndOrganisation(Guid serviceId, string username, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Guid> userServices = new List<Guid>();
                List<Guid> userOrganisations = new List<Guid>();
                ApplicationUser user = userManager.FindByName(username);
                UserProfile userProfile = dc.UserProfiles.FirstOrDefault(u => u.Username == username);
                bool isEditor = userManager.IsInRole(user.Id, RolesEnum.Editor.ToString());
                bool isBasic = userManager.IsInRole(user.Id, RolesEnum.BaseUser.ToString()) || userManager.IsInRole(user.Id, RolesEnum.ClaimedUser.ToString());
                bool isAdmin = !isEditor && !isBasic;

                Service service = dc.Services.Find(serviceId);
                Organisation organisation = service.Organisation;
                organisation.Submitted = true;
                service.LastEditedStep = (int)DataInputStepsEnum.DataInputSubmitted;
                organisation.Published = organisation.Published || !isBasic;
                service.Published = organisation.Published || !isBasic;

                if (!isAdmin)
                {
                    if (isBasic)
                    {
                        _emailService.SendNewOrganisationEmail(String.IsNullOrEmpty(userProfile.Name) ? userProfile.Username : userProfile.Name, userProfile.UserProfileId);
                    }

                    _emailService.SendOrganisationAddedEmail(organisation.OrganisationId, userProfile.UserProfileId, userManager);

                    List<MediaGallery> MediaGallery = dc.MediaGallery.Where(s => s.ServiceId == serviceId && !s.Approved).ToList();
                    if (MediaGallery.Count > 0) {
                        foreach (MediaGallery media in MediaGallery)
                        {
                            _emailService.SendMediaForApproval(userProfile.Username, media.UploadUserId.Value, service.OrganisationId, service.Organisation.Name, service.Organisation.Published, service.ServiceId, service.Name, service.Published, media.MediaGalleryId, media.Type, media.MediaUrl, media.AltText, media.Caption);
                        }
                    }
                }

                var serviceReview = dc.ServiceReviews.FirstOrDefault(r => r.ServiceId == serviceId);
                if (serviceReview == null)
                {
                    serviceReview = new ServiceReview
                    {
                        ReviewId = Guid.NewGuid(),
                        ServiceId = serviceId,
                        LastReviewedDate = DateTime.UtcNow.Date,
                        ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed,
                    };
                    dc.ServiceReviews.Add(serviceReview);
                }
                else
                {
                    serviceReview.LastReviewedDate = DateTime.UtcNow.Date;
                }

                dc.SaveChanges();

                _elasticSearchService.AddServiceToElasticSearch(service.ServiceId);
                _elasticSearchService.AddOrganisationToElasticSearch(organisation.OrganisationId);
            }
        }

        public void submitService(Guid serviceId, string username, ApplicationUserManager userManager)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Guid> userServices = new List<Guid>();
                ApplicationUser user = userManager.FindByName(username);
                UserProfile userProfile = dc.UserProfiles.FirstOrDefault(u => u.Username == username);
                bool isEditor = userManager.IsInRole(user.Id, RolesEnum.Editor.ToString());
                bool isBasic = userManager.IsInRole(user.Id, RolesEnum.BaseUser.ToString()) || userManager.IsInRole(user.Id, RolesEnum.ClaimedUser.ToString());
                bool isAdmin = !isEditor && !isBasic;

                Service service = dc.Services.Find(serviceId);
                Organisation organisation = service.Organisation;
                service.LastEditedStep = (int)DataInputStepsEnum.DataInputSubmitted;
                service.Published = organisation.Published || !isBasic;

                if (!isAdmin)
                {
                    List<MediaGallery> MediaGallery = dc.MediaGallery.Where(s => s.ServiceId == serviceId && !s.Approved).ToList();
                    if (MediaGallery.Count > 0)
                    {
                        foreach (MediaGallery media in MediaGallery)
                        {
                            _emailService.SendMediaForApproval(userProfile.Username, media.UploadUserId.Value, service.OrganisationId, service.Organisation.Name, service.Organisation.Published, service.ServiceId, service.Name, service.Published, media.MediaGalleryId, media.Type, media.MediaUrl, media.AltText, media.Caption);
                        }
                    }
                }

                var serviceReview = dc.ServiceReviews.FirstOrDefault(r => r.ServiceId == serviceId);
                if (serviceReview == null)
                {
                    serviceReview = new ServiceReview
                    {
                        ReviewId = Guid.NewGuid(),
                        ServiceId = serviceId,
                        LastReviewedDate = DateTime.UtcNow.Date,
                        ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed,
                    };
                    dc.ServiceReviews.Add(serviceReview);
                }
                else
                {
                    serviceReview.LastReviewedDate = DateTime.UtcNow.Date;
                }

                dc.SaveChanges();

                _elasticSearchService.AddServiceToElasticSearch(service.ServiceId);
                _elasticSearchService.AddOrganisationToElasticSearch(organisation.OrganisationId);
            }
        }

        public void SubmitSuggestedService(Guid serviceId, CurrentUserViewModel CurrentUser)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);
                service.LastEditedStep = (int)DataInputStepsEnum.DataInputSubmitted;
                dc.SaveChanges();

                _emailService.SendSuggestedServiceSubmittedEmail(service.ServiceId, CurrentUser.Email, CurrentUser.Username);
            }
        }

        public void ApproveSuggestedService(Guid serviceId, bool isAdmin = false)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);
                List<MediaGallery> MediaGallery = dc.MediaGallery.Where(m => m.ServiceId == serviceId && !m.Approved).ToList();
                service.Suggested = false;
                service.Published = true;
                dc.SaveChanges();

                _elasticSearchService.AddServiceToElasticSearch(serviceId);
                _elasticSearchService.AddOrganisationToElasticSearch(service.OrganisationId);

                if (MediaGallery.Count > 0)
                {
                    foreach (MediaGallery media in MediaGallery)
                    {
                        if (isAdmin)
                        {
                            ApproveServiceGallery(media.MediaGalleryId);
                        }
                        else
                        {
                            _emailService.SendMediaForApproval(media.UploadUser.Username, media.UploadUserId.Value, service.OrganisationId, service.Organisation.Name, service.Organisation.Published, service.ServiceId, service.Name, service.Published, media.MediaGalleryId, media.Type, media.MediaUrl, media.AltText, media.Caption);
                        }
                    }
                }

                _emailService.SendSuggestedServiceApprovedEmail(service.ServiceId, service.CreatedUser.Email, service.CreatedUser.Username, service.CreatedUser.Name);
            }
        }

        public ConfirmationViewModel GetConfirmationModel(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(serviceId);
                Organisation organisation = dc.Organisations.Find(service.OrganisationId);
                return new ConfirmationViewModel()
                {
                    ServiceId = service.ServiceId,
                    OrganisationId = organisation.OrganisationId,
                    ServiceName = service.Name,
                    OrganisationName = organisation.Name,
                    OrganisationClaimed = organisation.ClaimedUserId.HasValue,
                    Status = service.Published ? "Published" : "Under Review",
                    Suggested = service.Suggested,
                };
            }
        }

        public SummaryViewModel GetOrganisationSummary(Guid id, SummaryViewModel model = null)
        {
            if(model == null)
            {
                model = new SummaryViewModel();
            }

            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.Find(id);

                model.OrganisationId = organisation.OrganisationId;
                model.OrganisationName = organisation.Name;
                model.OrganisationRepresentative = dc.Claims.Where(or => or.OrganisationId == organisation.OrganisationId && or.ClaimedUserId == organisation.CreatedUserId).ToList().Count > 0;
                model.OrganisationDescription = organisation.Description;
                model.OrganisationPhoneNumber = organisation.PhoneNumber;
                model.OrganisationEmail = organisation.Email;
                model.OrganisationUrl = organisation.Url;
                model.OrganisationFacebook = organisation.Facebook;
                model.OrganisationTwitter = organisation.Twitter;
                model.OrganisationInstagram = organisation.Instagram;
                model.OrganisationLogo = organisation.Logo;
            }

            return model;
        }

        public SummaryViewModel GetSummary(Guid id)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(id);
                List<MediaGallery> serviceGallery = dc.MediaGallery.Where(m => m.ServiceId == service.ServiceId).ToList();
                List<MediaGallery> serviceImages = new List<MediaGallery>();
                MediaGallery video = new MediaGallery();

                foreach (MediaGallery media in serviceGallery)
                {
                    if (media.Type.Equals("Video"))
                    {
                        video = media;
                    }
                    else
                    {
                        serviceImages.Add(media);
                    }
                }

                SummaryViewModel model = new SummaryViewModel
                {
                    ServiceSubmitted = isServiceSubmitted(service.ServiceId),

                    ServiceId = service.ServiceId,
                    ServiceName = service.Name,
                    ServiceRepresentative = dc.ServiceClaims.Where(s => s.ServiceId == service.ServiceId && s.ClaimedUserId == service.CreatedUserId).ToList().Count > 0,
                    ServiceSummary = service.Summary,
                    ServiceDescription = service.Description,
                    ServicePhoneNumber = service.Phone,
                    ServiceEmail = service.Email,
                    ServiceUrl = service.Url,
                    ServiceReferalUrl = service.ReferralUrl,
                    ServiceFacebook = service.Facebook,
                    ServiceTwitter = service.Twitter,
                    ServiceInstagram = service.Instagram,

                    SelectedLocations = GetSelectedLocations(service.ServiceId),
                    SelectedServiceAreas = GetSelectedServiceAreas(service.ServiceId),
                    HowServiceAccessed = service.HowServiceAccessed ?? "",
                    ServiceLocations = GetServiceLocations(service.ServiceId),
                    ServiceServiceAreas = GetServiceServiceAreas(service.ServiceId),

                    SelectedCategories = GetSelectedCategories(service.ServiceId),

                    SelectedCommunityGroups = GetSelectedCommunityGroups(service.ServiceId),

                    SelectedAccessibilityFeatureLocationObjects = GetServiceAccessibilityFeatureObjects(service.ServiceId, service.HowServiceAccessed ?? ""),

                    ServiceLogo = service.Logo,
                    ServiceVideo = video.MediaUrl,
                };

                model = GetOrganisationSummary(service.OrganisationId, model);

                foreach (var image in serviceImages)
                {
                    switch (image.GalleryReference)
                    {
                        case 1:
                            model.ServiceImage1 = image.MediaUrl;
                            model.ServiceImage1AltText = image.AltText;
                            model.ServiceImage1Caption = image.Caption;
                            break;
                        case 2:
                            model.ServiceImage2 = image.MediaUrl;
                            model.ServiceImage2AltText = image.AltText;
                            model.ServiceImage2Caption = image.Caption;
                            break;
                        case 3:
                            model.ServiceImage3 = image.MediaUrl;
                            model.ServiceImage3AltText = image.AltText;
                            model.ServiceImage3Caption = image.Caption;
                            break;
                        default:
                            if (model.ServiceImage1 == null)
                            {
                                model.ServiceImage1 = image.MediaUrl;
                                model.ServiceImage1AltText = image.AltText;
                                model.ServiceImage1Caption = image.Caption;
                            }
                            else if (model.ServiceImage2 == null)
                            {
                                model.ServiceImage2 = image.MediaUrl;
                                model.ServiceImage2AltText = image.AltText;
                                model.ServiceImage2Caption = image.Caption;
                            }
                            else if (model.ServiceImage3 == null)
                            {
                                model.ServiceImage3 = image.MediaUrl;
                                model.ServiceImage3AltText = image.AltText;
                                model.ServiceImage3Caption = image.Caption;
                            }
                            break;
                    }
                }

                return model;
            }
        }
    }
}

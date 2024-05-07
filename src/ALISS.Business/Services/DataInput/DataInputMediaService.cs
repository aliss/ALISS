using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALISS.Business.Enums;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;

namespace ALISS.Business.Services
{
    public partial class DataInputService
    {
        public MediaViewModel GetMediaForEdit(Guid serviceId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.FirstOrDefault(o => o.ServiceId == serviceId);
                Organisation organisation = dc.Organisations.FirstOrDefault(o => o.OrganisationId == service.OrganisationId);
                List<MediaGallery> galleryMedia = dc.MediaGallery.Where(m => m.ServiceId == service.ServiceId).ToList();
                List<MediaGallery> galleryImages = new List<MediaGallery>();
                MediaViewModel model = new MediaViewModel()
                {
                    OrganisationId = service.OrganisationId,
                    ServiceId = serviceId,
                    OrganisationLogo = organisation.Logo,
                    ServiceLogo = service.Logo,

                    SummaryType = DataInputSummaryTypeEnum.NotSubmitted
                };

                foreach (MediaGallery media in galleryMedia)
                {
                    if (media.Type.Equals("Video"))
                    {
                        model.ServiceVideo = media.MediaUrl;
                        model.ServiceVideoId = media.MediaGalleryId;
                    }
                    else
                    {
                        galleryImages.Add(media);
                    }
                }

                if (galleryImages.Count > 0 && galleryImages.Count <= 3)
                {
                    foreach (var image in galleryImages)
                    {
                        switch (image.GalleryReference)
                        {
                            case 1:
                                model.ServiceGalleryImage1 = image.MediaUrl;
                                model.ServiceGallery1AltText = image.AltText;
                                model.ServiceGallery1Caption = image.Caption;
                                model.ServiceGalleryImageId1 = image.MediaGalleryId;
                                break;
                            case 2:
                                model.ServiceGalleryImage2 = image.MediaUrl;
                                model.ServiceGallery2AltText = image.AltText;
                                model.ServiceGallery2Caption = image.Caption;
                                model.ServiceGalleryImageId2 = image.MediaGalleryId;
                                break;
                            case 3:
                                model.ServiceGalleryImage3 = image.MediaUrl;
                                model.ServiceGallery3AltText = image.AltText;
                                model.ServiceGallery3Caption = image.Caption;
                                model.ServiceGalleryImageId3 = image.MediaGalleryId;
                                break;
                            default:
                                if (model.ServiceGalleryImage1 == null)
                                {
                                    model.ServiceGalleryImage1 = image.MediaUrl;
                                    model.ServiceGallery1AltText = image.AltText;
                                    model.ServiceGallery1Caption = image.Caption;
                                    model.ServiceGalleryImageId1 = image.MediaGalleryId;
                                }
                                else if (model.ServiceGalleryImage2 == null)
                                {
                                    model.ServiceGalleryImage2 = image.MediaUrl;
                                    model.ServiceGallery2AltText = image.AltText;
                                    model.ServiceGallery2Caption = image.Caption;
                                    model.ServiceGalleryImageId2 = image.MediaGalleryId;
                                }
                                else if (model.ServiceGalleryImage3 == null)
                                {
                                    model.ServiceGalleryImage3 = image.MediaUrl;
                                    model.ServiceGallery3AltText = image.AltText;
                                    model.ServiceGallery3Caption = image.Caption;
                                    model.ServiceGalleryImageId3 = image.MediaGalleryId;
                                }
                                break;
                        }
                    }
                }

                return model;
            }
        }

        public MediaViewModel GetServiceMediaForEdit(Guid id)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.FirstOrDefault(o => o.ServiceId == id);
                List<MediaGallery> galleryMedia = dc.MediaGallery.Where(m => m.ServiceId == service.ServiceId).ToList();
                List<MediaGallery> galleryImages = new List<MediaGallery>();
                MediaViewModel model = new MediaViewModel()
                {
                    OrganisationId = service.OrganisationId,
                    ServiceId = id,
                    ServiceLogo = service.Logo,

                    SummaryType = DataInputSummaryTypeEnum.Service
                };

                foreach (MediaGallery media in galleryMedia)
                {
                    if (media.Type.Equals("Video"))
                    {
                        model.ServiceVideo = media.MediaUrl;
                        model.ServiceVideoId = media.MediaGalleryId;
                    }
                    else
                    {
                        galleryImages.Add(media);
                    }
                }

                if (galleryImages.Count > 0 && galleryImages.Count <= 3)
                {
                    foreach (MediaGallery image in galleryImages)
                    {
                        switch (image.GalleryReference)
                        {
                            case 1:
                                model.ServiceGalleryImage1 = image.MediaUrl;
                                model.ServiceGallery1AltText = image.AltText;
                                model.ServiceGallery1Caption = image.Caption;
                                model.ServiceGalleryImageId1 = image.MediaGalleryId;
                                break;
                            case 2:
                                model.ServiceGalleryImage2 = image.MediaUrl;
                                model.ServiceGallery2AltText = image.AltText;
                                model.ServiceGallery2Caption = image.Caption;
                                model.ServiceGalleryImageId2 = image.MediaGalleryId;
                                break;
                            case 3:
                                model.ServiceGalleryImage3 = image.MediaUrl;
                                model.ServiceGallery3AltText = image.AltText;
                                model.ServiceGallery3Caption = image.Caption;
                                model.ServiceGalleryImageId3 = image.MediaGalleryId;
                                break;
                            default:
                                if (model.ServiceGalleryImage1 == null)
                                {
                                    model.ServiceGalleryImage1 = image.MediaUrl;
                                    model.ServiceGallery1AltText = image.AltText;
                                    model.ServiceGallery1Caption = image.Caption;
                                    model.ServiceGalleryImageId1 = image.MediaGalleryId;
                                }
                                else if (model.ServiceGalleryImage2 == null)
                                {
                                    model.ServiceGalleryImage2 = image.MediaUrl;
                                    model.ServiceGallery2AltText = image.AltText;
                                    model.ServiceGallery2Caption = image.Caption;
                                    model.ServiceGalleryImageId2 = image.MediaGalleryId;
                                }
                                else if (model.ServiceGalleryImage3 == null)
                                {
                                    model.ServiceGalleryImage3 = image.MediaUrl;
                                    model.ServiceGallery3AltText = image.AltText;
                                    model.ServiceGallery3Caption = image.Caption;
                                    model.ServiceGalleryImageId3 = image.MediaGalleryId;
                                }
                                break;
                        }
                    }
                }

                return model;
            }
        }

        public MediaViewModel GetOrganisationMediaForEdit(Guid id)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisation = dc.Organisations.FirstOrDefault(o => o.OrganisationId == id);
                return new MediaViewModel()
                {
                    OrganisationId = id,
                    OrganisationLogo = organisation.Logo,

                    SummaryType = DataInputSummaryTypeEnum.Organisation
                };
            }
        }

        public string EditServiceLogo(MediaViewModel model, int userProfileId, string logoUrl)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(model.ServiceId);

                if (serviceToEdit.LastEditedStep < (int)DataInputStepsEnum.MediaTestStep)
                {
                    serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.MediaTestStep;
                }
                serviceToEdit.UpdatedUserId = userProfileId;
                serviceToEdit.UpdatedOn = DateTime.UtcNow.Date;

                if (string.IsNullOrEmpty(serviceToEdit.Logo))
                {
                    serviceToEdit.LogoAltText = null;
                }
                else
                {
                    serviceToEdit.LogoAltText = serviceToEdit.Name + " Logo";
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
            }

            return "Service Edited Successfully";
        }

        public string EditServiceLogo(Guid serviceId, string logoUrl, int userProfileId)
        {
            using (var dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(serviceId);

                serviceToEdit.UpdatedUserId = userProfileId;
                serviceToEdit.UpdatedOn = DateTime.UtcNow.Date;

                serviceToEdit.Logo = logoUrl;

                var serviceAudit = new ServiceAudit()
                {
                    ServiceAuditId = Guid.NewGuid(),
                    ServiceId = serviceId,
                    UserProfileId = userProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.ServiceAudits.Add(serviceAudit);

                dc.SaveChanges();
            }

            return null;
        }

        public void RemoveOrganisationLogo(Guid organisationId)
        {
            using (var dc = new ALISSContext())
            {
                Organisation organisationToEdit = dc.Organisations.Find(organisationId);

                _blobStorageService.DeleteImageFromBlobStorage(organisationToEdit.Logo);
                organisationToEdit.Logo = null;
                organisationToEdit.LogoAltText = null;

                dc.SaveChanges();
            }
        }

        public void RemoveServiceLogo(Guid serviceId)
        {
            using (var dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(serviceId);

                _blobStorageService.DeleteImageFromBlobStorage(serviceToEdit.Logo);
                serviceToEdit.Logo = null;
                serviceToEdit.LogoAltText = null;

                dc.SaveChanges();
            }
        }

        public void RemoveMediaGallery(Guid mediaGalleryId)
        {
            using (var dc = new ALISSContext())
            {
                MediaGallery mg = dc.MediaGallery.Find(mediaGalleryId);

                _blobStorageService.DeleteImageFromBlobStorage(mg.MediaUrl);
                dc.MediaGallery.Remove(mg);

                dc.SaveChanges();
            }
        }

        public string EditOrganisationLogo(MediaViewModel model, int userProfileId, string logoUrl)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Organisation organisationToEdit = dc.Organisations.Find(model.OrganisationId);

                organisationToEdit.UpdatedUserId = userProfileId;
                organisationToEdit.UpdatedOn = DateTime.UtcNow.Date;

                if (!string.IsNullOrEmpty(organisationToEdit.Logo))
                {
                    organisationToEdit.LogoAltText = organisationToEdit.Name + " Logo";
                }
                else
                {
                    organisationToEdit.LogoAltText = null;
                }

                var organiationAudit = new OrganisationAudit()
                {
                    OrganisationAuditId = Guid.NewGuid(),
                    OrganisationId = model.OrganisationId,
                    UserProfileId = userProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.OrganisationAudits.Add(organiationAudit);

                dc.SaveChanges();
            }

            return "Organisation Edited Successfully";
        }

        public string EditOrganisationLogo(Guid organisationId, string logoUrl, int userProfileId)
        {
            using (var dc = new ALISSContext())
            {
                Organisation organisationToEdit = dc.Organisations.Find(organisationId);

                organisationToEdit.UpdatedUserId = userProfileId;
                organisationToEdit.UpdatedOn = DateTime.UtcNow.Date;

                organisationToEdit.Logo = logoUrl;

                var organiationAudit = new OrganisationAudit()
                {
                    OrganisationAuditId = Guid.NewGuid(),
                    OrganisationId = organisationId,
                    UserProfileId = userProfileId,
                    DateOfAction = DateTime.UtcNow.Date,
                };
                dc.OrganisationAudits.Add(organiationAudit);

                dc.SaveChanges();
            }

            return null;
        }

        public string ApproveServiceGallery(Guid mediaGalleryId)
        {
            try
            {
                using (ALISSContext dc = new ALISSContext())
                {
                    MediaGallery mediaItem = dc.MediaGallery.FirstOrDefault(m => m.MediaGalleryId == mediaGalleryId);
                    Service service = dc.Services.FirstOrDefault(s => s.ServiceId == mediaItem.ServiceId);

                    if (mediaItem != null && !mediaItem.Approved)
                    {
                        mediaItem.Approved = true;

                        dc.SaveChanges();

                        UserProfile userProfile = dc.UserProfiles.Find(mediaItem.UploadUserId);

                        _emailService.SendMediaApprovedEmail(userProfile.Email, userProfile.Name, userProfile.Username, mediaItem.Service.Name, mediaItem.Service.Slug, mediaItem.Type, mediaItem.MediaUrl, mediaItem.AltText, mediaItem.Caption);
                    }

                    if (service.Published)
                    {
                        _elasticSearchService.AddServiceToElasticSearch(service.ServiceId);
                    }
                }

                return "Media item was successfully approved and will now be visible on aliss.org";
            }
            catch (Exception ex)
            {
                return "An error occurred: " + ex.Message;
            }
        }

        public string EditServiceGallery(Guid serviceId, Guid mediaGalleryId, string mediaUrl, int userProfileId, bool isAdmin, int galleryReference)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                MediaGallery gallery = dc.MediaGallery.FirstOrDefault(s => s.MediaGalleryId == mediaGalleryId);
                UserProfile userProfile = dc.UserProfiles.Find(userProfileId);
                Service service = dc.Services.Find(serviceId);

                if (gallery != null)
                {
                    gallery.MediaUrl = mediaUrl;
                    gallery.Submitted = false;
                }
                else
                {
                    gallery = new MediaGallery
                    {
                        MediaGalleryId = mediaGalleryId,
                        ServiceId = serviceId,
                        MediaUrl = mediaUrl,
                        Type = "Image",
                        Approved = isAdmin,
                        UploadUserId = userProfileId,
                        GalleryReference = galleryReference,
                        Submitted = isAdmin,
                    };
                    dc.MediaGallery.Add(gallery);
                }
                dc.SaveChanges();

                return gallery.MediaGalleryId.ToString();
            }
        }

        public string EditServiceGallery(MediaViewModel model, int userProfileId, string mediaUrl, DataInputServiceGalleryEnum galleryImagePosition, bool isAdmin)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(model.ServiceId);

                Guid serviceGalleryId = Guid.Empty;
                bool dbChanged = false;
                string mediaAltText = "";
                string mediaCaption = "";
                bool submitted = false;

                if (serviceToEdit.LastEditedStep < (int)DataInputStepsEnum.MediaTestStep)
                {
                    serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.MediaTestStep;
                }

                switch (galleryImagePosition)
                {
                case DataInputServiceGalleryEnum.Gallery1:
                    serviceGalleryId = model.ServiceGalleryImageId1;
                    mediaAltText = model.ServiceGallery1AltText;
                    mediaCaption = model.ServiceGallery1Caption;
                    break;
                case DataInputServiceGalleryEnum.Gallery2:
                    serviceGalleryId = model.ServiceGalleryImageId2;
                    mediaAltText = model.ServiceGallery2AltText;
                    mediaCaption = model.ServiceGallery2Caption;
                    break;
                case DataInputServiceGalleryEnum.Gallery3:
                    serviceGalleryId = model.ServiceGalleryImageId3;
                    mediaAltText = model.ServiceGallery3AltText;
                    mediaCaption = model.ServiceGallery3Caption;
                    break;
                }

                MediaGallery image = dc.MediaGallery.FirstOrDefault(m => m.MediaGalleryId == serviceGalleryId);

                if (image != null && !string.IsNullOrEmpty(image.MediaUrl) || !string.IsNullOrEmpty(mediaUrl))
                {
                    submitted = image.Submitted;
                    if (mediaAltText != image.AltText || mediaCaption != image.Caption)
                    {
                        image.AltText = mediaAltText;
                        image.Caption = mediaCaption;
                        image.UploadUserId = userProfileId;
                        dbChanged = true;
                    }
                }

                if (dbChanged)
                {
                    var serviceAudit = new ServiceAudit()
                    {
                        ServiceAuditId = Guid.NewGuid(),
                        ServiceId = model.ServiceId,
                        UserProfileId = userProfileId,
                        DateOfAction = DateTime.UtcNow.Date,
                    };

                    dc.ServiceAudits.Add(serviceAudit);

                    if (image != null && !image.Submitted && serviceToEdit.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted)
                    {
                        image.Submitted = true;
                    }

                    dc.SaveChanges();
                }

                if (serviceToEdit.Published)
                {
                    _elasticSearchService.AddServiceToElasticSearch(serviceToEdit.ServiceId);
                }

                if (image != null && !submitted && serviceToEdit.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted)
                {
                    UserProfile userProfile = dc.UserProfiles.Find(userProfileId);
                    if (!isAdmin)
                    {
                        _emailService.SendMediaForApproval(userProfile.Username, userProfileId, serviceToEdit.OrganisationId, serviceToEdit.Organisation.Name, serviceToEdit.Organisation.Published, model.ServiceId, serviceToEdit.Name, serviceToEdit.Published, image.MediaGalleryId, "Image", image.MediaUrl, image.AltText, image.Caption);
                    }
                }
            }

            return "Service Gallery Edited Successfully";
        }

        public string EditServiceVideo(MediaViewModel model, int userProfileId, bool isAdmin)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service serviceToEdit = dc.Services.Find(model.ServiceId);
                MediaGallery video = dc.MediaGallery.FirstOrDefault(m => m.MediaGalleryId == model.ServiceVideoId);

                bool dbChanged = false;

                if (serviceToEdit.LastEditedStep < (int)DataInputStepsEnum.MediaTestStep)
                {
                    serviceToEdit.LastEditedStep = (int)DataInputStepsEnum.MediaTestStep;
                }

                if (!string.IsNullOrEmpty(model.ServiceVideo) && model.ServiceVideoId == Guid.Empty)
                {
                    video = new MediaGallery()
                    {
                        MediaGalleryId = Guid.NewGuid(),
                        ServiceId = serviceToEdit.ServiceId,
                        Service = serviceToEdit,
                        MediaUrl = model.ServiceVideo,
                        Approved = isAdmin,
                        UploadUserId = userProfileId,
                        Type = "Video",
                        Submitted = true,
                    };

                    dc.MediaGallery.Add(video);
                    dbChanged = true;
                }
                else if (!string.IsNullOrEmpty(model.ServiceVideo) && model.ServiceVideoId != Guid.Empty && video.MediaUrl != model.ServiceVideo)
                {
                    dbChanged = true;
                    video.MediaUrl = model.ServiceVideo;
                    video.Approved = isAdmin;
                    video.UploadUserId = userProfileId;
                }
                else if (string.IsNullOrEmpty(model.ServiceVideo))
                {
                    if (video != null)
                    {
                        dc.MediaGallery.Remove(video);
                        dbChanged = true;
                    }
                }

                if (dbChanged)
                {
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

                    if (!string.IsNullOrEmpty(model.ServiceVideo))
                    {
                        UserProfile userProfile = dc.UserProfiles.Find(userProfileId);
                        if (!isAdmin && serviceToEdit.LastEditedStep == (int)DataInputStepsEnum.DataInputSubmitted && model.SummaryType == DataInputSummaryTypeEnum.Service)
                        {
                            _emailService.SendMediaForApproval(userProfile.Username, userProfileId, serviceToEdit.OrganisationId, serviceToEdit.Organisation.Name, serviceToEdit.Organisation.Published, model.ServiceId, serviceToEdit.Name, serviceToEdit.Published, video.MediaGalleryId, "Video", model.ServiceVideo);
                        }
                    }
                }
            }

            return "Service Video Edited Successfully";
        }

        public int GetGalleryCount(Guid id)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                if (id == Guid.Empty || id == null)
                {
                    return 0;
                }
                return dc.MediaGallery.Where(m => m.ServiceId == id && m.Type == "Image").ToList().Count;
            }
        }

        public Guid GetGuidFromBlobUrl(string blobUrl)
        {
            using (var dc = new ALISSContext())
            {
                blobUrl = blobUrl.Split('#').First();

                MediaGallery mg = dc.MediaGallery.FirstOrDefault(u => u.MediaUrl == blobUrl);
                return mg?.MediaGalleryId ?? Guid.NewGuid();
            }
        }

        public MediaViewModel UpdateMediaModel(MediaViewModel model)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Service service = dc.Services.Find(model.ServiceId);
                List<MediaGallery> galleryMedia = dc.MediaGallery.Where(m => m.ServiceId == service.ServiceId).ToList();
                List<MediaGallery> galleryImages = new List<MediaGallery>();

                model.OrganisationLogo = service.Organisation.Logo;
                model.ServiceLogo = service.Logo;

                foreach (MediaGallery media in galleryMedia)
                {
                    if (media.Type.Equals("Video"))
                    {
                        model.ServiceVideo = media.MediaUrl;
                        model.ServiceVideoId = media.MediaGalleryId;
                    }
                    else
                    {
                        galleryImages.Add(media);
                    }
                }

                if (galleryImages.Count > 0 && galleryImages.Count <= 3)
                {
                    foreach (var image in galleryImages)
                    {
                        switch (image.GalleryReference)
                        {
                            case 1:
                                model.ServiceGalleryImage1 = image.MediaUrl;
                                model.ServiceGallery1AltText = image.AltText;
                                model.ServiceGallery1Caption = image.Caption;
                                model.ServiceGalleryImageId1 = image.MediaGalleryId;
                                break;
                            case 2:
                                model.ServiceGalleryImage2 = image.MediaUrl;
                                model.ServiceGallery2AltText = image.AltText;
                                model.ServiceGallery2Caption = image.Caption;
                                model.ServiceGalleryImageId2 = image.MediaGalleryId;
                                break;
                            case 3:
                                model.ServiceGalleryImage3 = image.MediaUrl;
                                model.ServiceGallery3AltText = image.AltText;
                                model.ServiceGallery3Caption = image.Caption;
                                model.ServiceGalleryImageId3 = image.MediaGalleryId;
                                break;
                            default:
                                if (model.ServiceGalleryImage1 == null)
                                {
                                    model.ServiceGalleryImage1 = image.MediaUrl;
                                    model.ServiceGallery1AltText = image.AltText;
                                    model.ServiceGallery1Caption = image.Caption;
                                    model.ServiceGalleryImageId1 = image.MediaGalleryId;
                                }
                                else if (model.ServiceGalleryImage2 == null)
                                {
                                    model.ServiceGalleryImage2 = image.MediaUrl;
                                    model.ServiceGallery2AltText = image.AltText;
                                    model.ServiceGallery2Caption = image.Caption;
                                    model.ServiceGalleryImageId2 = image.MediaGalleryId;
                                }
                                else if (model.ServiceGalleryImage3 == null)
                                {
                                    model.ServiceGalleryImage3 = image.MediaUrl;
                                    model.ServiceGallery3AltText = image.AltText;
                                    model.ServiceGallery3Caption = image.Caption;
                                    model.ServiceGalleryImageId3 = image.MediaGalleryId;
                                }
                                break;
                        }
                    }
                }

                return model;
            }
        }
    }
}

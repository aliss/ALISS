using ALISS.Business.PresentationTransferObjects.Collection;
using ALISS.Business.ViewModels.Collection;
using ALISS.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ALISS.Business.Services
{
    public class CollectionService
    {
        private int _ItemsPerPage = 10;
        private ElasticSearchService _elasticSearchService;
        private ServiceService _serviceService;
        private OrganisationService _organisationService;

        public CollectionService()
        {
            _elasticSearchService = new ElasticSearchService();
            _serviceService = new ServiceService();
            _organisationService = new OrganisationService();
        }

        public CollectionListingViewModel GetCollectionsForUser(int userProfileId, int page = 1)
        {
            List<CollectionListPTO> collectionList = new List<CollectionListPTO>();
            using (ALISSContext dc = new ALISSContext())
            {
                int skip = (page - 1) * _ItemsPerPage;
                List<Collection> collections = dc.Collections.Where(u => u.UserProfileId == userProfileId).OrderBy(c => c.CanDelete).ThenBy(n => n.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                int totalCount = dc.Collections.Count(u => u.UserProfileId == userProfileId);
                if (totalCount == 0)
                {
                    Collection defaultCollection = new Collection()
                    {
                        Name = "My saved services",
                        CanDelete = false,
                        UserProfileId = userProfileId,
                        CollectionId = Guid.NewGuid()
                    };

                    dc.Collections.Add(defaultCollection);
                    dc.SaveChanges();

                    collectionList.Add(new CollectionListPTO()
                    {
                        CanDelete = defaultCollection.CanDelete,
                        Name = defaultCollection.Name,
                        CollectionId = defaultCollection.CollectionId,
                        ServiceCount = 0
                    });
                }
                else
                {
                    foreach (var collection in collections)
                    {
                        collectionList.Add(new CollectionListPTO()
                        {
                            CanDelete = collection.CanDelete,
                            Name = collection.Name,
                            CollectionId = collection.CollectionId,
                            ServiceCount = dc.CollectionServices.Count(c => c.CollectionId == collection.CollectionId)
                        });
                    }
                }
                CollectionListingViewModel model = new CollectionListingViewModel()
                {
                    Collections = collectionList.ToList(),
                    Page = page,
                    TotalCount = totalCount,
                    UserProfileId = userProfileId,
                };

                return model;
            }
        }

        public CollectionViewModel GetCollection(Guid collectionId, int page = 1, ApplicationUserManager userManager = null, string username = "")
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Collection collection = dc.Collections.Find(collectionId);
                CollectionViewModel model = new CollectionViewModel()
                {
                    CollectionId = collectionId,
                    Name = collection.Name,
                };
                List<CollectionServicePTO> collectionServicesToAdd = new List<CollectionServicePTO>();
                int skip = (page - 1) * _ItemsPerPage;
                List<Models.Models.CollectionService> collectionServices = dc.CollectionServices.Include(s => s.Service).Where(c => c.CollectionId == collectionId).OrderBy(n => n.Service.Name).Skip(skip).Take(_ItemsPerPage).ToList();
                int totalCount = dc.CollectionServices.Count(c => c.CollectionId == collectionId);

                foreach (Models.Models.CollectionService collectionService in collectionServices)
                {
                    Service service = dc.Services.Find(collectionService.ServiceId);
                    CollectionServicePTO serviceToAdd = new CollectionServicePTO()
                    {
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        Description = service.Description,
                        Url = service.Url,
                        Phone = service.Phone,
                        Email = service.Email,
                        Twitter = service.Twitter,
                        Facebook = service.Facebook,
                        Instagram = service.Instagram,
                        IsClaimed = service.ClaimedUserId.HasValue,
                        OrganisationId = service.OrganisationId,
                        OrganisationIsClaimed = service.Organisation.ClaimedUserId.HasValue,
                        OrganisationName = service.Organisation.Name,
                        Categories = dc.ServiceCategories.Include(c => c.Category).Where(s => s.ServiceId == service.ServiceId).Select(c => c.Category.Name).ToList(),
                        LastUpdated = service.UpdatedOn.HasValue ? service.UpdatedOn.Value : service.CreatedOn,
                        ServiceAreas = dc.ServiceServiceAreas.Include(l => l.ServiceArea).Where(s => s.ServiceId == service.ServiceId).Select(l => l.ServiceArea.Name).ToList()
                    };

                    if(userManager != null && !string.IsNullOrEmpty(username))
                    {
                        serviceToAdd.CanEdit = _serviceService.CanUserEditService(userManager, username, service.ServiceId);
                        serviceToAdd.CanEditOrganisation = _organisationService.CanUserEditOrganisation(userManager, username, service.OrganisationId);
                    }

                    List<Tuple<string, string>> locations = new List<Tuple<string, string>>();
                    foreach (Location location in dc.ServiceLocations.Include(l => l.Location).Where(s => s.ServiceId == service.ServiceId).Select(l => l.Location).ToList())
                    {
                        string formattedLocation = $"{location.Address}, {location.City}, {location.Postcode}";
                        locations.Add(new Tuple<string, string>(location.Name, formattedLocation));
                    }
                    serviceToAdd.Locations = locations;
                    collectionServicesToAdd.Add(serviceToAdd);
                }
                model.Services = collectionServicesToAdd;
                model.TotalCount = totalCount;

                return model;
            }
        }

        public EmailCollectionViewModel GetCollectionToEmail(PostEmailCollectionViewModel emailModel)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Models.Models.CollectionService> collectionServices = dc.CollectionServices.Include(s => s.Service.Organisation).Where(c => c.CollectionId == emailModel.CollectionId).ToList();
                string sender = dc.Collections.Find(emailModel.CollectionId).UserProfile.Name;
                EmailCollectionViewModel model = new EmailCollectionViewModel()
                {
                    SenderName = String.IsNullOrEmpty(sender) ? "Someone" : sender,
                    RecipientName = emailModel.RecipientName,
                    Email = emailModel.Email
                };

                List<EmailCollectionServicePTO> services = new List<EmailCollectionServicePTO>();
                foreach (Models.Models.CollectionService collectionService in collectionServices)
                {
                    string orgSlug = "";
                    string serviceSlug = "";
                    services.Add(new EmailCollectionServicePTO()
                    {
                        OrganisationLink = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString() + "organisations/" + collectionService.Service.OrganisationId,
                        OrganisationName = collectionService.Service.Organisation.Name,
                        ServiceLink = ConfigurationManager.AppSettings["BasePublicSiteUrl"].ToString() + "services/" + collectionService.ServiceId,
                        ServiceName = collectionService.Service.Name
                    });
                }

                model.Services = services;
                return model;
            }
        }

        public Guid AddNewCollectionForUser(EditCollectionViewModel model)
        {
            if (!DoesCollectionExistForUser(model.Name, model.UserProfileId))
            {
                Collection collectionToAdd = new Collection()
                {
                    CollectionId = Guid.NewGuid(),
                    Name = model.Name,
                    UserProfileId = model.UserProfileId,
                    CanDelete = true
                };

                using (ALISSContext dc = new ALISSContext())
                {
                    dc.Collections.Add(collectionToAdd);
                    dc.SaveChanges();
                }

                return collectionToAdd.CollectionId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid AddServiceToCollection(AddCollectionServiceViewModel model)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Guid collectionId = Guid.Empty;
                if (model.CollectionId == null && !String.IsNullOrEmpty(model.CollectionName))
                {
                    if (!DoesCollectionExistForUser(model.CollectionName, model.UserProfileId))
                    {
                        Collection collectionToAdd = new Collection()
                        {
                            CollectionId = Guid.NewGuid(),
                            Name = model.CollectionName,
                            UserProfileId = model.UserProfileId,
                            CanDelete = true
                        };

                        dc.Collections.Add(collectionToAdd);
                        collectionId = collectionToAdd.CollectionId;
                    }
                    else
                    {
                        return collectionId;
                    }
                }
                else
                {
                    collectionId = model.CollectionId.Value;
                }

                if (dc.CollectionServices.Count(c => c.CollectionId == collectionId && c.ServiceId == model.ServiceId) == 0)
                {
                    Models.Models.CollectionService collectionService = new Models.Models.CollectionService()
                    {
                        ServiceId = model.ServiceId,
                        CollectionId = collectionId
                    };
                    dc.CollectionServices.Add(collectionService);
                }

                dc.SaveChanges();

                return collectionId;
            }
        }

        public void DeleteCollection(Guid collectionId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Models.Models.CollectionService> collectionServices = dc.CollectionServices.Where(s => s.CollectionId == collectionId).ToList();
                dc.CollectionServices.RemoveRange(collectionServices);

                Collection collectionToRemove = dc.Collections.Find(collectionId);
                dc.Collections.Remove(collectionToRemove);

                dc.SaveChanges();
            }
        }

        public void RemoveServiceFromCollection(AddCollectionServiceViewModel model)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Models.Models.CollectionService collectionToRemove = dc.CollectionServices.Where(c => c.CollectionId == model.CollectionId.Value && c.ServiceId == model.ServiceId).FirstOrDefault();

                dc.CollectionServices.Remove(collectionToRemove);
                dc.SaveChanges();
            }
        }

        private bool DoesCollectionExistForUser(string collectionName, int userProfileId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Collection collection = dc.Collections.Where(n => n.Name.ToLower() == collectionName.ToLower() && n.UserProfileId == userProfileId).FirstOrDefault();
                if (collection == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}

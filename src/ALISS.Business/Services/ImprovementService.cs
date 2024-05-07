using ALISS.Business.PresentationTransferObjects.Improvement;
using ALISS.Business.ViewModels.Improvement;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.Services
{
    public class ImprovementService
    {
        private int _ItemsPerPage = 10;
        private ElasticSearchService _elasticSearchService;
        private ServiceService _serviceService;
        private OrganisationService _organisationService;

        public ImprovementService()
        {
            _elasticSearchService = new ElasticSearchService();
            _serviceService = new ServiceService();
            _organisationService = new OrganisationService();
        }

        public ImprovementListingViewModel GetSuggestedImprovements(ApplicationUserManager userManager, string username, int page = 1)
        {
            int skip = (page - 1) * _ItemsPerPage;
            bool isAdmin;
            List<Guid> userServices = _serviceService.GetUserServices(userManager, username, out isAdmin);
            List<Guid> userOrganisations = _organisationService.GetUserOrganisations(userManager, username, out isAdmin);

            using (ALISSContext dc = new ALISSContext())
            {
                List<ImprovementListingPTO> improvementList = new List<ImprovementListingPTO>();
                int totalCount = dc.Improvements.Count();
                List<Improvement> improvements = dc.Improvements.OrderBy(r => r.Resolved).ThenByDescending(c => c.CreatedOn).Skip(skip).Take(_ItemsPerPage).ToList();

                foreach (Improvement improvement in improvements)
                {
                    if ((improvement.OrganisationId.HasValue && dc.Organisations.Find(improvement.OrganisationId) != null) || improvement.OrganisationName != null ||
                        (improvement.ServiceId.HasValue && dc.Services.Find(improvement.ServiceId) != null) || improvement.ServiceName != null)
                    {
                        improvementList.Add(new ImprovementListingPTO()
                        {
                            ImprovementId = improvement.ImprovementId,
                            OrganisationName = improvement.OrganisationId.HasValue ? dc.Organisations.Find(improvement.OrganisationId.Value).Name : improvement.OrganisationName ?? string.Empty,
                            OrganisationId = improvement.OrganisationId ?? null,
                            ServiceName = improvement.ServiceId.HasValue ? dc.Services.Find(improvement.ServiceId.Value).Name : improvement.ServiceName ?? string.Empty,
                            ServiceId = improvement.ServiceId ?? null,
                            Resolved = improvement.Resolved,
                            CreatedOn = improvement.CreatedOn,
                            CanEditService = improvement.ServiceId != null && ((userServices.Count == 0 && isAdmin) || userServices.Contains(improvement.ServiceId.Value) || _serviceService.ValidServiceForEditor(userManager, username, improvement.ServiceId.Value)),
                            CanEditOrganisation = improvement.OrganisationId != null && ((userOrganisations.Count == 0 && isAdmin) || userOrganisations.Contains(improvement.OrganisationId.Value) || _organisationService.ValidOrganisationForEditor(userManager, username, improvement.OrganisationId.Value)),
                        });
                    }
                }

                ImprovementListingViewModel model = new ImprovementListingViewModel()
                {
                    Improvements = improvementList.ToList(),
                    TotalCount = totalCount,
                    Page = page,
                    TotalPages = (int)Math.Ceiling((double)totalCount / _ItemsPerPage)
                };

                return model;
            }
        }

        public void AddSuggestedImprovement(EditImprovementViewModel model)
        {
            Guid? organisationId, serviceId;
            if(model.OrganisationId != Guid.Empty)
            {
                organisationId = model.OrganisationId;
                serviceId = null;
            }
            else
            {
                serviceId = model.ServiceId;
                organisationId = null;
            }

            Improvement improvementToAdd = new Improvement()
            {
                Name = model.Name,
                CreatedOn = DateTime.Now,
                Email = model.Email,
                ServiceId = serviceId,
                ImprovementId = Guid.NewGuid(),
                OrganisationId = organisationId,
                SuggestedImprovement = model.SuggestedImprovement,
                Resolved = false
            };

            using (ALISSContext dc = new ALISSContext())
            {
                dc.Improvements.Add(improvementToAdd);
                dc.SaveChanges();
            }
        }

        public ViewImprovementViewModel GetImprovementForView(Guid improvementId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Improvement improvement = dc.Improvements.Find(improvementId);
                return new ViewImprovementViewModel()
                {
                    ImprovementId = improvementId,
                    Email = improvement.Email,
                    Name = improvement.Name,
                    ServiceId = improvement.ServiceId,
                    ServiceName = improvement.ServiceId.HasValue ? dc.Services.Find(improvement.ServiceId.Value).Name : improvement.ServiceName,
                    OrganisationId = improvement.OrganisationId,
                    OrganisationName = improvement.OrganisationId.HasValue ? dc.Organisations.Find(improvement.OrganisationId.Value).Name : improvement.OrganisationName,
                    SuggestedImprovement = improvement.SuggestedImprovement,
                    Resolved = improvement.Resolved ? "Yes" : "No",
                };
            }
        }

        public void ResolveImprovement(Guid improvementId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Improvement improvement = dc.Improvements.Find(improvementId);
                improvement.Resolved = !improvement.Resolved;
                UserProfile user = dc.UserProfiles.Where(e => e.Email == improvement.Email).FirstOrDefault();
                if (user == null)
                {
                    improvement.Email = "";
                }
                dc.SaveChanges();
            }
        }

        public string GetImprovementRequesterEmail(Guid improvementId)
        {
           using (ALISSContext dc = new ALISSContext())
            {
                return dc.Improvements.Find(improvementId).Email;
            }
        }

        public bool IsOrganisationImprovement(Guid improvementId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                return dc.Improvements.Find(improvementId).OrganisationId != null;
            }
        }

        public string OrganisationImprovementName(Guid improvementId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Improvement improvement = dc.Improvements.Find(improvementId);
                return improvement.OrganisationId.HasValue
                    ? dc.Organisations.Find(improvement.OrganisationId).Name
                    : improvement.OrganisationName;
            }
        }

        public string ServiceImprovementName(Guid improvementId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Improvement improvement = dc.Improvements.Find(improvementId);
                return improvement.ServiceId.HasValue
                    ? dc.Services.Find(improvement.ServiceId).Name
                    : improvement.ServiceName;
            }
        }
    }
}

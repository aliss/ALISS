using ALISS.Business;
using ALISS.Business.Enums;
using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.User;
using ALISS.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace ALISS.Admin.Web.ControllerAPI
{
    [RoutePrefix("api/review")]
    public class ReviewController : ApiController
    {
        private readonly ServiceService _serviceService;
        private readonly OrganisationService _organisationService;
        private readonly ElasticSearchService _elasticSearchService;

        private readonly ApplicationUserManager UserManager;
        private readonly CurrentUserViewModel CurrentUser;

        public ReviewController()
        {
            _serviceService = new ServiceService();
            _organisationService = new OrganisationService();
            _elasticSearchService = new ElasticSearchService();

            UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            System.Security.Principal.IPrincipal user = HttpContext.Current.User;
            UserProfileService upService = new UserProfileService();
            CurrentUser = upService.GetCurentLoggedInUser(user.Identity.Name);
        }

        [HttpGet]
        [Route("FlowTest")]
        public void FlowTest(int emailNumber = 0)
        {
            if (emailNumber > 0)
            {
                _serviceService.ServiceReviewFlowTest(emailNumber);
            }
            else
            {
                for (int i = 1; i <= 3; i++)
                {
                    _serviceService.ServiceReviewFlowTest(i);
                }
            }
        }

        [HttpGet]
        [Route("SendReviewEmails")]
        public void SendReviewEmails(int emailNumber = 0)
        {
            if (emailNumber > 0)
            {
                _serviceService.ProcessServiceReviewEmails(emailNumber);
            }
            else
            {
                for (int i = 1; i <= 3; i++)
                {
                    _serviceService.ProcessServiceReviewEmails(i);
                }
            }
        }

        [HttpGet]
        [Route("Test")]
        public void Test()
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = dc.Services.ToList();
                foreach (Service existingService in services)
                {
                    ServiceReview review = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == existingService.ServiceId);
                    if (review != null)
                    {
                        review.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail3;
                    }
                }
                List<Guid> addedOrganisations = new List<Guid>();

                Guid organisationId = Guid.NewGuid();
                _organisationService.AddOrganisation(new AddOrganisationViewModel
                {
                    OrganisationId = organisationId,
                    Name = "Test Review Email Organisation 1",
                },
                CurrentUser.UserProfileId, UserManager, out organisationId);
                addedOrganisations.Add(organisationId);

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        EditServiceViewModel editService = new EditServiceViewModel
                        {
                            Name = "Test Review Email Service " + i + "." + j,
                            OrganisationId = organisationId,
                        };
                        _serviceService.AddService(editService, CurrentUser.UserProfileId, (int)DataInputStepsEnum.DataInputSubmitted);
                        Service service = dc.Services.FirstOrDefault(s => s.Name == editService.Name);
                        ServiceReview review = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == service.ServiceId);
                        review.LastReviewedDate = DateTime.Today.AddMonths(-10 - i);
                        review.ReviewEmailState = j;
                        service.CreatedUserId = CurrentUser.UserProfileId;
                    }
                }

                dc.SaveChanges();

                SendReviewEmails();

                _organisationService.DeleteOrganisation(organisationId, out string error);

                foreach (Service existingService in services)
                {
                    ServiceReview review = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == existingService.ServiceId);
                    if (review != null)
                    {
                        review.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                    }
                }
                dc.SaveChanges();
            }
        }

        [HttpGet]
        [Route("TestReset")]
        public void TestReset()
        {
            AlterServiceDates(0);
        }

        [HttpGet]
        [Route("TestEmail1")]
        public void TestEmail1()
        {
            AlterServiceDates(1);
        }

        [HttpGet]
        [Route("TestEmail2")]
        public void TestEmail2()
        {
            AlterServiceDates(2);
        }

        [HttpGet]
        [Route("TestEmail3")]
        public void TestEmail3()
        {
            AlterServiceDates(3);
        }

        [HttpGet]
        [Route("TestSetup")]
        public void TestSetup()
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = dc.Services.ToList();
                foreach (Service existingService in services)
                {
                    ServiceReview review = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == existingService.ServiceId);
                    review.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail3;
                }
                dc.SaveChanges();
            }
        }

        private void AlterServiceDates(int email)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                List<Service> services = dc.Services.ToList();
                foreach (Service existingService in services)
                {
                    ServiceReview review = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == existingService.ServiceId);
                    review.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail3;
                }

                Guid guid1 = Guid.Parse("ac9df28c-86fa-48e8-95af-e38e042222e8");
                ServiceReview claimantService = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == guid1);
                Guid guid2 = Guid.Parse("2e063de5-128c-465f-b726-deb5d142554f");
                ServiceReview noClaimantService = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == guid2);
                Guid guid3 = Guid.Parse("3ff5cc00-62a2-4006-bb8f-5c45ddc6f99a");
                ServiceReview claimantAndManagersService = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == guid3);

                switch (email)
                {
                case 0:
                    claimantService.LastReviewedDate = DateTime.Today;
                    noClaimantService.LastReviewedDate = DateTime.Today;
                    claimantAndManagersService.LastReviewedDate = DateTime.Today;
                    claimantService.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                    noClaimantService.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                    claimantAndManagersService.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                    break;
                case 1:
                    claimantService.LastReviewedDate = DateTime.Today.AddMonths(-10);
                    noClaimantService.LastReviewedDate = DateTime.Today.AddMonths(-10);
                    claimantAndManagersService.LastReviewedDate = DateTime.Today.AddMonths(-10);
                    claimantService.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                    noClaimantService.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                    claimantAndManagersService.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                    break;
                case 2:
                    claimantService.LastReviewedDate = DateTime.Today.AddMonths(-11);
                    noClaimantService.LastReviewedDate = DateTime.Today.AddMonths(-11);
                    claimantAndManagersService.LastReviewedDate = DateTime.Today.AddMonths(-11);
                    claimantService.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail1;
                    noClaimantService.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail1;
                    claimantAndManagersService.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail1;
                    break;
                case 3:
                    claimantService.LastReviewedDate = DateTime.Today.AddMonths(-12);
                    noClaimantService.LastReviewedDate = DateTime.Today.AddMonths(-12);
                    claimantAndManagersService.LastReviewedDate = DateTime.Today.AddMonths(-12);
                    claimantService.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail2;
                    noClaimantService.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail2;
                    claimantAndManagersService.ReviewEmailState = (int)ReviewEmailEnum.ReceviedEmail2;
                    break;
                }

                dc.SaveChanges();

                if (email != 0)
                {
                    SendReviewEmails();
                }

                foreach (Service existingService in services)
                {
                    if (email == 0 || (existingService.ServiceId != guid1 && existingService.ServiceId != guid2 && existingService.ServiceId != guid3))
                    {
                        ServiceReview review = dc.ServiceReviews.FirstOrDefault(s => s.ServiceId == existingService.ServiceId);
                        review.ReviewEmailState = (int)ReviewEmailEnum.RecentlyReviewed;
                    }
                }
                dc.SaveChanges();
            }
        }

        [HttpGet]
        [Route("TestToggleDeprioritised")]
        public void TestToggleDeprioritised(Guid? id)
        {
            if (id.HasValue)
            {
                using (ALISSContext dc = new ALISSContext())
                {
                    Service service = dc.Services.Find(id);
                    service.Deprioritised = !service.Deprioritised;
                    dc.SaveChanges();

                    if (service.Published)
                    {
                        _elasticSearchService.AddServiceToElasticSearch(id.Value);
                    }
                }
            }
        }
    }
}

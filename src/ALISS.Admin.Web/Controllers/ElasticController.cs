using ALISS.Business.Services;
using ALISS.Business.ViewModels.Organisation;
using ALISS.Business.ViewModels.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize(Roles = "ALISSAdmin")]
    public class ElasticController : ALISSBaseController
    {
        private readonly ServiceService _serviceService;
        private readonly OrganisationService _organisationService;
        private readonly ElasticSearchService _elasticSearchService;

        public ElasticController()
        {
            _serviceService = new ServiceService();
            _organisationService = new OrganisationService();
            _elasticSearchService = new ElasticSearchService();
        }

        // GET: Elastic
        public ActionResult Index()
        {
            ServiceListingViewModel serviceModel = _serviceService.GetAllServices(UserManager, CurrentUser.Username, "", 1);
            OrganisationListingViewModel organisationModel = _organisationService.GetOrganisations(UserManager, CurrentUser.Username, "", 1);
            ViewBag.TotalServices = serviceModel.TotalResults;
            ViewBag.TotalOrganisations = organisationModel.TotalResults;
            return View();
        }

        public ActionResult PopulateOrganisationIndex()
        {
            if (Request.QueryString["from"] != null && Request.QueryString["to"] != null)
            {
                _elasticSearchService.AddOrganisationsToElasticSearch(int.Parse(Request.QueryString["from"]), int.Parse(Request.QueryString["to"]));
            }
            else
            {
                _elasticSearchService.AddAllOrganisationsToElasticSearch();
            }
            return RedirectToAction("Index");
        }

        public ActionResult PopulateServiceIndex()
        {
            if (Request.QueryString["from"] != null && Request.QueryString["to"] != null)
            {
                _elasticSearchService.AddServicesToElasticSearch(int.Parse(Request.QueryString["from"]), int.Parse(Request.QueryString["to"]));
            }
            else
            {
                _elasticSearchService.AddAllServicesToElasticSearch();
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteOrganisationIndex()
        {
            _elasticSearchService.DeleteOrganisationIndex();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteServiceIndex()
        {
            _elasticSearchService.DeleteServiceIndex();
            return RedirectToAction("Index");
        }
    }
}
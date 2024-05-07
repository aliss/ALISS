using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALISS.Business.Enums;
using ALISS.Business;
using ALISS.Business.Services;
using ALISS.Business.ViewModels.DataInput;
using ALISS.Models.Models;
using Microsoft.AspNet.Identity;

namespace ALISS.Admin.Web.Controllers.DataInputControllers
{
    public class DataInputOrganisationSummaryController : DataInputBaseController
    {
        // GET: DataInputOrganisationSummary
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue && !_organisationService.CanUserEditOrganisation(UserManager, CurrentUser.Username, id.Value))
            {
                return RedirectToAction("Index", "AddToALISS");
            }

            TempData.Clear();

            SummaryViewModel model;
            if (id.HasValue && _organisationService.DoesOrganisationIdExist(id.Value) && _dataInputService.isOrganisationSubmitted(id.Value))
            {
                ViewBag.CurrentStep = DataInputStepsEnum.SummaryTestStep;
                ViewBag.Submitted = _dataInputService.isOrganisationSubmitted(id.Value);
                ViewBag.SummaryType = DataInputSummaryTypeEnum.Organisation;
                ViewBag.Published = _organisationService.IsOrganisationPublished(id.Value);
                ViewBag.OrganisationId = id.Value;
                ViewBag.OrganisationSubmitted = _dataInputService.isOrganisationSubmitted(id.Value);


                using (ALISSContext dc = new ALISSContext())
                {
                    Organisation organisation = dc.Organisations.Find(id);

                    ViewBag.OrganisationId = organisation.OrganisationId;
                    ViewBag.IsLeadClaimant = organisation.ClaimedUser != null && organisation.ClaimedUserId == CurrentUser.UserProfileId;
                    ViewBag.IsClaimant = dc.OrganisationClaimUsers.Where(x => x.OrganisationId == id && x.ClaimedUserId == CurrentUser.UserProfileId).Any();

                    model = _dataInputService.GetOrganisationSummary(id.Value);
                    model.SummaryType = DataInputSummaryTypeEnum.Organisation;
                }

                TempData["newOrganisation"] = _dataInputService.GetOrganisationForEdit(model.OrganisationId);
                TempData["newServiceOrgId"] = model.OrganisationId;

                model.SummaryType = DataInputSummaryTypeEnum.Organisation;

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "AddToALISS");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SummaryViewModel model, string submit)
        {
            TempData.Remove("newOrganisation");
            TempData.Remove("newServiceOrgId");

            if (submit.Equals("Save"))
            {
                return RedirectToAction("Index", "Organisation");
            }
            else if (submit.Equals("Add Service"))
            {
                TempData["newOrganisation"] = _dataInputService.GetOrganisationForEdit(model.OrganisationId);
                TempData["newServiceOrgId"] = _dataInputService.GetOrganisationForEdit(model.OrganisationId).OrganisationId;
                return RedirectToAction("Index", "DataInputService", new { id = Guid.Empty });
            }

            return RedirectToAction("Index", "AddToALISS");
        }
    }
}
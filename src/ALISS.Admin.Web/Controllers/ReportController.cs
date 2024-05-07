using ALISS.Business.Services;
using ALISS.Business.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALISS.Admin.Web.Controllers
{
    [Authorize(Roles = "ALISSAdmin")]
    public class ReportController : ALISSBaseController
    {
        private readonly ReportingService _reportingService;

        public ReportController()
        {
            _reportingService = new ReportingService();
        }

        // GET: Report
        public FileResult GetLocationReport()
        {
            var reportBytes = _reportingService.GetLocationReport();

            return File(reportBytes, "application/excel", "LocationReport.xlsx");
        }

        public ActionResult ActivityReport()
        {
            ActivityReportViewModel model = _reportingService.GetActivityReport(DateTime.Today.AddMonths(-1), DateTime.Today, false);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivityReport(ActivityReportViewModel model)
        {
            DateTime from = new DateTime(model.YearFrom, model.MonthFrom, model.DayFrom);
            DateTime to = new DateTime(model.YearTo, model.MonthTo, model.DayTo);
            ActivityReportViewModel returnModel = _reportingService.GetActivityReport(from, to, model.IncludeUnpublished);

            return View(returnModel);
        }
    }
}
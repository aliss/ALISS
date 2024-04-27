using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ALISS.Business.Enums;
using ALISS.Models.Models;
using ALISS.Business.ViewModels.Report;

namespace ALISS.Business.Services
{
    public class ReportingService
    {
        private readonly CategoryService _categoryService;

        public ReportingService()
        {
            _categoryService = new CategoryService();
        }


        public byte[] GetLocationReport()
        {
            ExcelPackage pck = new ExcelPackage();
            
            ExcelWorksheet coverSheet = pck.Workbook.Worksheets.Add("Location Report");

            using (ALISSContext dc = new ALISSContext())
            {
                var localAuthorities = dc.ServiceAreas.Where(s => s.Type == (int)ServiceAreaTypeEnum.LocalAuthority).ToList();
                var categories = _categoryService.ListCategories("").Categories;

                int coverRow = 3;
                foreach (var localAuthority in localAuthorities)
                {
                    List<Guid> serviceIds = dc.ServiceServiceAreas.Where(s => s.ServiceAreaId == localAuthority.ServiceAreaId).Select(s => s.ServiceId).ToList();
                    coverSheet.Cells[coverRow, 1].Value = localAuthority.Name;
                    coverSheet.Cells[coverRow, 2].Value = serviceIds.Count();

                    ExcelWorksheet localAuthoritySheet = pck.Workbook.Worksheets.Add(localAuthority.Name);
                    int localRow = 3;
                    foreach (var category in categories)
                    {
                        localAuthoritySheet.Cells[localRow, 1].Value = category.Name;
                        localAuthoritySheet.Cells[localRow, 2].Value = dc.ServiceCategories.Count(c => c.CategoryId == category.CategoryId && serviceIds.Contains(c.ServiceId));
                        localRow++;

                        foreach (var levelTwo in category.NextLevelCategories)
                        {
                            localAuthoritySheet.Cells[localRow, 1].Value = levelTwo.Name;
                            localAuthoritySheet.Cells[localRow, 2].Value = dc.ServiceCategories.Count(c => c.CategoryId == levelTwo.CategoryId && serviceIds.Contains(c.ServiceId));
                            localRow++;

                            foreach (var levelThree in levelTwo.NextLevelCategories)
                            {
                                localAuthoritySheet.Cells[localRow, 1].Value = levelThree.Name;
                                localAuthoritySheet.Cells[localRow, 2].Value = dc.ServiceCategories.Count(c => c.CategoryId == levelThree.CategoryId && serviceIds.Contains(c.ServiceId));
                                localRow++;
                            }
                        }

                        localRow++;
                    }

                    coverRow++;
                }
            }

            return pck.GetAsByteArray();
        }

        public ActivityReportViewModel GetActivityReport(DateTime from, DateTime to, bool includeUnpublished)
        {
            ActivityReportViewModel model = new ActivityReportViewModel()
            {
                DayFrom = from.Day,
                MonthFrom = from.Month,
                YearFrom = from.Year,
                DayTo = to.Day,
                MonthTo = to.Month,
                YearTo = to.Year,
                IncludeUnpublished = includeUnpublished
            };

            using (ALISSContext dc = new ALISSContext())
            {
                model.NumberOfServicesAdded = includeUnpublished ? dc.Services.Count(c => c.CreatedOn >= from && c.CreatedOn <= to) : dc.Services.Include(o => o.Organisation).Count(c => c.CreatedOn >= from && c.CreatedOn <= to && c.Organisation.Published);
                model.NumberOfServicesEdited = includeUnpublished ? dc.ServiceAudits.Count(d => d.DateOfAction >= from && d.DateOfAction <= to) - model.NumberOfServicesAdded : dc.ServiceAudits.Include(o => o.Service.Organisation).Count(d => d.DateOfAction >= from && d.DateOfAction <= to && d.Service.Organisation.Published) - model.NumberOfServicesAdded;
                model.NumberOfOrganisationsAdded = includeUnpublished ? dc.Organisations.Count(c => c.CreatedOn >= from && c.CreatedOn <= to) : dc.Organisations.Count(c => c.CreatedOn >= from && c.CreatedOn <= to && c.Published);
                model.NumberOfOrganisationsEdited = includeUnpublished ? dc.OrganisationAudits.Count(d => d.DateOfAction >= from && d.DateOfAction <= to) - model.NumberOfOrganisationsAdded : dc.OrganisationAudits.Include(o => o.Organisation).Count(d => d.DateOfAction >= from && d.DateOfAction <= to && d.Organisation.Published) - model.NumberOfOrganisationsAdded;
                model.NumberOfNewUsers = dc.UserProfiles.Count(d => d.DateJoined >= from && d.DateJoined <= to);
                model.NumberOfUsersLoggedIn = dc.UserProfiles.Count(l => l.LastLogin >= from && l.LastLogin <= to);
                model.TotalNumberOfOrganisationClaimsMade = dc.Claims.Count(c => c.CreatedOn >= from && c.CreatedOn <= to);
                model.TotalNumberOfServiceClaimsMade = dc.ServiceClaims.Count(c => c.CreatedOn >= from && c.CreatedOn <= to);
                model.NumberOfApprovedOrganisationClaims = dc.Claims.Count(c => c.CreatedOn >= from && c.CreatedOn <= to && c.Status == 10);
                model.NumberOfApprovedServiceClaims = dc.ServiceClaims.Count(c => c.CreatedOn >= from && c.CreatedOn <= to && c.Status == 10);
            }

            return model;
        }

        public byte[] GetActivityReport(ActivityReportViewModel model)
        {
            ExcelPackage pck = new ExcelPackage();

            ExcelWorksheet workSheet = pck.Workbook.Worksheets.Add("Activity Report");

            //workSheet.Cells[1, 1].Value = $"Activity Report from {model.DateFrom.ToString("dd/MM/yyyy")} to {model.DateTo.ToString("dd/MM/yyyy")}";

            return pck.GetAsByteArray();
        }
    }
}

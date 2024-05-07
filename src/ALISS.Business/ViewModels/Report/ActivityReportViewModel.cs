using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Report
{
    public class ActivityReportViewModel
    {
        public int DayFrom { get; set; }
        public int MonthFrom { get; set; }
        public int YearFrom { get; set; }
        public int DayTo { get; set; }
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
        public bool IncludeUnpublished { get; set; }
        public int NumberOfServicesAdded { get; set; }
        public int NumberOfServicesEdited { get; set; }
        public int NumberOfOrganisationsAdded { get; set; }
        public int NumberOfOrganisationsEdited { get; set; }
        public int NumberOfNewUsers { get; set; }
        public int NumberOfUsersLoggedIn { get; set; }
        public int TotalNumberOfOrganisationClaimsMade { get; set; }
        public int TotalNumberOfServiceClaimsMade { get; set; }
        public int NumberOfApprovedOrganisationClaims { get; set; }
        public int NumberOfApprovedServiceClaims { get; set; }
        public int NumberOfSuggestedImprovements { get; set; }
    }
}

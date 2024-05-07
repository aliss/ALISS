using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ALISS.Business.Enums
{
    public enum DataInputStepsEnum
    {
        [Display(Name = "Total Steps")]
        TotalSteps = 8,
        [Display(Name = "Submitted")]
        DataInputSubmitted = 100,

        [Display(Name = "Organisation Step")]
        OrganisationTestStep = 1,
        [Display(Name = "Service Step")]
        ServiceTestStep = 2,
        [Display(Name = "Where Step")]
        WhereTestStep = 3,
        [Display(Name = "Categories Step")]
        CategoriesTestStep = 4,
        [Display(Name = "Who Step")]
        WhoTestStep = 5,
        [Display(Name = "Accessibility Step")]
        AccessibilityTestStep = 6,
        [Display(Name = "Media Step")]
        MediaTestStep = 7,
        [Display(Name = "Summary Step")]
        SummaryTestStep = 8
    }
}

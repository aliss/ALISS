using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.Enums
{
    public enum DataInputSummaryTypeEnum
    {
        [Display(Name = "Data Input Creation")]
        NotSubmitted = 1,
        [Display(Name = "Service Summary")]
        Service = 2,
        [Display(Name = "Organisation Summary")]
        Organisation = 3,
        [Display(Name = "Service Review")]
        ServiceReview = 4,
        [Display(Name = "Suggested Service")]
        SuggestedService = 5,
    }
}

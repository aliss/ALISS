using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.Enums
{
    public enum ClaimStatusEnum
    {
        [Display(Name = "Un-reviewed")]
        Unreviewed = 0,
        [Display(Name = "Approved")]
        Approved = 10,
        [Display(Name = "Denied")]
        Denied = 20,
        [Display(Name = "Revoked")]
        Revoked = 30
    }
}

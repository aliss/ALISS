using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.Enums
{
    public enum RolesEnum
    {
        [Display(Name = "Super Admin")]
        SuperAdmin = 1,
        [Display(Name = "ALISS Admin")]
        ALISSAdmin = 2,
        [Display(Name = "Editor")]
        Editor = 3,
        [Display(Name = "Claimed User")]
        ClaimedUser = 4,
        [Display(Name = "Basic User")]
        BaseUser = 5,
    }
}

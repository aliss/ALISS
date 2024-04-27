using System.ComponentModel.DataAnnotations;

namespace ALISS.Business.Enums
{
    public enum ServiceAreaTypeEnum
    {
        [Display(Name = "Health Board")]
        HealthBoard = 1,
        [Display(Name = "Local Authority")]
        LocalAuthority = 2,
        [Display(Name = "Country")]
        Country = 3,
        [Display(Name = "Ward")]
        Ward = 4
    }
}

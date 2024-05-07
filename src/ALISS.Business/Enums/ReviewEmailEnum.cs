using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ALISS.Business.Enums
{
    public enum ReviewEmailEnum
    {
        [Display(Name = "Recently Reviewed")]
        RecentlyReviewed = 0,
        [Display(Name = "Receieved Initial Email")]
        ReceviedEmail1 = 1,
        [Display(Name = "Receieved Reminder Email")]
        ReceviedEmail2 = 2,
        [Display(Name = "Receieved Urgent Email")]
        ReceviedEmail3 = 3,
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Models.Models
{
    public class ServiceReview
    {
        [Key]
        public Guid ReviewId { get; set; }
        public Guid ServiceId { get; set; }
        public DateTime? LastReviewedDate { get; set; }
        public int? LastReviewedUserId { get; set; }
        public int? ReviewEmailState { get; set; }
        public DateTime? ReviewEmailDate { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("LastReviewedUserId")]
        public virtual UserProfile LastReviewedUser { get; set; }
    }
}

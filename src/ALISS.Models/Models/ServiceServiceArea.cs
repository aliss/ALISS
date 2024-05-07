using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceServiceArea
    {
        [Key, Column(Order = 0)]
        public Guid ServiceId { get; set; }
        [Key, Column(Order = 1)]
        public int ServiceAreaId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("ServiceAreaId")]
        public virtual ServiceArea ServiceArea { get; set; }
    }
}

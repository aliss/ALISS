using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceLocation
    {
        [Key, Column(Order = 0)]
        public Guid ServiceId { get; set; }
        [Key, Column(Order = 1)]
        public Guid LocationId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
    }
}

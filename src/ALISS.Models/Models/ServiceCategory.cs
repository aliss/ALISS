using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceCategory
    {
        [Key, Column(Order = 0)]
        public Guid ServiceId { get; set; }
        [Key, Column(Order = 1)]
        public int CategoryId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}

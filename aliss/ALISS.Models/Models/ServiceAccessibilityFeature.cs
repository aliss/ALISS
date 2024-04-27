using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceAccessibilityFeature
	{
        [Key]
        public int ServiceAccessibilityFeatureId { get; set; }
        [Column(Order = 0)]
        public Guid ServiceId { get; set; }
        [Column(Order = 1)]
        public int AccessibilityFeatureId { get; set; }
        [MaxLength(500)]
        public string AdditionalInfo { get; set; }
        public Guid? LocationId { get; set; }
        public bool Virtual { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("AccessibilityFeatureId")]
        public virtual AccessibilityFeature AccessibilityFeature { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; } 
    }
}

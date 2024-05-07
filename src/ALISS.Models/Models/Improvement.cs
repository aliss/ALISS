using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Models.Models
{
    public class Improvement
    {
        [Key]
        public Guid ImprovementId { get; set; }
        public Guid? ServiceId { get; set; }
        [MaxLength(100)]
        public string ServiceName { get; set; }
        public Guid? OrganisationId { get; set; }
        [MaxLength(100)]
        public string OrganisationName { get; set; }
        [Required]
        [MaxLength(1024)]
        public string SuggestedImprovement { get; set; }
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Resolved { get; set; }
    }
}

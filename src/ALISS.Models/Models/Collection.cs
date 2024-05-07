using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Models.Models
{
    public class Collection
    {
        [Key]
        public Guid CollectionId { get; set; }
        public int UserProfileId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public bool CanDelete { get; set; }

        [ForeignKey("UserProfileId")]
        public virtual UserProfile UserProfile { get; set; }
    }
}

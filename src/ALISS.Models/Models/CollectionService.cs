using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Models.Models
{
    public class CollectionService
    {
        [Key, Column(Order = 0)]
        public Guid CollectionId { get; set; }
        [Key, Column(Order = 1)]
        public Guid ServiceId { get; set; }

        [ForeignKey("CollectionId")]
        public virtual Collection Collection { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
    }
}

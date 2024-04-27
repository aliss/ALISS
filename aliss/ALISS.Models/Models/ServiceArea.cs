using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceArea
    {
        [Key]
        public int ServiceAreaId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Slug { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }

        public int Type { get; set; }

        public string GeoJson { get; set; }

        public int? ParentServiceAreaId { get; set; }
    }
}

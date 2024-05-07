using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class CommunityGroup
    {
        [Key]
        public int CommunityGroupId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Slug { get; set; }
        [MaxLength(50)]
        public string Icon { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentCommunityGroupId { get; set; }
        public bool IsMinMax { get; set; }

        [ForeignKey("ParentCommunityGroupId")]
        public virtual Category ParentCommunityGroup { get; set; }
    }
}

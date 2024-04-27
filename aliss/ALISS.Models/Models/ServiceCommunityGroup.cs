using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class ServiceCommunityGroup
	{
        [Key, Column(Order = 0)]
        public Guid ServiceId { get; set; }
        [Key, Column(Order = 1)]
        public int CommunityGroupId { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("CommunityGroupId")]
        public virtual CommunityGroup CommunityGroup { get; set; }
    }
}

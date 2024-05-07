using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class MediaGallery
    {
        [Key]
        public Guid MediaGalleryId { get; set; }

        public Guid ServiceId { get; set; }

        public string Caption { get; set; }

        public string AltText { get; set; }

        public string MediaUrl { get; set; }

        public bool Submitted { get; set; }

        public bool Approved { get; set; }

        public int? ApprovedByUserId { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public int? UploadUserId { get; set; }

        public DateTime? UploadDate { get; set; }

        public string Type { get; set; }

        public int GalleryReference { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        [ForeignKey("ApprovedByUserId")]
        public virtual UserProfile ApprovedByUser { get; set; }
        [ForeignKey("UploadUserId")]
        public virtual UserProfile UploadUser { get; set; }
    }
}

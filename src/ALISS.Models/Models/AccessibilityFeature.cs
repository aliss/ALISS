using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALISS.Models.Models
{
    public class AccessibilityFeature
    {
        [Key]
        public int AccessibilityFeatureId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Slug { get; set; }
        public string PromptQuestions { get; set; }
        [MaxLength(50)]
        public string Icon { get; set; }
        public int DisplayOrder { get; set; }
        public int? ParentAccessibilityFeatureId { get; set; }
        public bool Virtual { get; set; }
        public bool Physical { get; set; }

        [ForeignKey("ParentAccessibilityFeatureId")]
        public virtual Category ParentAccessibilityFeature { get; set; }
    }
}

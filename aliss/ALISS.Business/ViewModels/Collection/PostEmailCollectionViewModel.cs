using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Collection
{
    public class PostEmailCollectionViewModel
    {
        public Guid CollectionId { get; set; }
        [Display(Name = "Recipient Name")]
        [Required]
        public string RecipientName { get; set; }
        [Required]
        public string Email { get; set; }
    }
}

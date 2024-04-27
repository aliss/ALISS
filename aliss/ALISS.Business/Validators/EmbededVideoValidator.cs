using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.Validators
{
    public class EmbededVideoValidator : ValidationAttribute
    {
        private List<string> _videoProviderPrefixs;

        public EmbededVideoValidator()
        {
            _videoProviderPrefixs = new List<string>
            {
                "https://youtube.com/watch?v=",
                "https://youtu.be/",
                "https://vimeo.com/",

                "https://www.youtube.com/watch?v=",
                "https://www.youtu.be/",
                "https://www.vimeo.com/",
            };
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string videoUrl = value as string;
            if (videoUrl == null) return ValidationResult.Success;
            foreach (string videoProvider in _videoProviderPrefixs)
            {
                if (videoUrl.ToLower().Contains(videoProvider))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("This is not a valid Vimeo or You Tube link");
        }
    }
}

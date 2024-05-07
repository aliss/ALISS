using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.Validators
{
    public class FileTypeValidator : ValidationAttribute
    {
        private List<string> _fileTypes;
        public FileTypeValidator() 
        {
            _fileTypes = new List<string>
            {
                ".png",
                ".jpg",
                ".jpeg"
            };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string filename = value as string;
            if (filename == null) return ValidationResult.Success;
            foreach (string fileType in _fileTypes)
            {
                if (filename.ToLower().EndsWith(fileType))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("The selected image must be one of the specified file type (.png, .jpeg or .jpg)");
        }
    }
}

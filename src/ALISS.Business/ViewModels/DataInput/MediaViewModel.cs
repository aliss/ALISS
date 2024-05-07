using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages;
using Tactuum.Core.Attributes;
using ALISS.Business.Validators;
using System.ComponentModel;
using ALISS.Business.Enums;

namespace ALISS.Business.ViewModels.DataInput
{
    public class MediaViewModel
    {
        public Guid ServiceId { get; set; }
        public Guid OrganisationId { get; set; }

        [Display(Description = "An image file for the logo of the organisation you would like to add.")]
        [FileTypeValidator]
        public string OrganisationLogo { get; set; }

        [Display(Description = "An image file for the logo of the service you would like to add.")]
        [FileTypeValidator]
        public string ServiceLogo { get; set; }

        public Guid ServiceGalleryImageId1 { get; set; }
        [Display(Description = "An image file for the gallery of the service.")]
        [FileTypeValidator]
        public string ServiceGalleryImage1 { get; set; }
        [MaxLength(140, ErrorMessage = "The alt text field must be a maximum of 140 characters long.")]
        [Display(Name = "Gallery Image 1 Alt Text", Description = "Please provide alt text for your image. This text should describe the image and will be what screen readers share or what appears if the image is unable to load. By providing alt text you will help ensure that your information remains accessible.")]
        public string ServiceGallery1AltText { get; set; }
        [Display(Name = "Gallery Image 1 Caption (optional)")]
        [MaxLength(200, ErrorMessage = "The caption field must be a maximum of 200 characters long.")]
        public string ServiceGallery1Caption { get; set; }

        public Guid ServiceGalleryImageId2 { get; set; }
        [Display(Description = "An image file for the gallery of the service.")]
        [FileTypeValidator]
        public string ServiceGalleryImage2 { get; set; }
        [MaxLength(140, ErrorMessage = "The alt text field must be a maximum of 140 characters long.")]
        [Display(Name = "Gallery Image 2 Alt Text", Description = "Please provide alt text for your image. This text should describe the image and will be what screen readers share or what appears if the image is unable to load. By providing alt text you will help ensure that your information remains accessible.")]
        public string ServiceGallery2AltText { get; set; }
        [Display(Name = "Gallery Image 2 Caption (optional)")]
        [MaxLength(200, ErrorMessage = "The caption field must be a maximum of 200 characters long.")]
        public string ServiceGallery2Caption { get; set; }

        public Guid ServiceGalleryImageId3 { get; set; }
        [Display(Description = "An image file for the gallery of the service.")]
        [FileTypeValidator]
        public string ServiceGalleryImage3 { get; set; }
        [MaxLength(140, ErrorMessage = "The alt text field must be a maximum of 140 characters long.")]
        [Display(Name = "Gallery Image 3 Alt Text", Description = "Please provide alt text for your image. This text should describe the image and will be what screen readers share or what appears if the image is unable to load. By providing alt text you will help ensure that your information remains accessible.")]
        public string ServiceGallery3AltText { get; set; }
        [Display(Name = "Gallery Image 3 Caption (optional)")]
        [MaxLength(200, ErrorMessage = "The caption field must be a maximum of 200 characters long.")]
        public string ServiceGallery3Caption { get; set; }

        public Guid ServiceVideoId { get; set; }
        [EmbededVideoValidator]
        [Url]
        [Display(Name = "Service Embedded Video (optional)", Description = "Please supply a Youtube or Vimeo web url for the video you would like to be embedded in your Service Gallery.<br><br>Please ensure the video has subtitles or a voiceover or alt text added before you add the video URL to ALISS. This helps with making the video accessible to everyone.<br>Additionally, note that by adding to ALISS, ALISS Users will be able to see more video content from the YouTube or Vimeo Channel you uploaded it from.")]
        public string ServiceVideo { get; set; }

        public DataInputSummaryTypeEnum SummaryType { get; set; }
    }
}

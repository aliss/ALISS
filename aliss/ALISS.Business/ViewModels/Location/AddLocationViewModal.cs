using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tactuum.Core.Attributes;

namespace ALISS.Business.ViewModels.Location
{
    class AddLocationViewModel
    {
        [Key]
        public Guid LocationId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Address { get; set; }
        [MaxLength(30)]
        public string City { get; set; }
        [MaxLength(10)]
        public string Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid OrganisationId { get; set; }

    }
}

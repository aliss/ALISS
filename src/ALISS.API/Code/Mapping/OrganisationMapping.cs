using ALISS.API.Models.API;
using ALISS.API.Models.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ALISS.API.Code.Mapping
{
    public class OrganisationMapping
    {
        static Regex htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public OrganisationModel MapOrganisationToOutput(OrganisationElasticSearchModel source)
        {
            string flatDescription = source.description != null ? source.description.Trim() : "";
            if (flatDescription.StartsWith("<p>") && flatDescription.EndsWith("</p>"))
            {
                flatDescription = flatDescription.Substring(3, flatDescription.Length - 7);
            }
            flatDescription = flatDescription
                .Replace("<p>", Environment.NewLine)
                .Replace("</p>", Environment.NewLine)
                .Replace("<br>", Environment.NewLine)
                .Replace("<br/>", Environment.NewLine)
                .Replace("<br />", Environment.NewLine)
                .Replace("<ul>", Environment.NewLine)
                .Replace("</ul>", Environment.NewLine)
                .Replace("</li><li>", Environment.NewLine);
            flatDescription = htmlRegex.Replace(flatDescription, string.Empty);

            OrganisationModel model = new OrganisationModel()
            {
				alissUrl = new Uri(ConfigurationManager.AppSettings["BaseSiteUrl"].ToString() + "/organisations/" + source.slug),
				description = flatDescription,
                description_formatted = source.description,
				email = source.email,
				facebook = String.IsNullOrEmpty(source.facebook) ? null : new Uri(source.facebook),
                instagram = String.IsNullOrEmpty(source.instagram) ? null : new Uri(source.instagram),
                twitter = String.IsNullOrEmpty(source.twitter) ? null : new Uri(source.twitter),
                id = source.id,
				is_claimed = source.is_claimed,
				last_edited = source.last_edited,
				name = source.name,
				permalink = new Uri(ConfigurationManager.AppSettings["BaseSiteUrl"].ToString() + "/organisations/" + source.id),
				phone = source.phone,
                slug = source.slug,
                url = String.IsNullOrEmpty(source.url) ? null : new Uri(source.url),
            };
            
            List<Location> locations = new List<Location>();
            foreach (LocationModel location in source.locations)
            {
                locations.Add(new Location()
                {
                    formatted_address = location.formatted_address,
                    street_address = location.street_address,
                    country = location.country,
                    description = location.description,
                    id = location.id,
                    latitude = location.point.Latitude,
                    longitude = location.point.Longitude,
                    locality = location.locality,
                    name = location.name,
                    postal_code = location.postal_code,
                    region = location.region,
                    state = location.state
                });
            }
            model.locations = locations;

            List<Service> services = new List<Service>();
            foreach (OrganisationServiceModel service in source.services)
            {
                List<ServiceArea> servicesAreas = new List<ServiceArea>();
                foreach (ServiceAreaElasticSearchModel serviceArea in service.service_areas)
                {
                    servicesAreas.Add(new ServiceArea()
                    {
                        code = serviceArea.code,
                        name = serviceArea.name,
                        type = serviceArea.type
                    });
                }

                List<Category> categories = new List<Category>();
                foreach (ServiceCategoryModel category in service.categories)
                {
                    categories.Add(new Category()
                    {
                        slug = category.slug,
                        name = category.name
                    });
                }

                services.Add(new Service()
                {
                    description = service.description,
                    summary = service.summary,
                    email = service.email,
                    id = service.id,
                    phone = service.phone,
                    name = service.name,
                    is_claimed = service.is_claimed,
                    slug = service.slug,
                    url = String.IsNullOrEmpty(service.url) ? null : new Uri(service.url),
                    referral_url = String.IsNullOrEmpty(service.referral_url) ? null : new Uri(service.referral_url),
                    service_areas = servicesAreas,
                    categories = categories,
                });
            }

            model.services = services;

            return model;
        }
    }
}
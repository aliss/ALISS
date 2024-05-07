using ALISS.API.Models.API;
using ALISS.API.Models.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ALISS.API.Code.Mapping
{
    public class ServiceMapping
    {
        static Regex htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public ServiceModel MapServiceToOutput(ServiceElasticSearchModel source)
        {
            string flatDescription = source.description.Trim();
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

            ServiceModel model = new ServiceModel()
            {
                aliss_url = new Uri(ConfigurationManager.AppSettings["BaseSiteUrl"].ToString() + "/services/" + source.slug),
                description = flatDescription,
                description_formatted = source.description,
                email = source.email,
                facebook = source.facebook,
                twitter = source.twitter,
                instagram = source.instagram,
                id = source.id,
                last_updated = source.last_edited,
                last_reviewed = source.last_reviewed,
                is_claimed = source.is_claimed,
                is_deprioritised = source.is_deprioritised,
                name = source.name,
                summary = source.summary,
                permalink = new Uri(ConfigurationManager.AppSettings["BaseSiteUrl"].ToString() + "/services/" + source.id),
                phone = source.phone,
                slug = source.slug,
                url = String.IsNullOrEmpty(source.url) ? null : new Uri(source.url),
                referral_url = String.IsNullOrEmpty(source.referral_url) ? null : new Uri(source.referral_url),
                location_score = source.location_score,
                organisation = new ServiceOrganisation()
                {
                    slug = source.organisation.slug,
                    id = source.organisation.id,
                    is_claimed = source.organisation.is_claimed,
                    name = source.organisation.name,
                    aliss_url = new Uri(ConfigurationManager.AppSettings["BaseSiteUrl"].ToString() + "/organisations/" + source.organisation.slug),
                    permalink = new Uri(ConfigurationManager.AppSettings["BaseSiteUrl"].ToString() + "/organisations/" + source.organisation.id)
                }
            };

            List<Category> categories = new List<Category>();
            foreach (ServiceCategoryModel category in source.categories)
            {
                categories.Add(new Category()
                {
                    name = category.name,
                    slug = category.slug,
                    selected = category.selected,
                });
            }
            model.categories = categories;

            List<AccessibilityFeature> accessibilityFeatures = new List<AccessibilityFeature>();
            foreach (ServiceAccessibilityFeatureModel accessibilityFeature in source.accessibility_features)
            {
                accessibilityFeatures.Add(new AccessibilityFeature()
                {
                    name = accessibilityFeature.name,
                    additional_info = accessibilityFeature.additional_info,
                    slug = accessibilityFeature.slug,
                    icon = accessibilityFeature.icon,
                    location_id = accessibilityFeature.location_id,
                });
            }
            model.accessibility_features = accessibilityFeatures;

            List<CommunityGroup> communityGroups = new List<CommunityGroup>();
            foreach (ServiceCommunityGroupModel communityGroup in source.community_groups)
            {
                communityGroups.Add(new CommunityGroup()
                {
                    name = communityGroup.name,
                    slug = communityGroup.slug,
                    is_range = communityGroup.is_range,
                    min = communityGroup.min_value,
                    max = communityGroup.max_value,
                    selected = communityGroup.selected,
                });
            }
            model.community_groups = communityGroups;

            List<Media> media_gallery = new List<Media>();
            foreach (ServiceMedia media in source.media_gallery)
            {
                media_gallery.Add(new Media()
                {
                    id = media.id,
                    url = media.url,
                    alt_text = media.alt_text,
                    caption = media.caption,
                    type = media.type,
                    thumbnail = media.thumbnail
                });
            }
            model.media_gallery = media_gallery;

            List<ServiceArea> serviceAreas = new List<ServiceArea>();
            foreach (ServiceAreaElasticSearchModel serviceArea in source.service_areas.Where(t => t.type.ToLower() == "country"))
            {
                serviceAreas.Add(new ServiceArea()
                {
                    code = serviceArea.code,
                    name = serviceArea.name,
                    type = serviceArea.type
                });
            }
            foreach (ServiceAreaElasticSearchModel serviceArea in source.service_areas.Where(t => t.type.ToLower() != "country").OrderBy(n => n.name).ThenBy(t => t.type))
            {
                serviceAreas.Add(new ServiceArea()
                {
                    code = serviceArea.code,
                    name = serviceArea.name,
                    type = serviceArea.type,
                });
            }
            model.service_areas = serviceAreas;

            List<Location> locations = new List<Location>();
            foreach (LocationModel location in source.locations)
            {
                locations.Add(new Location()
                {
                    formatted_address = location.formatted_address,
                    street_address = location.street_address,
                    country = location.country,
                    description = location.description,
                    state = location.state,
                    id = location.id,
                    latitude = location.point.Latitude,
                    longitude = location.point.Longitude,
                    locality = location.locality,
                    name = location.name,
                    postal_code = location.postal_code,
                    region = location.region,
                });
            }
            model.locations = locations;

            return model;
        }
    }
}
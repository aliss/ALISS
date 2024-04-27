using ALISS.API.Models.External;
using ALISS.Business.PresentationTransferObjects.Location;
using ALISS.Business.PresentationTransferObjects.Service;
using ALISS.Business.ViewModels.Location;
using ALISS.Models.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Windows.Input;
using PostcodeResult = ALISS.Business.ViewModels.Location.PostcodeResult;

namespace ALISS.Business.Services
{

    public class LocationService
    {
        private int _ItemsPerPage = 10;
        private string regexMatch = "[$&+,:;=?@#.\"|'\\-<>.^*()%!/\\-, '']";
        private ElasticSearchService _elasticSearchService;
        private string googleMapsKey = System.Configuration.ConfigurationManager.AppSettings["Settings:GoogleApiKey"];

        public LocationService()
        {
            _elasticSearchService = new ElasticSearchService();
        }

        public LocationListingViewModel ListLocation(string searchTerm, Guid organisationId, int page = 1)
        {
            LocationListingViewModel locationList = new LocationListingViewModel()
            {
                SearchTerm = searchTerm,
                Page = page,
                Location = new List<LocationPTO>(),
            };

            using (ALISSContext dc = new ALISSContext())
            {
                List<Location> locations = dc.Locations.Where(o => o.OrganisationId == organisationId).OrderBy(n => n.Name).ToList();
                searchTerm = Regex.Replace(searchTerm, regexMatch, string.Empty);
                locations = String.IsNullOrEmpty(searchTerm) ? locations : locations.Where(n => (String.IsNullOrEmpty(n.Name) == false && n.Name.ToLower()
                .Contains(searchTerm.ToLower())) || n.Postcode.ToLower().Contains(searchTerm.ToLower()) || n.City.ToLower()
                .Contains(searchTerm.ToLower()) || (String.IsNullOrEmpty(n.Address) == false && n.Address.ToLower().Contains(searchTerm.ToLower()))).ToList();

                locationList.OrganisationName = dc.Organisations.Find(organisationId).Name;
                locationList.TotalPages = (int)Math.Ceiling((double)locations.Count / _ItemsPerPage);
                locationList.TotalResults = locations.Count;
                int skip = (page - 1) * _ItemsPerPage;

                locations = locations.Skip(skip).Take(_ItemsPerPage).ToList();

                foreach (Location Location in locations)
                {
                    LocationPTO LocationListItem = new LocationPTO()
                    {
                        LocationId = Location.LocationId,
                        Name = Location.Name,
                        LocationCount = dc.ServiceLocations.Count(c => c.LocationId == Location.LocationId),
                        Address = Location.Address,
                        City = Location.City,
                        Postcode = Location.Postcode
                    };

                    locationList.Location.Add(LocationListItem);
                }
            }


            return locationList;
        }

        public string AddLocation(EditLocationViewModel model)
        {
            PostcodeResponse postcodeResponse = GetLatLongForPostcode(model);
            using (ALISSContext dc = new ALISSContext())
            {
                Location locationToAdd = new Location()
                {
                    LocationId = Guid.NewGuid(),
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    Postcode = model.Postcode,
                    Latitude = postcodeResponse.status == 200 ? postcodeResponse.result.latitude : 0,
                    Longitude = postcodeResponse.status == 200 ? postcodeResponse.result.longitude : 0,
                    OrganisationId = model.OrganisationId,

                };

                dc.Locations.Add(locationToAdd);
                dc.SaveChanges();
            }

            _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);

            return "Location added successfully";
        }

        public EditLocationViewModel AddServiceLocation(EditLocationViewModel model)
        {
            if (model.Latitude == 0 && model.Longitude == 0)
            {
                PostcodeResponse postcodeResponse = GetLatLongForPostcode(model);
                model.Latitude = postcodeResponse.status == 200 ? postcodeResponse.result.latitude : 0;
                model.Longitude = postcodeResponse.status == 200 ? postcodeResponse.result.longitude : 0;
            }

            using (ALISSContext dc = new ALISSContext())
            {
                Organisation locationOrg = dc.Organisations.Find(model.OrganisationId);
                if (locationOrg == null)
                {
                    locationOrg = new Organisation()
                    {
                        OrganisationId = model.OrganisationId,
                        Name = $"{model.Address}HoldingOrganisation",
                        CreatedUserId = 1,
                        CreatedOn = DateTime.Now,
                        Published = false
                    };
                    dc.Organisations.Add(locationOrg);
                    dc.SaveChanges();
                }

                Location locationToAdd = new Location()
                {
                    LocationId = Guid.NewGuid(),
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    Postcode = model.Postcode,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    OrganisationId = model.OrganisationId,
                };

                dc.Locations.Add(locationToAdd);
                dc.SaveChanges();

                return GetLocationForEdit(locationToAdd.LocationId);
            }
        }

        public EditLocationViewModel EditServiceLocation(EditLocationViewModel model)
        {
            if (model.Latitude == 0 && model.Longitude == 0)
            {
                PostcodeResponse postcodeResponse = GetLatLongForPostcode(model);
                model.Latitude = postcodeResponse.status == 200 ? postcodeResponse.result.latitude : 0;
                model.Longitude = postcodeResponse.status == 200 ? postcodeResponse.result.longitude : 0;
            }

            using (ALISSContext dc = new ALISSContext())
            {
                Location location = dc.Locations.Find(model.LocationId);
                if (location != null)
                {
                    location.Name = model.Name;
                    location.Address = model.Address;
                    location.City = model.City;
                    location.Postcode = model.Postcode;
                    location.Latitude = model.Latitude;
                    location.Longitude = model.Longitude;
                }

                dc.SaveChanges();

                List<ServiceLocation> serviceLocations = dc.ServiceLocations.Where(x => x.LocationId == model.LocationId).ToList();
                foreach (ServiceLocation serviceLocation in serviceLocations)
                {
                    _elasticSearchService.AddServiceToElasticSearch(serviceLocation.ServiceId);
                }

                return GetLocationForEdit(model.LocationId);
            }
        }
        
        public EditLocationViewModel GetLocationForEdit(Guid locationId)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var locationToEdit = dc.Locations.Find(locationId);

                return new EditLocationViewModel()
                {
                    LocationId = locationToEdit.LocationId,
                    Name = locationToEdit.Name,
                    Address = locationToEdit.Address,
                    City = locationToEdit.City,
                    Postcode = locationToEdit.Postcode,
                    Latitude = locationToEdit.Latitude,
                    Longitude = locationToEdit.Longitude,
                    OrganisationId = locationToEdit.OrganisationId,
                };
            }
        }

        //POST: Locations
        public string EditLocation(EditLocationViewModel model)
        {
            List<Guid> servicesWithLocation;
            bool organisationPublished;
            using (ALISSContext dc = new ALISSContext())
            {
                Location locationToEdit = dc.Locations.Find(model.LocationId);

                PostcodeResponse postcodeResponse = GetLatLongForPostcode(model);
                locationToEdit.Latitude = postcodeResponse.status == 200 ? postcodeResponse.result.latitude : 0;
                locationToEdit.Longitude = postcodeResponse.status == 200 ? postcodeResponse.result.longitude : 0;


                locationToEdit.Name = model.Name;
                locationToEdit.Address = model.Address;
                locationToEdit.City = model.City;
                locationToEdit.Postcode = model.Postcode;
                locationToEdit.OrganisationId = model.OrganisationId;

                dc.SaveChanges();

                organisationPublished = dc.Organisations.Find(model.OrganisationId).Published;
                servicesWithLocation = dc.ServiceLocations.Where(l => l.LocationId == locationToEdit.LocationId && l.Service.Published).Select(s => s.ServiceId).ToList();
            }

            foreach(Guid service in servicesWithLocation)
            {
                _elasticSearchService.AddServiceToElasticSearch(service);
            }

            if (organisationPublished)
            {
                _elasticSearchService.AddOrganisationToElasticSearch(model.OrganisationId);
            }

            return "Location edited successfully";
        }

        public DeleteLocationViewModel GetLocationForDelete(Guid locationId)
        {
            DeleteLocationViewModel model = new DeleteLocationViewModel()
            {
                LocationId = locationId,
                RelatedServices = new List<RelatedServicePTO>()
            };
            using (ALISSContext dc = new ALISSContext())
            {
                Location location = dc.Locations.Find(locationId);
                model.OrganisationId = location.OrganisationId;
                model.LocationName = location.Address;
                var relatedServices = dc.ServiceLocations.Where(c => c.LocationId == locationId).Select(s => s.ServiceId).ToList();
                var singleLocationServices = dc.ServiceLocations.GroupBy(s => s.ServiceId).Where(s => s.Count() == 1 && relatedServices.Contains(s.Key)).Select(s => s.FirstOrDefault()).ToList();

                if (singleLocationServices.Count() > 0)
                {
                    model.CanDelete = false;
                    foreach (var relatedService in singleLocationServices)
                    {
                        Service service = dc.Services.Find(relatedService.ServiceId);
                        model.RelatedServices.Add(new RelatedServicePTO()
                        {
                            ServiceId = relatedService.ServiceId,
                            ServiceName = service.Name,
                            OrganisationId = service.OrganisationId
                        });
                    }
                    return model;

                }

                model.CanDelete = true;
                return model;
            }
        }


        public Guid DeleteLocation(Guid locationId)
        {
            Guid organisationId;
            using (ALISSContext dc = new ALISSContext())
            {
                var serviceLocations = dc.ServiceLocations.Where(l => l.LocationId == locationId);
                if (serviceLocations.Count() > 0)
                {
                    dc.ServiceLocations.RemoveRange(serviceLocations);
                }

                var locationToRemove = dc.Locations.Find(locationId);
                organisationId = locationToRemove.OrganisationId;
                dc.Locations.Remove(locationToRemove);
                dc.SaveChanges();
            }

            _elasticSearchService.AddOrganisationToElasticSearch(organisationId);

            return organisationId;
        }

        public List<SelectListItem> GetLocationDropdownForServices(Guid organisationId, Guid? serviceId)
        {
            List<SelectListItem> locationDropdown = new List<SelectListItem>();
            using (ALISSContext dc = new ALISSContext())
            {
                List<Location> locations = dc.Locations.Where(o => o.OrganisationId == organisationId).ToList();
                foreach (var location in locations)
                {
                    SelectListItem locationToAdd = new SelectListItem()
                    {
                        Text = $"{location.Address}, {location.City}, {location.Postcode}",
                        Value = location.LocationId.ToString(),
                        Selected = serviceId.HasValue ? dc.ServiceLocations.Count(s => s.ServiceId == serviceId.Value && s.LocationId == location.LocationId) > 0 : false
                    };
                    locationDropdown.Add(locationToAdd);
                }
            }

            return locationDropdown;
        }

        public List<ServiceLocationPTO> GetLocationListForServices(Guid organisationId, Guid? serviceId)
        {
            List<ServiceLocationPTO> locationList = new List<ServiceLocationPTO>();

            using (ALISSContext dc = new ALISSContext())
            {
                List<Location> locations = dc.Locations.Where(o => o.OrganisationId == organisationId).ToList();
                foreach (Location location in locations)
                {
                    string formattedAddress = $"{location.Address}, {location.City}, {location.Postcode}";
                    if (!String.IsNullOrEmpty(location.Name))
                    {
                        formattedAddress = $"{location.Name}, {formattedAddress}";
                    }

                    ServiceLocationPTO locationToAdd = new ServiceLocationPTO()
                    {
                        LocationId = location.LocationId,
                        Text = $"{location.Address}, {location.City}, {location.Postcode}",
                        Value = location.LocationId.ToString(),
                        Selected = serviceId.HasValue && dc.ServiceLocations.Count(s => s.ServiceId == serviceId.Value && s.LocationId == location.LocationId) > 0,
                        Name = location.Name,
                        FormattedAddress = formattedAddress,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Address = location.Address,
                        City = location.City,
                        Postcode = location.Postcode
                    };

                    locationList.Add(locationToAdd);
                }
            }

            return locationList;
        }

        private bool DoesLocationExist(string locationName)
        {
            using (ALISSContext dc = new ALISSContext())
            {
                Location location = dc.Locations.Where(n => n.Name.ToLower() == locationName.ToLower()).FirstOrDefault();

                if (location != null)
                {
                    return true;
                }

                return false;
            }
        }

        private PostcodeResponse GetLatLongForPostcode(EditLocationViewModel location)
        {
            switch (location.Postcode.Replace(" ", "").ToLower())
            {
            case "g334rz":
                return new PostcodeResponse
                {
                    error = null,
                    status = 200,
                    result = new PostcodeResult
                    {
                        postcode = location.Postcode,
                        latitude = 55.857569,
                        longitude = -4.133327
                    }
                };
            }

            var gRequest = (HttpWebRequest)WebRequest.Create("https://maps.googleapis.com/maps/api/geocode/json?" +
                "address=" + location.Address.Replace(" ", "+") +
                "&components=" +
                    "country:GB" +
                    "|postal_code:" + location.Postcode.Replace(" ", "").ToUpper() +
                "&key=" + googleMapsKey);

            //var gRequest = (HttpWebRequest)WebRequest.Create("https://maps.googleapis.com/maps/api/geocode/json?address=Glasgow&components=country:GB|postal_code:G313PX&key=" + googleMapsKey);

            try
            {
                using (var response = (HttpWebResponse)gRequest.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        dynamic json = Json.Decode(objText);

                        return new PostcodeResponse
                        {
                            error = null,
                            status = 200,
                            result = new PostcodeResult
                            {
                                postcode = location.Postcode,
                                latitude = (double)json.results[0].geometry.location.lat,
                                longitude = (double)json.results[0].geometry.location.lng,
                            }
                        };
                    }
                }
            }
            catch (Exception)
            {
                var request = (HttpWebRequest)WebRequest.Create("http://api.postcodes.io/postcodes/" + location.Postcode);
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        var model = (PostcodeResponse)js.Deserialize(objText, typeof(PostcodeResponse));
                        return model;
                    }
                }
            }
                catch (Exception ex) 
            {
                return new PostcodeResponse()
                {
                    status = 404,
                        error = ex.Message
                };
            }
        }
    }
    }
}














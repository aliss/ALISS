/** @format */

class Map {
	constructor() {
		this.init = this.init.bind(this);
		this.loadScript();
	}

	async loadScript() {
		if (googleMapsKey === "") {
			googleMapsKey = "AIzaSyBaQiiWHE0SExspaEBocXLaj-0LbK_BCFA";
		}
		const script = document.createElement("script");
		script.src = `https://maps.googleapis.com/maps/api/js?key=${googleMapsKey}&libraries=places&v=weekly&callback=initMap`;
		// script.src = `https://maps.googleapis.com/maps/api/js?sensor=false&libraries=places&v=weekly&callback=initMap`;
		script.defer = true;
		script.async = true;

		try {
			await new Promise((resolve) => {
				window.initMap = () => {
					delete window.initMap;
					resolve();
					this.init();
				};
				script.onerror = () => {
					delete window.initMap;
					console.error("Error loading Google Maps API script.");
				};
				document.head.appendChild(script);
			});
		} catch (error) {
			console.error("Error loading Google Maps API script:", error);
		}
	}

	init() {
		const MAPDATA = {
			Zoom: 14
		};

		const CLASSES = {
			MapElement: "aliss-component-map"
		};

		const ID = {
			MapElement: "aliss-component-map",
			MapErrorElement: "aliss-component-map-error",
			MapErrorElementContents: "aliss-component-map-error-contents"
		};

		const ATTRIBUTES = {
			Areas: "data-map-areas",
			Markers: "data-map-markers",
			ID: "data-id"
		};

		const Type = {
			Feature: "Feature"
		};

		const NAMEKEY = {
			Lad18nm: "lad18nm"
		};

		const COORDS = { lat: 55.87649918, lng: -4.21478987 };

		let infoWindow : any = new google.maps.InfoWindow();
		let markers = [];
		
		let if_map = document.getElementById(ID.MapElement);
		if (if_map) {
			let map: null = null;

			let data_areas = if_map.getAttribute(ATTRIBUTES.Areas),
				data_markers = if_map.getAttribute(ATTRIBUTES.Markers),
				data_ID = if_map.getAttribute(ATTRIBUTES.ID);

			// gridOptions_areas = JSON.parse(data_areas),
			let gridOptions_markers = JSON.parse(data_markers);

			let defaultCenter = COORDS;
			
			if (data_markers && data_areas) {
				defaultCenter = {
					lat: gridOptions_markers.markers[0].latitude,
					lng: gridOptions_markers.markers[0].longitude
				};
			} else if (data_markers == null && data_areas) {
				defaultCenter = COORDS;
			} else if (data_markers && data_areas == null) {
				defaultCenter = {
					lat: gridOptions_markers.markers[0].latitude,
					lng: gridOptions_markers.markers[0].longitude
				};
			} else if (data_markers == null && data_areas == null) {
				if_map.remove();
				if_map.style.display = "none";
				document.getElementById(ID.MapErrorElement).addClass =
					'Please try refreshing the page or visiting the page again later. If the issue persists, please notify us on <a id="aliss-contact-email" href="mailto:help@aliss.org"';
			}

			map = new google.maps.Map(
				document.getElementById(ID.MapElement) as HTMLElement, 
				{
					center: defaultCenter,
					zoom: MAPDATA.Zoom,
					streetViewControl: false
				}
			);
			
			var bounds = new google.maps.LatLngBounds();

			if (data_markers) {
				const markerImage = 'https://unpkg.com/leaflet@1.5.1/dist/images/marker-icon.png';
				for (let marker_count in gridOptions_markers.markers) {
					const marker = new google.maps.Marker({
						icon: markerImage,
						position: {
							lat: gridOptions_markers.markers[marker_count].latitude,
							lng: gridOptions_markers.markers[marker_count].longitude
						},
						map,
						title: gridOptions_markers.markers[marker_count].address,
						alt: gridOptions_markers.markers[marker_count].address
					});
					marker.addListener("click", () => {
						infoWindow.setContent(`<h4 style="margin:0;padding:0">${gridOptions_markers.markers[marker_count].name}</h4><p style="margin:0;padding:0">${gridOptions_markers.markers[marker_count].address}</p>`);
						infoWindow.open(map, marker);
					});
					markers.push(marker);
					if(gridOptions_markers.markers.length > 1) {
						bounds.extend(marker.getPosition());
						map.fitBounds(bounds, true);
					}

					google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
						let image = document.querySelector('[aria-label="'+ gridOptions_markers.markers[marker_count].address +'"] img') as HTMLImageElement;
						if (image != null) {
							image.alt = gridOptions_markers.markers[marker_count].address;
						}
					});
				}
			}
			// fix for the alt text to be realeased in the aliss sprint
			// if (data_markers) {
			// 	// Assign the markers

			// 	for (marker_count in gridOptions_markers.markers) {
			// 		L.marker([        gridOptions_markers.markers[marker_count].latitude,
			// 			gridOptions_markers.markers[marker_count].longitude,
			// 		], {
			// 			alt: gridOptions_markers.markers[marker_count].address,
			// 		})
			// 			.bindPopup(gridOptions_markers.markers[marker_count].address)
			// 			.addTo(map);
			// 	}
				
			// }
			
			if (data_ID) {
				const APIURL = 'service-area-spatial/?service_id=';
				//const APIURL = 'http://dacs-api.aliss.org/v4/service-area-spatial/?service_id=';
				const request = new XMLHttpRequest();
				request.open("GET", apiBaseUrl + APIURL + data_ID, true);
				request.onload = function () {
					if (this.status >= 200 && this.status < 400) {
						const response = this.response.replace(/\/\/.*\n/g, '');
						const data = JSON.parse(response);
						if (data.length > 0) {
							var boundaries: { lng: any; lat: any; }[] = [];
							var boundary = [];
							data.forEach(function (feature) {
								if (feature.length != 0) {
									console.log(feature);
									map.data.addGeoJson(feature);
									map.data.setStyle(function (e) {
										console.log(e);
										return {
											strokeColor: '#4C66DF',           
											strokeWeight: 2,
											strokeOpacity: 1,                         
											fillColor: '#0000FF',
											fillOpacity: 0.15
										};
									});
									map.data.addListener('click', function(event : any) {
										let country = event.feature.getProperty('ctry17nm');
										let county = event.feature.getProperty('HBName');
										let localArea = event.feature.getProperty('lad18nm');
										let name = event.feature.getProperty('name');
										let areaTitle = localArea != null ? localArea : county != null ? county : country != null ? country : name;
										infoWindow.setContent('<h5 style="margin: 0; padding: 0">' + areaTitle +'</h5>');// show the html variable in the infoWindow
										infoWindow.setPosition(event.latLng);
										infoWindow.open(map);
									});
									let areas = feature.geometry.coordinates;
									const type = feature.geometry.type;
									for (let a = 0; a < areas.length; a++) {
										let coords = areas[a];
										for(let p = 0; p < coords.length; p++) {
											if (type === 'Polygon') {
												boundaries.push({ 
													lng: coords[p][0], 
													lat: coords[p][1]
												});
											} else if (type=== 'MultiPolygon') {
												let boundArea = coords[p];
												for(let b = 0; b < boundArea.length; b++) {
													boundaries.push({
														lng: boundArea[b][0], 
														lat: boundArea[b][1]
													});
												}
											}
										}
									}
								}
							});
							
							boundary.push(new google.maps.Polygon({
								path: boundaries
							}));
							
							for (var j = 0; j < boundary.length; j++) {
								for (var i = 0; i < boundary[j].getPath().getLength(); i++) {
									bounds.extend(boundary[j].getPath().getAt(i));
								}
							}
							google.maps.event.addListener(map, "click", function () {
								infoWindow.close();
							});
							map.fitBounds(bounds, true);
						}
					} else {
						console.error("Failed to load data from API.");
					}
				};
				request.onerror = function () {
					console.error("Error loading data from API.");
				};
				
				request.send();
			}
		}
	}
}

export default Map;

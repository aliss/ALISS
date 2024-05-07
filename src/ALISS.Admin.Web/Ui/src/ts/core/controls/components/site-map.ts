/** @format */
import 'regenerator-runtime/runtime';
import Aliss from '../..';

class Map {
	constructor() {
		this.init = this.init.bind(this);
		this.loadScript();

		enum ID {
			checkedList = "checkedSelection",
		}
	}

	async loadScript() {
		if (googleMapsKey === "") {
			googleMapsKey = "AIzaSyBaQiiWHE0SExspaEBocXLaj-0LbK_BCFA";
		}
		const script = document.createElement("script");
		script.src = `https://maps.googleapis.com/maps/api/js?key=${googleMapsKey}&libraries=places&v=weekly&callback=initMap`;
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
		let map: null = null;
		var bounds = new google.maps.LatLngBounds();
		var boundary = [];
		var boundaries: { lng: any; lat: any; }[] = [];
		var dataRegions = [];

		const checkboxContainer = document.querySelector('[data-checked-selection]') as HTMLElement;
		let checkboxContainerName = checkboxContainer?.dataset.name as string;
		const hiddenInput = document.getElementById(checkboxContainerName) as HTMLInputElement;
		var checkboxes = checkboxContainer?.querySelectorAll('input[type=checkbox]');
		var serviceAreaContainer = document.querySelector('.aliss-datainput-serviceareas') as HTMLElement;
		var serviceAreaCheckboxes = serviceAreaContainer?.querySelectorAll('input[type=checkbox]');
		const locationSubmit = document.getElementById('submit-location') as HTMLButtonElement;

		const onLocationChange = (checkbox: HTMLInputElement) => {
			checkbox.addEventListener(Aliss.Enums.Events.Change, function () {
				if (checkbox.checked) {
					addMarker(checkbox.dataset.name, checkbox.dataset.formatted, Number(checkbox.dataset.lat), Number(checkbox.dataset.lon));
				} else {
					removeMarker(checkbox.dataset.formatted);
				}
				initRemoveSelected();
			});
		}

		const onServiceAreaChange = (checkbox: HTMLInputElement) => {
			checkbox.addEventListener(Aliss.Enums.Events.Change, function () {
				initRemoveSelectedServiceAreas();
				refreshMapRegions();
			});
		}

		const refreshMapRegions = () => {
			dataRegions.forEach(function (feature) {
				map.data.remove(feature[0]);
			});

			let selectedServiceAreasField = document.getElementById("SelectedServiceAreas") as HTMLInputElement;
			var selectedServiceAreas = selectedServiceAreasField.value;
			if (selectedServiceAreas) {
				getRegionsAndAddToMap(apiBaseUrl + "selected-service-area-spatial/?service_areas=" + selectedServiceAreas);
			}
			
			// bounds = new google.maps.LatLngBounds();
			// markers.forEach(function (marker) {
			// 	bounds.extend(marker.getPosition());
			// });
			// console.log("refreshMapRegions");
			// console.log(bounds);

			// map.fitBounds(bounds, true);
		}

		const removeMarker = (address) => {
			var markerIndex = markers.findIndex((x => x.title == address));
			var marker = markers[markerIndex];
			marker.setMap(null);
			markers.splice(markerIndex, 1);

			bounds = new google.maps.LatLngBounds();
			markers.forEach(function (item) {
				bounds.extend(item.getPosition());
			});
			for (var j = 0; j < boundary.length; j++) {
				for (var i = 0; i < boundary[j].getPath().getLength(); i++) {
					bounds.extend(boundary[j].getPath().getAt(i));
				}
			}

			map.fitBounds(bounds, true);
		}

		const addMarker = (name, address, latitude, longitude) => {
			const marker = new google.maps.Marker({
				icon: markerImage,
				position: {
					lat: latitude,
					lng: longitude
				},
				map,
				title: address,
				alt: address
			});
			marker.addListener("click", () => {
				infoWindow.setContent(`<h4 style="margin:0;padding:0">${name}</h4><p style="margin:0;padding:0">${address}</p>`);
				infoWindow.open(map, marker);
			});
			markers.push(marker);
			bounds.extend(marker.getPosition());
			map.fitBounds(bounds, true);

			google.maps.event.addListenerOnce(map, 'tilesloaded', function () {
				let image = document.querySelector('[aria-label="'+ address +'"] img') as HTMLImageElement;
				if (image != null) {
					image.alt = address;
				}
			});
		}

		const removeRegion = () => {
			dataRegions.forEach(function (feature) {
				map.data.remove(feature[0]);
			});
		}

		const getRegionsAndAddToMap = (serviceAreasApiUrl) => {
			const request = new XMLHttpRequest();
			request.open("GET", serviceAreasApiUrl, true);
			request.onload = function () {
				if (this.status >= 200 && this.status < 400) {
					const response = this.response.replace(/\/\/.*\n/g, '');
					const data = JSON.parse(response);
					if (data.length > 0) {
						// console.log("API Output:");
						// console.log(data);
						data.forEach(function (feature) {
							addRegionToMap(feature);
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
						console.log("getRegionsAndAddToMap");
						console.log(bounds);
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

		const addRegionToMap = (feature) => {
			if (feature.length != 0) {
				var dataFeature = map.data.addGeoJson(feature,);
				dataRegions.push(dataFeature);
				map.data.setStyle(function (e) {
					return {
						strokeColor: '#4C66DF',
						strokeWeight: 2,
						strokeOpacity: 1,
						fillColor: '#0000FF',
						fillOpacity: 0.15
					};
				});
				map.data.addListener('click', function (event: any) {
					let country = event.feature.getProperty('ctry17nm');
					let county = event.feature.getProperty('HBName');
					let localArea = event.feature.getProperty('lad18nm');
					let name = event.feature.getProperty('name');
					let areaTitle = localArea != null ? localArea : county != null ? county : country != null ? country : name;
					infoWindow.setContent('<h5 style="margin: 0; padding: 0">' + areaTitle + '</h5>');// show the html variable in the infoWindow
					infoWindow.setPosition(event.latLng);
					infoWindow.open(map);
				});
				let areas = feature.geometry.coordinates;
				const type = feature.geometry.type;
				for (let a = 0; a < areas.length; a++) {
					let coords = areas[a];
					for (let p = 0; p < coords.length; p++) {
						if (type === 'Polygon') {
							boundaries.push({
								lng: coords[p][0],
								lat: coords[p][1]
							});
						} else if (type === 'MultiPolygon') {
							let boundArea = coords[p];
							for (let b = 0; b < boundArea.length; b++) {
								boundaries.push({
									lng: boundArea[b][0],
									lat: boundArea[b][1]
								});
							}
						}
					}
				}
			}
		}

		const initRemoveSelectedServiceAreas = () => {
			console.log("Adding click events");
			var removeButtons = document.querySelectorAll('.aliss-selected__remove--service_area');
			for (var i = 0; i < removeButtons.length; i++) {
				removeButtons[i].addEventListener("click", (el) => {
					el.preventDefault();
					console.log("refreshing regions");
					refreshMapRegions();
				});
			}
		}

		const initRemoveSelected = () => {
			var removeButtons = document.querySelectorAll('.aliss-selected__remove--location');
			for (var i = 0; i < removeButtons.length; i++) {
				removeButtons[i].addEventListener("click", (el) => {
					el.preventDefault();
					console.log("refreshing markers");
					let element = el.target as HTMLButtonElement;
					let input = element.dataset.input as string;
					let item = document.getElementById(input) as HTMLInputElement;
					removeMarker(item.dataset.formatted);
				});
			}
		}

		if (checkboxContainer) {
			for (var i = 0; i < checkboxes.length; i++) {
				var item = checkboxes[i] as HTMLInputElement;
				onLocationChange(item);
			}
		}

		if (locationSubmit) {
			checkboxContainer.addEventListener("change", () => {
				checkboxes = checkboxContainer?.querySelectorAll('input[type=checkbox]');
				for (var i = 0; i < checkboxes.length; i++) {
					var item = checkboxes[i] as HTMLInputElement;
					if (item.checked) {
						addMarker(item.dataset.name, item.dataset.formatted, Number(item.dataset.lat), Number(item.dataset.lon));
					}
					onLocationChange(item);
				}
			})
		}

		if (serviceAreaContainer) {
			for (var i = 0; i < serviceAreaCheckboxes.length; i++) {
				var item = serviceAreaCheckboxes[i] as HTMLInputElement;
				onServiceAreaChange(item);
			}
		}

		initRemoveSelected();
		initRemoveSelectedServiceAreas();

		const markerImage = 'https://unpkg.com/leaflet@1.5.1/dist/images/marker-icon.png';

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
			AreasIds: "data-map-areas-ids",
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

		let infoWindow: any = new google.maps.InfoWindow();
		let markers = [];
		let dataFeature = [];

		let if_map = document.getElementById(ID.MapElement);
		if (if_map) {
			let data_areas = if_map.getAttribute(ATTRIBUTES.Areas),
				data_areas_ids = if_map.getAttribute(ATTRIBUTES.AreasIds),
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
			}

			map = new google.maps.Map(
				document.getElementById(ID.MapElement) as HTMLElement,
				{
					center: defaultCenter,
					zoom: MAPDATA.Zoom,
					streetViewControl: false,
					maxZoom: 15
				}
			);

			if (data_markers) {
				for (let marker_count in gridOptions_markers.markers) {
					addMarker(
						gridOptions_markers.markers[marker_count].name,
						gridOptions_markers.markers[marker_count].address,
						gridOptions_markers.markers[marker_count].latitude,
						gridOptions_markers.markers[marker_count].longitude);
				}
			}

			if (data_ID) {
				getRegionsAndAddToMap(apiBaseUrl + "selected-service-area-spatial/?service_areas=" + data_areas_ids);
			}
		}
	}
}

export default Map;

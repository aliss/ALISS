import { post } from 'jquery';
import Aliss from '../../';
import Template from './forms.checkedselection.template';

class Locations {
	constructor() {

		enum CLASSES {
			error = "aliss-form__group--error",
			addLocations = "add-locations",
			addRegions = "add-regions",
			selectedItem = "aliss-selected__item",
			hide = 'hide',
			googleSuggestionsContainer = "pac-container",
		}

		enum ID {
			selectList = "availableLocations",
			checkedList = "checkedSelection",
			newCheckboxAddition = "newCheckboxAddition",
			addNewLocation = "add-new-location",
			newLocation = "new-location",
			cancelLocation = "cancel-new-location",
			submitLocation = "submit-location",
			searchAddress = "search-address",
			manualAdddress = "manual-address",
			manualAddressContainer = 'manualAddressContainer',
			newLocationName = "NewLocationName",
			newLocationAddress = "NewLocationAddress",
			newLocationCity = "NewLocationCity",
			newLocationPostcode = "NewLocationPostcode",
			newLocationLatitude = "NewLocationLatitude",
			newLocationLongitude = "NewLocationLongitude",
			orgId = "OrganisationId",
			addLocations = "add-locations",
			addRegions = "add-regions",
			locationsFieldset = "locationsFieldset",
			regionsFieldset = "regionsFieldset",
			locationsViewAllBtn = "view-selected-addresses",
			regionsViewAllBtn = "view-selected-regions",
			selectedLocations = "selected-locations",
			selectedRegions = "serviceareas-selected",
			locationsCount = "count-selected-locations",
			editLocationButton = "edit_location_",
			editLocation = "edit-location",
			cancelLocationEdit = "cancel-edit-location",
			submitLocationEdit = "submit-updated-location",
			searchAddressEdit = "search-address-edit",
			editLocationName = "EditLocationName",
			editLocationAddress = "EditLocationAddress",
			editLocationCity = "EditLocationCity",
			editLocationPostcode = "EditLocationPostcode",
			editLocationLatitude = "EditLocationLatitude",
			editLocationLongitude = "EditLocationLongitude",
			editLocationId = "EditLocationId",
		}

		enum EDITS {
			id = "##ID##",
			name = "##NAME##",
			value = "##VALUE##"
		}

		const checkSelected = () => {
			const locationCheckboxes = document.querySelectorAll('input[name="' + ID.selectList + '"]');
			const hiddenBox = document.getElementById(ID.selectedLocations) as HTMLInputElement;

			for (var i = 0; i < locationCheckboxes.length; i++) {
				const item = locationCheckboxes[i] as HTMLInputElement;
				if (item.checked) {
					let selectName = item.dataset.name as string;
					let selectedLabel = item.dataset.label as string;
					let selectedValue = item.value;
					const html = Template;
					const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), selectedLabel).replace(new RegExp(EDITS.id, 'g'), selectedValue).replace(EDITS.name, "SelectedLocations");
					var domElement = document.createElement("div");
					domElement.id = "check-" + selectedValue;
					domElement.classList.add(CLASSES.selectedItem);
					domElement.innerHTML = updatedHtml;
					hiddenBox.appendChild(domElement);
				}
			}
		}

		const init = () => {
			toggleLocationsAndRegions();
			toggleLocationSelections();
			toggleRegionSelections();
			editAddress();

			checkSelected();

			const adjustGoogleSuggestions = (input: HTMLInputElement) => {
				let googleDropDown = document.querySelectorAll('.' + CLASSES.googleSuggestionsContainer);
				let searchAddressParent = input.parentElement as HTMLElement;
				let googleSuggestions = [] as any;
				
				if (googleDropDown != null && googleDropDown.length > 0) {
					googleDropDown.forEach(el => {
						googleSuggestions.push(el);
						el.remove();
					});
					searchAddressParent.appendChild(googleSuggestions.slice(-1)[0]);
				}
			}

			let addNewLocationButton = document.getElementById(ID.addNewLocation) as HTMLElement;
			let targetDiv = document.getElementById(ID.newLocation) as HTMLElement;
			let targetDivEdit = document.getElementById(ID.editLocation) as HTMLElement;
			if (typeof (addNewLocationButton) != 'undefined' && addNewLocationButton != null) {
				addNewLocationButton.onclick = (e) => {
					e.preventDefault();
					Aliss.Helpers.toggleClass(targetDiv, CLASSES.hide);
				}
				var locationCancel = document.getElementById(ID.cancelLocation) as HTMLElement;
				var locationSubmit = document.getElementById(ID.submitLocation) as HTMLElement;
				let searchAddress = document.getElementById(ID.searchAddress) as HTMLInputElement;
				let manualAdddress = document.getElementById(ID.manualAdddress) as HTMLInputElement;
				let manualAdddressContainer = document.getElementById(ID.manualAddressContainer) as HTMLDivElement;
				let name = document.getElementById(ID.newLocationName) as HTMLInputElement;
				let address = document.getElementById(ID.newLocationAddress) as HTMLInputElement;
				let city = document.getElementById(ID.newLocationCity) as HTMLInputElement;
				let postcode = document.getElementById(ID.newLocationPostcode) as HTMLInputElement;
				let orgId = document.getElementById(ID.orgId) as HTMLInputElement;
				let lat = document.getElementById(ID.newLocationLatitude) as HTMLInputElement;
				let lon = document.getElementById(ID.newLocationLongitude) as HTMLInputElement;

				var locationSubmitEdit = document.getElementById(ID.submitLocationEdit) as HTMLElement;
				let searchAddressEdit = document.getElementById(ID.searchAddressEdit) as HTMLInputElement;
				let editName = document.getElementById(ID.editLocationName) as HTMLInputElement;
				let editAddress = document.getElementById(ID.editLocationAddress) as HTMLInputElement;
				let editCity = document.getElementById(ID.editLocationCity) as HTMLInputElement;
				let editPostcode = document.getElementById(ID.editLocationPostcode) as HTMLInputElement;
				let editLat = document.getElementById(ID.editLocationLatitude) as HTMLInputElement;
				let editLon = document.getElementById(ID.editLocationLongitude) as HTMLInputElement;
				let editLocationId = document.getElementById(ID.editLocationId) as HTMLInputElement;

				var postcodeRegex = /^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))\s?[0-9][A-Za-z]{2})$/;

				postcode.addEventListener("input", () => {
					var postcodeCheck = postcodeRegex.test(postcode.value);
					console.log(postcodeCheck);
				});

				searchAddress.addEventListener("input", () => {
					adjustGoogleSuggestions(searchAddress);
				});

				searchAddress.addEventListener("focus", () => {
					adjustGoogleSuggestions(searchAddress);
				});

				if (locationCancel != null) {
					locationCancel.onclick = function (e) {
						e.preventDefault();
						searchAddress.value = "";
						name.value = "";
						address.value = "";
						city.value = "";
						postcode.value = "";
						lat.value = "";
						lon.value = "";
						Aliss.Helpers.toggleClass(targetDiv, CLASSES.hide);
					}
				}

				if (locationSubmitEdit != null) {
					locationSubmitEdit.onclick = function (e) {
						e.preventDefault();

						var postcodeCheck = postcodeRegex.test(editPostcode.value);

						if (editAddress.value == "" || editCity.value == "" || editPostcode.value == "" || postcodeCheck == false) {
							editAddress.value == "" ? Aliss.Helpers.addClass(editAddress.parentNode, CLASSES.error) : null;
							editCity.value == "" ? Aliss.Helpers.addClass(editCity.parentNode, CLASSES.error) : null;
							editPostcode.value == "" || postcodeCheck == false ? Aliss.Helpers.addClass(editPostcode.parentNode, CLASSES.error) : null;
						} else {
							let data = {
								LocationId: editLocationId.value,
								OrganisationId: orgId.value,
								Name: editName.value,
								Address: editAddress.value,
								City: editCity.value,
								Postcode: editPostcode.value,
								Latitude: editLat.value,
								Longitude: editLon.value
							};

							Aliss.Helpers.sendJson("/Organisation/EditServiceLocation", data, (response: any) => {
								let result = JSON.parse(response);
								console.log(result);

								let formattedLocation = result.Address + ", " + result.City + ", " + result.Postcode;
								if (result.Name != null) {
									formattedLocation = result.Name + ", " + formattedLocation;
								}

								let locationCheckbox = document.getElementById("location_" + result.LocationId) as HTMLInputElement;
								let locationEditButton = document.getElementById("edit_location_" + result.LocationId) as HTMLInputElement;
								let locationLabel = document.getElementById("location_label_" + result.LocationId) as HTMLLabelElement;

								var isChecked = locationCheckbox.checked;
								if (isChecked) {
									locationCheckbox.click();
									// setTimeout(() => {
									// 	locationCheckbox?.click();
									// }, 300);
								}
								locationCheckbox.dataset.name = result.Name ?? "";
								locationCheckbox.dataset.label = result.Address + ", " + result.City + ", " + result.Postcode;
								locationCheckbox.dataset.formatted = formattedLocation;
								locationCheckbox.dataset.lat = result.Latitude;
								locationCheckbox.dataset.lon = result.Longitude;
								locationCheckbox.value = result.LocationId;
								locationLabel.textContent = result.Address + ", " + result.City + ", " + result.Postcode;
								locationEditButton.dataset.name = result.Name ?? "";
								locationEditButton.dataset.address = result.Address;
								locationEditButton.dataset.city = result.City;
								locationEditButton.dataset.postcode = result.Postcode;
								locationEditButton.dataset.formatted = formattedLocation;
								locationEditButton.dataset.lat = result.Latitude;
								locationEditButton.dataset.lon = result.Longitude;
								if (isChecked) {
									// setTimeout(() => {
									// 	locationCheckbox?.click();
									// }, 300);
									locationCheckbox.click();
								}
																

								setTimeout(() => {
									Aliss.Helpers.addClass(targetDivEdit, CLASSES.hide);
									editName.value = "";
									editAddress.value = "";
									editCity.value = "";
									editPostcode.value = "";
									editLat.value = "";
									editLon.value = "";
									searchAddressEdit.value = "";
								}, 300);
							});
						}
					}
				}

				if (locationSubmit != null) {
					locationSubmit.onclick = function (e) {
						var postcodeCheck = postcodeRegex.test(postcode.value);

						e.preventDefault();
						let data = {
							OrganisationId: orgId.value,
							Name: name.value,
							Address: address.value,
							City: city.value,
							Postcode: postcode.value,
							Latitude: lat.value,
							Longitude: lon.value
						};

						if (address.value == "" || city.value == "" || postcode.value == "" || postcodeCheck == false) {
							address.value == "" ? Aliss.Helpers.addClass(address.parentNode, CLASSES.error) : null;
							city.value == "" ? Aliss.Helpers.addClass(city.parentNode, CLASSES.error) : null;
							postcode.value == "" || postcodeCheck == false ? Aliss.Helpers.addClass(postcode.parentNode, CLASSES.error) : null;

							if(manualAdddressContainer.classList.contains(CLASSES.hide)){
								manualAdddress.checked = true;
								manualAdddressContainer.classList.remove(CLASSES.hide);
							}

							if (!manualAdddress.checked) {
								Aliss.Helpers.addClass(searchAddress.parentNode, CLASSES.error);
							}
							else {
								Aliss.Helpers.removeClass(searchAddress.parentNode, CLASSES.error);
							}
						} else {
							Aliss.Helpers.sendJson("/Organisation/AddServiceLocation", data, (response: any) => {
								let result = JSON.parse(response);
								console.log(result);
								let checkedList = document.getElementById(ID.checkedList);
								let newCheckboxContainer = document.createElement('div') as HTMLElement;
								let newCheckbox = document.createElement('input') as HTMLInputElement;
								let newCheckboxLabel = document.createElement('label') as HTMLLabelElement;
								let newEditButton = document.createElement('a') as HTMLAnchorElement;
								newCheckboxContainer.classList.add('aliss-form__checkbox');

								let formattedLocation = result.Address + ", " + result.City + ", " + result.Postcode;
								if (result.Name != null) {
									formattedLocation = result.Name + ", " + formattedLocation;
								}

								newCheckbox.id = "location_" + result.LocationId;
								newCheckbox.dataset.name = result.Name ?? "";
								newCheckbox.dataset.label = result.Address + ", " + result.City + ", " + result.Postcode;
								newCheckbox.dataset.formatted = formattedLocation;
								newCheckbox.name = ID.selectList;
								newCheckbox.classList.add('aliss-form__input');
								newCheckbox.type = "checkbox";
								newCheckbox.value = result.LocationId;
								newCheckbox.dataset.lat = result.Latitude;
								newCheckbox.dataset.lon = result.Longitude;

								newCheckboxLabel.textContent = result.Address + ", " + result.City + ", " + result.Postcode;
								newCheckboxLabel.htmlFor = "location_" + result.LocationId;
								newCheckboxLabel.id = "location_label_" + result.LocationId;

								newEditButton.href = "#edit-location";
								newEditButton.id = "edit_location_" + result.LocationId;
								newEditButton.classList.add("fa");
								newEditButton.classList.add("fa-pencil-square-o");
								newEditButton.dataset.id = result.LocationId;
								newEditButton.dataset.name = result.Name ?? "";
								newEditButton.dataset.address = result.Address;
								newEditButton.dataset.city = result.City;
								newEditButton.dataset.postcode = result.Postcode;
								newEditButton.dataset.lat = result.Latitude;
								newEditButton.dataset.lon = result.Longitude;

								newCheckboxContainer.appendChild(newCheckbox);
								newCheckboxContainer.appendChild(newCheckboxLabel);
								newCheckboxContainer.appendChild(document.createTextNode(" "));
								newCheckboxContainer.appendChild(newEditButton);

								checkedList?.appendChild(newCheckboxContainer);
								document.getElementById("location_" + result.LocationId);

								setTimeout(() => {
									Aliss.Helpers.addClass(targetDiv, CLASSES.hide);
									name.value = "";
									address.value = "";
									city.value = "";
									postcode.value = "";
									lat.value = "";
									lon.value = "";
									searchAddress.value = "";
									lat.value = "";
									lon.value = "";
								}, 300);

								var link = document.getElementById("location_" + result.LocationId);
								const reloadedSelections = new Aliss.Controls.CheckedSelections;
								reloadedSelections.init;
								setTimeout(() => {
									link?.click();
								}, 300);
							});
						}
					}
				}

			}
		}

		const toggleLocationsAndRegions = () => {
			let locationsFieldset = document.getElementById(ID.locationsFieldset) as HTMLElement;
			let regionsFieldset = document.getElementById(ID.regionsFieldset) as HTMLElement;
			document.addEventListener("change", (el) => {
				let element = el.target as HTMLInputElement;
				if (element.id == ID.addLocations) {
					if (element.checked) {
						Aliss.Helpers.removeClass(locationsFieldset, CLASSES.hide);
						Aliss.Helpers.addClass(regionsFieldset, CLASSES.hide);
					}
				}
				else if (element.id == ID.addRegions) {
					if (element.checked) {
						Aliss.Helpers.removeClass(regionsFieldset, CLASSES.hide);
						Aliss.Helpers.addClass(locationsFieldset, CLASSES.hide);
					}
				}
			});
		}

		const toggleLocationSelections = () => {
			let locationsViewAllBtn = document.getElementById(ID.locationsViewAllBtn) as HTMLElement;
			let selectedLocations = document.getElementById(ID.selectedLocations) as HTMLElement;

			if (locationsViewAllBtn != null && selectedLocations != null) {
				if (selectedLocations.classList.contains(CLASSES.hide)) {
					locationsViewAllBtn.textContent = "View all";
				} else {
					locationsViewAllBtn.textContent = "Hide all";
				}
				locationsViewAllBtn.addEventListener("click", (el) => {
					el.preventDefault();
					selectedLocations.classList.toggle(CLASSES.hide);
					if (selectedLocations.classList.contains(CLASSES.hide)) {
						locationsViewAllBtn.textContent = "View all";
					} else {
						locationsViewAllBtn.textContent = "Hide all";
					}
				});
			}
		}
		const toggleRegionSelections = () => {
			let regionsViewAllBtn = document.getElementById(ID.regionsViewAllBtn) as HTMLElement;
			let selectedRegions = document.getElementById(ID.selectedRegions) as HTMLElement;

			if (regionsViewAllBtn != null && selectedRegions != null) {
				if (selectedRegions.classList.contains(CLASSES.hide)) {
					regionsViewAllBtn.textContent = "View all";
				} else {
					regionsViewAllBtn.textContent = "Hide all";
				}
				regionsViewAllBtn.addEventListener("click", (el) => {
					el.preventDefault();
					selectedRegions.classList.toggle(CLASSES.hide);
					if (selectedRegions.classList.contains(CLASSES.hide)) {
						regionsViewAllBtn.textContent = "View all";
					} else {
						regionsViewAllBtn.textContent = "Hide all";
					}
				});
			}
		}

		const editAddress = () => {
			document.addEventListener("click", (el) => {
				let element = el.target as HTMLInputElement;
				if (element.id.startsWith(ID.editLocationButton)) {
					el.preventDefault();
					let targetDivEdit = document.getElementById(ID.editLocation) as HTMLElement;
					var locationCancelEdit = document.getElementById(ID.cancelLocationEdit) as HTMLElement;
					var locationSubmitEdit = document.getElementById(ID.submitLocationEdit) as HTMLElement;
					let searchAddressEdit = document.getElementById(ID.searchAddressEdit) as HTMLInputElement;
					let editName = document.getElementById(ID.editLocationName) as HTMLInputElement;
					let editAddress = document.getElementById(ID.editLocationAddress) as HTMLInputElement;
					let editCity = document.getElementById(ID.editLocationCity) as HTMLInputElement;
					let editPostcode = document.getElementById(ID.editLocationPostcode) as HTMLInputElement;
					let editLat = document.getElementById(ID.editLocationLatitude) as HTMLInputElement;
					let editLon = document.getElementById(ID.editLocationLongitude) as HTMLInputElement;
					let editLocationId = document.getElementById(ID.editLocationId) as HTMLInputElement;

					searchAddressEdit.value = "";
					editName.value = element.dataset.name as string;
					editAddress.value = element.dataset.address as string;
					editCity.value = element.dataset.city as string;
					editPostcode.value = element.dataset.postcode as string;
					editLocationId.value = element.dataset.id as string;
					editLat.value = "";
					editLon.value = "";

					if (targetDivEdit.classList.contains(CLASSES.hide)) {
						Aliss.Helpers.toggleClass(targetDivEdit, CLASSES.hide);
					}
					
					let orgId = document.getElementById(ID.orgId) as HTMLInputElement;
					
					if (locationCancelEdit != null) {
						locationCancelEdit.onclick = function (e) {
							e.preventDefault();
							searchAddressEdit.value = "";
							editName.value = "";
							editAddress.value = "";
							editCity.value = "";
							editPostcode.value = "";
							editLat.value = "";
							editLon.value = "";
							editLocationId.value = "";
							Aliss.Helpers.toggleClass(targetDivEdit, CLASSES.hide);
						}
					}
				}
			});
		}

		init();
	}
}

export default Locations;

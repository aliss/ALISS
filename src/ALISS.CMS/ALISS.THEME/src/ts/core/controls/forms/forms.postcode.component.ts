import { post } from "jquery";

/* eslint-disable no-extra-boolean-cast */
class Postcode {
	constructor() {
		enum ID {
			searchSubmit = 'search-button-postcode',
			searchForm = 'search-form-postcode',
			searchInput = 'search-postcode',
			selectedTitle = 'title-categories-selected',
			searchClear = 'search-clear',
			filterPostcodeHidden = 'postcode',
			filterSubmit = 'search-filters-submit-form',
			queryInput = 'filter-what-entry',
			queryHidden = 'Query',
			sort = 'SortDistance'
		}

		var formSubmit = document.getElementById(ID.searchSubmit) as HTMLButtonElement;
		var form = document.getElementById(ID.searchForm) as HTMLFormElement;
		var formInput = document.getElementById(ID.searchInput) as HTMLInputElement;
		var formMessage = document.getElementById("srch") as HTMLDivElement;
		var formClear = document.getElementById(ID.searchClear) as HTMLSpanElement;
        let postcodeHidden = document.getElementById(ID.filterPostcodeHidden) as HTMLInputElement;
		var typingTimer: ReturnType<typeof setTimeout>;
		let queryInput = document.getElementById(ID.queryInput) as HTMLInputElement;
		var sortRadio = document.getElementById(ID.sort) as HTMLInputElement;

		const init = () => {
			if (document.getElementById(ID.searchInput)) {
				formInput.addEventListener('focus', e => {
					postcodeAutoComplete();
				});
			}
			if (document.getElementById(ID.searchSubmit)) {
				formSubmit.addEventListener('click', (e) => {
					e.preventDefault();
					if (queryInput && document.activeElement == queryInput) {
						let form = document.getElementById(ID.filterSubmit) as HTMLButtonElement;
						let queryHidden = document.getElementById(ID.queryHidden) as HTMLInputElement;
						queryHidden.value = queryInput.value;
						form.click();
					}
					else{
						getGeocodedLocation();
					}
				});
			}

			if (document.getElementById(ID.searchClear)) {
				formClear.addEventListener('click', e => {
					var input = formClear.previousElementSibling as HTMLInputElement;
					input.value = "";
        			input.focus();
				})
			}
		}

		const postcodeAutoComplete = () => {
			var options = {
				componentRestrictions: {
					country: "gb"
				}
			};
			const autocomplete = new google.maps.places.Autocomplete(formInput, options);

			autocomplete.addListener("place_changed", () => {
				const place = autocomplete.getPlace();
				console.log(place);
			});
		};

		const updateAndSubmit = (postcode = '') => {
			if (!!postcode) {
				formInput.value = postcode;
			}

			if(postcodeHidden && !!postcode){
				postcodeHidden.value = postcode;
				sortRadio.checked = true;
				let form = document.getElementById(ID.filterSubmit) as HTMLButtonElement;
				// form.click();
			}
			else if (form){
				form.submit();
			}
		};

		const getPostcode = async (lat = '', long = '') => {
			if (!!lat && !!long) {
				const response = await fetch('https://api.postcodes.io/postcodes?lat=' + lat + '&lon=' + long + '&radius=2000');
				const myJson = await response.json();
				return myJson.result ? myJson.result[0].postcode : "";
			}
		};

		const autocompletePostcode = async (query = '') => {
			if (!!query) {
				const response = await fetch('https://api.postcodes.io/postcodes/' + query + '/autocomplete');
				const myJson = await response.json();
				return myJson.result ? myJson.result[0] : "";
			}
		};

		const getGeocodedLocation = () => {
			if (formInput.value === "") {
				updateAndSubmit();
			} else {
				var query = formInput.value;
				if (query.length >= 2) {
					var regExp = /(^|\s+)(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2})($|\s+))/gi;
					if (regExp.test(query)) {
						query = query.replace(/\s/g, '');
						query = query.replace(/(?=.{3}$)/, ' ');
						query = query.toUpperCase();
						updateAndSubmit(query);
					} else {
						autocompletePostcode(query)
							.then(postcode => {
								if (!!postcode) {
									updateAndSubmit(postcode);
								} else {
									query = formInput.value;
									console.log("query = " + query);
									if (regExp.test(query)) {
										query = query.replace(/\s/g, '');
										query = query.replace(/(?=.{3}$)/, ' ');
										query = query.toUpperCase();
										updateAndSubmit(query);
									} else {
										let geocoder = new window.google.maps.Geocoder();
										let searchTerm = formInput.value;
										if (searchTerm.toLowerCase().includes('shetland')) {
											searchTerm = 'Lerwick';
										} else if (searchTerm.toLowerCase().includes('orkney')) {
											searchTerm = 'Kirkwall';
										} else if (searchTerm.toLowerCase().includes('arran')) {
											searchTerm = 'Brodick';
										} else if (searchTerm.toLowerCase().includes('lewis') || searchTerm.toLowerCase().includes('harris')) {
											searchTerm = 'Stornoway';
										}
										geocoder.geocode({
											'address': searchTerm + ', Scotland'
										}, (res: any, status: any) => {
											if (status === window.google.maps.GeocoderStatus.OK) {
												if(res[0].formatted_address === 'Scotland, UK'){
													updateAndSubmit(query);
												}
												else{
													console.log(`${res[0].geometry.location.lat()}, ${res[0].geometry.location.lng()}`);
													getPostcode(res[0].geometry.location.lat(), res[0].geometry.location.lng())
														.then(postcode => {
															updateAndSubmit(!!postcode ? postcode : query);
														});
												}
											} else {
												if (status === window.google.maps.GeocoderStatus.ZERO_RESULTS) {
													updateAndSubmit(query);
												}
											}
										});
									}
								}
							});
					}
				} else {
					showError("Please ensure you search by a address, placename or postcode.");
				}
			}
		};

		const showError = (errorMessage = '') => {
			if (errorMessage) {
				formMessage.innerHTML = "<div class='error'><h2>Error</h2><p>" + errorMessage + "</p></div>";
			} else {
				formMessage.innerHTML = "";
			}
		};

		init();
	}
}

export default Postcode;

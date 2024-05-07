import { post } from 'jquery';
import Aliss from '../../';

class Locations {
	constructor() {

		enum CLASSES {
			error = "aliss-form__group--error"
		}

		enum IDS {
			selections = "selected-locations",
			selectList = "locations",
			addNewLocation = "add-new-location",
			newLocation = "new-location",
			submitLocation = "submit-location",
			newLocationName = "NewLocationName",
			newLocationAddress = "NewLocationAddress",
			newLocationCity = "NewLocationCity",
			newLocationPostcode = "NewLocationPostcode",
			orgId = "OrganisationId"
		}

		const init = () => {
			let link = document.getElementById(IDS.addNewLocation) as HTMLElement;
			let targetDiv = document.getElementById(IDS.newLocation) as HTMLElement;
			if (typeof(link) != 'undefined' && link != null) {
				link.onclick = (e) => {
					e.preventDefault();
					Aliss.Helpers.toggleClass(targetDiv, "hide");
				}
				var locationSubmit = document.getElementById(IDS.submitLocation) as HTMLElement;
				let name = document.getElementById(IDS.newLocationName) as HTMLInputElement;
				let address = document.getElementById(IDS.newLocationAddress) as HTMLInputElement;
				let city = document.getElementById(IDS.newLocationCity) as HTMLInputElement;
				let postcode = document.getElementById(IDS.newLocationPostcode) as HTMLInputElement;
				let orgId = document.getElementById(IDS.orgId) as HTMLInputElement;
				var postcodeRegex = /^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))\s?[0-9][A-Za-z]{2})$/;
				var postcodeCheck = postcodeRegex.test(postcode.value);
					
				locationSubmit.onclick = function (e) {
					e.preventDefault();
					let data = {
						OrganisationId: orgId.value,
						Name: name.value,
						Address: address.value,
						City: city.value,
						Postcode: postcode.value
					};

					console.log("postcode: " + postcodeCheck);
					
					if(address.value == "" || city.value == "" || postcode.value == "" || postcodeCheck) {
						address.value == "" ? Aliss.Helpers.addClass(address.parentNode, CLASSES.error) : null;
						city.value == "" ? Aliss.Helpers.addClass(city.parentNode, CLASSES.error) : null;
						postcode.value == "" || postcodeCheck ? Aliss.Helpers.addClass(postcode.parentNode, CLASSES.error) : null;
					} else {
						Aliss.Helpers.sendJson("/Organisation/AddServiceLocation", data, (response: any) => {
							let result = JSON.parse(response);					
							let selectBox = document.getElementById(IDS.selectList) as HTMLSelectElement;
							let newOption = document.createElement(Aliss.Enums.Elements.Option);
							newOption.value = result.LocationId;
							newOption.text = result.Address + ", " + result.City  + ", " + result.Postcode;
							selectBox.appendChild(newOption);
							setTimeout(()=>{
								selectBox.selectedIndex = selectBox.options.length - 1;
								Aliss.Helpers.trigger(selectBox, Aliss.Enums.Events.Change);
								Aliss.Helpers.addClass(targetDiv, "hide");
								name.value = "";
								address.value = "";
								city.value = "";
								postcode.value = "";
							}, 300);
						});
					}
				}
			}
		}

		init();
	}
}

export default Locations;
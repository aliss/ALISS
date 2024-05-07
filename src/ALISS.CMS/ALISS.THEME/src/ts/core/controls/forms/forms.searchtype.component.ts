
import Aliss from '../../index';

class SearchType {
	constructor() {

		enum ID {
			radioPostcode = 'searchType1',
			radioOrgs = 'searchType2',
			searchPostcode = 'search-form-postcode',
			searchOrgs = 'search-form-organisations'
		}

		enum SELECTOR {
			checkbox = 'search-type-selector'
		}

		let getPostcodeCheckbox = document.getElementById(ID.radioPostcode) as HTMLInputElement;
		let getOrgCheckbox = document.getElementById(ID.radioOrgs) as HTMLInputElement;
		let getsearch = document.getElementById(ID.searchPostcode) as HTMLFormElement;
		let getorgs = document.getElementById(ID.searchOrgs) as HTMLFormElement;
		
		const init = () => {
			
			if (typeof(getPostcodeCheckbox) != 'undefined' && getPostcodeCheckbox != null) {

				getPostcodeCheckbox.addEventListener(Aliss.Enums.Events.Change, () => {
					showPostcode();
				});
				getOrgCheckbox.addEventListener(Aliss.Enums.Events.Change, () => {
					showOrgs();
				});
			}
		}

		const showPostcode = () => {
			getOrgCheckbox.checked = false;
			getPostcodeCheckbox.checked = true;
			getsearch.classList.remove("hide");
			getorgs.classList.add("hide");
			getsearch.setAttribute("aria-hidden", "false");
			getorgs.setAttribute("aria-hidden", "true");
		}
		
		const showOrgs = () => {			
			getPostcodeCheckbox.checked = false;
			getOrgCheckbox.checked = true;
			getsearch.classList.add("hide");
			getorgs.classList.remove("hide");
			getsearch.setAttribute("aria-hidden", "true");
			getorgs.setAttribute("aria-hidden", "false");
		}

		init();
	}
}

export default SearchType;
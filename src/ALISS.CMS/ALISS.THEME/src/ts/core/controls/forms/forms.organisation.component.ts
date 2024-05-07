import Aliss from '../../';

class OrganisationRepresentative {
	constructor() {

		enum ID {
			OrganisationRepresentative = 'OrganisationRepresentative',
			RepresentativeInfo = 'representative-info'
		}

		enum ATTRIBUTES {
			AriaHidden = 'aria-hidden'
		}

		const init = () => {
			var orgRep =  document.getElementById(ID.OrganisationRepresentative) as HTMLInputElement;
			if (typeof(orgRep) != 'undefined' && orgRep != null) {
				orgRep.addEventListener("change", () => {
					var repInfo =  document.getElementById(ID.RepresentativeInfo) as HTMLElement;
					Aliss.Helpers.toggleClass(repInfo, 'hide');
					if (orgRep.checked) {
						repInfo.setAttribute(ATTRIBUTES.AriaHidden, "false");
					} else {
						repInfo.setAttribute(ATTRIBUTES.AriaHidden, "true");
					}
				});
			}
		}

		init();
	}
}

export default OrganisationRepresentative;
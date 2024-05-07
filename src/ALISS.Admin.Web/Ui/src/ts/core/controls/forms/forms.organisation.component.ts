import Aliss from '../../';

class OrganisationRepresentative {
	constructor() {

		enum ID {
			OrganisationRepresentative = 'OrganisationRepresentative',
			OrganisationRepresentativeInfo = 'organisation-representative-info',
			OrganisationRepresentativeRole = 'OrganisationRepresentativeRole',
			OrganisationRepresentativeName = 'OrganisationRepresentativeName',
			OrganisationRepresentativePhone = 'OrganisationRepresentativePhone',
			OrganisationAcceptDataStandards = 'OrganisationAcceptDataStandards'
		}

		enum ATTRIBUTES {
			AriaHidden = 'aria-hidden'
		}

		const init = () => {
			var orgRep =  document.getElementById(ID.OrganisationRepresentative) as HTMLInputElement;
			if (typeof(orgRep) != 'undefined' && orgRep != null) {
				orgRep.addEventListener("change", () => {
					var repInfo = document.getElementById(ID.OrganisationRepresentativeInfo) as HTMLElement;
					var repRole = document.getElementById(ID.OrganisationRepresentativeRole) as HTMLInputElement;
					var repName = document.getElementById(ID.OrganisationRepresentativeName) as HTMLInputElement;
					var repPhone = document.getElementById(ID.OrganisationRepresentativePhone) as HTMLInputElement;
					var acceptData = document.getElementById(ID.OrganisationAcceptDataStandards) as HTMLInputElement;
					Aliss.Helpers.toggleClass(repInfo, 'hide');
					if (orgRep.checked) {
						repInfo.setAttribute(ATTRIBUTES.AriaHidden, "false");
					} else {
						repInfo.setAttribute(ATTRIBUTES.AriaHidden, "true");
						repRole.value = "";
						repName.value = "";
						repPhone.value = "";
						acceptData.checked = false;
					}
				});
			}
		}

		init();
	}
}

export default OrganisationRepresentative;
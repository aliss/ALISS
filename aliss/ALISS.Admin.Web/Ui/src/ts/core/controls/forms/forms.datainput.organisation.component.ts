import Aliss from '../../';

class DataInputOrganisation {
	constructor() {

		enum ID {
			OrganisationRepresentative_yes = 'OrganisationRepresentative-Yes',
			OrganisationRepresentative_no = 'OrganisationRepresentative-No',
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
			var orgRep_yes =  document.getElementById(ID.OrganisationRepresentative_yes) as HTMLInputElement;
			var orgRep_no =  document.getElementById(ID.OrganisationRepresentative_no) as HTMLInputElement;
			
			if (typeof(orgRep_yes) != 'undefined' && orgRep_yes != null &&
				typeof(orgRep_no) != 'undefined' && orgRep_no != null) {

				if(orgRep_yes.checked)
				{
					var repInfo = document.getElementById(ID.OrganisationRepresentativeInfo) as HTMLElement;
					Aliss.Helpers.toggleClass(repInfo, 'hide');
					repInfo.setAttribute(ATTRIBUTES.AriaHidden, "false");
				}
				orgRep_yes.addEventListener("change", () => {
					var repInfo = document.getElementById(ID.OrganisationRepresentativeInfo) as HTMLElement;
					var repRole = document.getElementById(ID.OrganisationRepresentativeRole) as HTMLInputElement;
					var repName = document.getElementById(ID.OrganisationRepresentativeName) as HTMLInputElement;
					var repPhone = document.getElementById(ID.OrganisationRepresentativePhone) as HTMLInputElement;
					var acceptData = document.getElementById(ID.OrganisationAcceptDataStandards) as HTMLInputElement;
					Aliss.Helpers.toggleClass(repInfo, 'hide');
					if (orgRep_yes.checked) {
						repInfo.setAttribute(ATTRIBUTES.AriaHidden, "false");
					} else {
						repInfo.setAttribute(ATTRIBUTES.AriaHidden, "true");
						repRole.value = "";
						repName.value = "";
						repPhone.value = "";
						acceptData.checked = false;
					}
				});

				orgRep_no.addEventListener("change", () => {
					var repInfo = document.getElementById(ID.OrganisationRepresentativeInfo) as HTMLElement;
					var repRole = document.getElementById(ID.OrganisationRepresentativeRole) as HTMLInputElement;
					var repName = document.getElementById(ID.OrganisationRepresentativeName) as HTMLInputElement;
					var repPhone = document.getElementById(ID.OrganisationRepresentativePhone) as HTMLInputElement;
					var acceptData = document.getElementById(ID.OrganisationAcceptDataStandards) as HTMLInputElement;
					Aliss.Helpers.toggleClass(repInfo, 'hide');
					if (!orgRep_no.checked) {
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

export default DataInputOrganisation;
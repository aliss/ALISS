import Aliss from '../../';

class ServiceRepresentative {
	constructor() {

		enum ID {
			ServiceRepresentative = 'ServiceRepresentative',
			ServiceRepresentative_yes = 'ServiceRepresentative-Yes',
			ServiceRepresentative_no = 'ServiceRepresentative-No',
			ServiceRepresentativeInfo = 'service-representative-info',
			ServiceRepresentativeRole = 'ServiceRepresentativeRole',
			ServiceRepresentativeName = 'ServiceRepresentativeName',
			ServiceRepresentativePhone = 'ServiceRepresentativePhone',
			ServiceAcceptDataStandards = 'ServiceAcceptDataStandards',

			ServiceName = 'ServiceName',
			ServiceNameCount = 'ServiceName_Count',
			OrganisationName = 'organisationModel_OrganisationName',
			UseOrganisationName = 'UseOrganisationName',

			ServiceDescription = 'editor',
			ServiceDescriptionCount = 'ServiceDescription_Count',
			ServiceDescriptionOuter = 'editorDiv',
			OrganisationDescription = 'organisationModel_OrganisationDescription',
			UseOrganisationDescription = 'UseOrganisationDescription',

			UseAllOrganisationContactDetails = 'UseAllOrganisationContactDetails',

			ServicePhoneNumber = 'PhoneNumber',
			OrganisationPhoneNumber = 'OrganisationPhoneNumber',
			UseOrganisationPhoneNumber = 'UseOrganisationPhoneNumber',

			ServiceEmail = 'Email',
			ServiceEmailCount = 'Email_Count',
			OrganisationEmail = 'OrganisationEmail',
			UseOrganisationEmail = 'UseOrganisationEmail',

			ServiceUrl = 'Url',
			ServiceUrlCount = 'Url_Count',
			OrganisationUrl = 'OrganisationUrl',
			UseOrganisationUrl = 'UseOrganisationUrl',

			ServiceFacebook = 'Facebook',
			ServiceFacebookCount = 'Facebook_Count',
			OrganisationFacebook = 'OrganisationFacebook',
			UseOrganisationFacebook = 'UseOrganisationFacebook',

			ServiceTwitter = 'Twitter',
			ServiceTwitterCount = 'Twitter_Count',
			OrganisationTwitter = 'OrganisationTwitter',
			UseOrganisationTwitter = 'UseOrganisationTwitter',

			ServiceInstagram = 'Instagram',
			ServiceInstagramCount = 'Instagram_Count',
			OrganisationInstagram = 'OrganisationInstagram',
			UseOrganisationInstagram = 'UseOrganisationInstagram',

			slug = 'Slug',
			slugDisplay = 'SlugDisplay',
		}

		enum ATTRIBUTES {
			AriaHidden = 'aria-hidden'
		}

		const init = () => {
			var serviceRep_yes = document.getElementById(ID.ServiceRepresentative_yes) as HTMLInputElement;
			var serviceRep_no = document.getElementById(ID.ServiceRepresentative_no) as HTMLInputElement;
			var serviceRep = document.getElementById(ID.ServiceRepresentative) as HTMLInputElement;

			if (typeof (serviceRep_yes) != 'undefined' && serviceRep_yes != null &&
				typeof (serviceRep_no) != 'undefined' && serviceRep_no != null) {
				
				if (serviceRep_yes.checked) {
					var repInfo = document.getElementById(ID.ServiceRepresentativeInfo) as HTMLElement;
					Aliss.Helpers.toggleClass(repInfo, 'hide');
					repInfo.setAttribute(ATTRIBUTES.AriaHidden, "false");
				}

				serviceRep_yes.addEventListener("change", () => {
					var repInfo = document.getElementById(ID.ServiceRepresentativeInfo) as HTMLElement;
					var repRole = document.getElementById(ID.ServiceRepresentativeRole) as HTMLInputElement;
					var repName = document.getElementById(ID.ServiceRepresentativeName) as HTMLInputElement;
					var repPhone = document.getElementById(ID.ServiceRepresentativePhone) as HTMLInputElement;
					var acceptData = document.getElementById(ID.ServiceAcceptDataStandards) as HTMLInputElement;
					Aliss.Helpers.toggleClass(repInfo, 'hide');
					if (serviceRep_yes.checked) {
						repInfo.setAttribute(ATTRIBUTES.AriaHidden, "false");
					} else {
						repInfo.setAttribute(ATTRIBUTES.AriaHidden, "true");
						repRole.value = "";
						repName.value = "";
						repPhone.value = "";
						acceptData.checked = false;
					}
				});

				serviceRep_no.addEventListener("change", () => {
					var repInfo = document.getElementById(ID.ServiceRepresentativeInfo) as HTMLElement;
					var repRole = document.getElementById(ID.ServiceRepresentativeRole) as HTMLInputElement;
					var repName = document.getElementById(ID.ServiceRepresentativeName) as HTMLInputElement;
					var repPhone = document.getElementById(ID.ServiceRepresentativePhone) as HTMLInputElement;
					var acceptData = document.getElementById(ID.ServiceAcceptDataStandards) as HTMLInputElement;
					Aliss.Helpers.toggleClass(repInfo, 'hide');
					if (!serviceRep_no.checked) {
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

		const updateSlugToOrganisation = () => {
			var categoryElement = document.getElementById(ID.OrganisationName) as HTMLInputElement;
			var categoryName = categoryElement.value;
			categoryName = categoryName.replace(/^\s+|\s+$/g, '');
			categoryName = categoryName.toLowerCase();

			// remove accents, swap ñ for n, etc
			var from = "àáãäâèéëêìíïîòóöôùúüûñç·/_,:;";
			var to = "aaaaaeeeeiiiioooouuuunc------";
			for (var i = 0, l = from.length; i < l; i++) {
				categoryName = categoryName.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
			}

			categoryName = categoryName.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
				.replace(/\s+/g, '-') // collapse whitespace and replace by -
				.replace(/-+/g, '-'); // collapse dashes

			var slug = document.getElementById(ID.slug) as HTMLInputElement;
			var slugDisplay = document.getElementById(ID.slugDisplay) as HTMLInputElement;
			slug.value = categoryName;
			slugDisplay.value = categoryName;
		}

		const clearSlug = () => {
			var slug = document.getElementById(ID.slug) as HTMLInputElement;
			var slugDisplay = document.getElementById(ID.slugDisplay) as HTMLInputElement;
			slug.value = "";
			slugDisplay.value = "";
		}

		init();

		//Use Org Values
		var useOrgName = document.getElementById(ID.UseOrganisationName) as HTMLInputElement
		var serName = document.getElementById(ID.ServiceName) as HTMLInputElement
		var orgName = document.getElementById(ID.OrganisationName) as HTMLInputElement
		
		var useOrgDesc = document.getElementById(ID.UseOrganisationDescription) as HTMLInputElement
		var serDesc = document.getElementById(ID.ServiceDescription)?.firstChild as HTMLDivElement;
		var serDescCount = document.getElementById(ID.ServiceDescriptionCount) as HTMLSpanElement;
		var orgDesc = document.getElementById(ID.OrganisationDescription) as HTMLInputElement
		var serDescOuter = document.getElementById(ID.ServiceDescriptionOuter) as HTMLDivElement

		var useOrgContactDetails = document.getElementById(ID.UseAllOrganisationContactDetails) as HTMLInputElement

		var useOrgPhoneNumber = document.getElementById(ID.UseOrganisationPhoneNumber) as HTMLInputElement
		var serPhoneNumber = document.getElementById(ID.ServicePhoneNumber) as HTMLInputElement
		var orgPhoneNumber = document.getElementById(ID.OrganisationPhoneNumber) as HTMLInputElement

		var useOrgEmail = document.getElementById(ID.UseOrganisationEmail) as HTMLInputElement
		var serEmail = document.getElementById(ID.ServiceEmail) as HTMLInputElement
		var orgEmail = document.getElementById(ID.OrganisationEmail) as HTMLInputElement

		var useOrgUrl = document.getElementById(ID.UseOrganisationUrl) as HTMLInputElement
		var serUrl = document.getElementById(ID.ServiceUrl) as HTMLInputElement
		var orgUrl = document.getElementById(ID.OrganisationUrl) as HTMLInputElement

		var useOrgFacebook = document.getElementById(ID.UseOrganisationFacebook) as HTMLInputElement
		var serFacebook = document.getElementById(ID.ServiceFacebook) as HTMLInputElement
		var orgFacebook = document.getElementById(ID.OrganisationFacebook) as HTMLInputElement

		var useOrgTwitter = document.getElementById(ID.UseOrganisationTwitter) as HTMLInputElement
		var serTwitter = document.getElementById(ID.ServiceTwitter) as HTMLInputElement
		var orgTwitter = document.getElementById(ID.OrganisationTwitter) as HTMLInputElement

		var useOrgInstagram = document.getElementById(ID.UseOrganisationInstagram) as HTMLInputElement
		var serInstagram = document.getElementById(ID.ServiceInstagram) as HTMLInputElement
		var orgInstagram = document.getElementById(ID.OrganisationInstagram) as HTMLInputElement

		if (useOrgName) {
			if(useOrgName.checked)
			{	
				serName.disabled = true;
				serName.value = orgName.value;
				updateSlugToOrganisation();
			}
		
			useOrgName.addEventListener("change", () => {
				serName.disabled = !serName.disabled;

				if(serName.disabled)
				{
					serName.value = orgName.value;
					updateSlugToOrganisation();
				}
				else
				{
					serName.value = "";
					clearSlug()
				}
			})
		}

		if (useOrgDesc) {
			if(useOrgDesc.checked)
			{
				serDescOuter.style.pointerEvents = "none";
				serDesc.style.color = "grey";
				serDescCount.innerText = "";
			}

			useOrgDesc.addEventListener("change", () => {
				if(useOrgDesc.checked)
				{
					serDescOuter.style.pointerEvents = "none";
					serDesc.style.color = "grey";
					serDesc.innerHTML = orgDesc.value;
					serDescCount.innerText = "";
				}
				else
				{
					serDescOuter.style.pointerEvents = "auto";
					serDesc.style.color = "black";
					serDesc.textContent = "";
					serDescCount.innerText = "1000 character(s) remaining";
				}
			})
		}

		if (useOrgContactDetails) {
			if(useOrgContactDetails.checked)
			{
				serPhoneNumber.disabled = true;
				serPhoneNumber.value = orgPhoneNumber.value;
				useOrgPhoneNumber.disabled = true;
				useOrgPhoneNumber.checked = true;

				serEmail.disabled = true;
				serEmail.value = orgEmail.value;
				useOrgEmail.disabled = true;
				useOrgEmail.checked = true;

				serUrl.disabled = true;
				serUrl.value = orgUrl.value;
				useOrgUrl.disabled = true;
				useOrgUrl.checked = true;

				serFacebook.disabled = true;
				serFacebook.value = orgFacebook.value;
				useOrgFacebook.disabled = true;
				useOrgFacebook.checked = true;

				serTwitter.disabled = true;
				serTwitter.value = orgTwitter.value;
				useOrgTwitter.disabled = true;
				useOrgTwitter.checked = true;

				serInstagram.disabled = true;
				serInstagram.value = orgInstagram.value;
				useOrgInstagram.disabled = true;
				useOrgInstagram.checked = true;
			}

			useOrgContactDetails.addEventListener("change", () => {
				if(useOrgContactDetails.checked)
				{
					serPhoneNumber.disabled = true;
					serPhoneNumber.value = orgPhoneNumber.value;
					useOrgPhoneNumber.disabled = true;
					useOrgPhoneNumber.checked = true;

					serEmail.disabled = true;
					serEmail.value = orgEmail.value;
					useOrgEmail.disabled = true;
					useOrgEmail.checked = true;

					serUrl.disabled = true;
					serUrl.value = orgUrl.value;
					useOrgUrl.disabled = true;
					useOrgUrl.checked = true;

					serFacebook.disabled = true;
					serFacebook.value = orgFacebook.value;
					useOrgFacebook.disabled = true;
					useOrgFacebook.checked = true;

					serTwitter.disabled = true;
					serTwitter.value = orgTwitter.value;
					useOrgTwitter.disabled = true;
					useOrgTwitter.checked = true;

					serInstagram.disabled = true;
					serInstagram.value = orgInstagram.value;
					useOrgInstagram.disabled = true;
					useOrgInstagram.checked = true;
				}
				else
				{
					serPhoneNumber.disabled = false;
					serPhoneNumber.value = "";
					useOrgPhoneNumber.disabled = false;
					useOrgPhoneNumber.checked = false;

					serEmail.disabled = false;
					serEmail.value = "";
					useOrgEmail.disabled = false;
					useOrgEmail.checked = false;

					serUrl.disabled = false;
					serUrl.value = "";
					useOrgUrl.disabled = false;
					useOrgUrl.checked = false;

					serFacebook.disabled = false;
					serFacebook.value = "";
					useOrgFacebook.disabled = false;
					useOrgFacebook.checked = false;

					serTwitter.disabled = false;
					serTwitter.value = "";
					useOrgTwitter.disabled = false;
					useOrgTwitter.checked = false;

					serInstagram.disabled = false;
					serInstagram.value = "";
					useOrgInstagram.disabled = false;
					useOrgInstagram.checked = false;
				}
			})
		}

		if (useOrgPhoneNumber) {
			if(useOrgPhoneNumber.checked)
			{
				serPhoneNumber.disabled = true;
				serPhoneNumber.value = orgPhoneNumber.value;
			}

			useOrgPhoneNumber.addEventListener("change", () => {
				serPhoneNumber.disabled = !serPhoneNumber.disabled;

				if(serPhoneNumber.disabled)
				{
					serPhoneNumber.value = orgPhoneNumber.value;
				}
				else
				{
					serPhoneNumber.value = "";
				}
			})
		}

		if (useOrgEmail) {
			if(useOrgEmail.checked)
			{
				serEmail.disabled = true;
				serEmail.value = orgEmail.value;
			}
			
			useOrgEmail.addEventListener("change", () => {
				serEmail.disabled = !serEmail.disabled;

				if(serEmail.disabled)
				{
					serEmail.value = orgEmail.value;
				}
				else
				{
					serEmail.value = "";
				}
			})
		}

		if (useOrgUrl) {
			if(useOrgUrl.checked)
			{
				serUrl.disabled = true;
				serUrl.value = orgUrl.value;
			}

			useOrgUrl.addEventListener("change", () => {
				serUrl.disabled = !serUrl.disabled;

				if(serUrl.disabled)
				{
					serUrl.value = orgUrl.value;
				}
				else
				{
					serUrl.value = "";
				}
			})
		}

		if (useOrgFacebook) {
			if(useOrgFacebook.checked)
			{
				serFacebook.disabled = true;
				serFacebook.value = orgFacebook.value;
			}

			useOrgFacebook.addEventListener("change", () => {
				serFacebook.disabled = !serFacebook.disabled;

				if(serFacebook.disabled)
				{
					serFacebook.value = orgFacebook.value;
				}
				else
				{
					serFacebook.value = "";
				}
			})
		}

		if (useOrgTwitter) {
			if(useOrgTwitter.checked){
				serTwitter.disabled = true;
				serTwitter.value = orgTwitter.value;
			}

			useOrgTwitter.addEventListener("change", () => {
				serTwitter.disabled = !serTwitter.disabled;

				if(serTwitter.disabled)
				{
					serTwitter.value = orgTwitter.value;
				}
				else
				{
					serTwitter.value = "";
				}
			})
		}

		if (useOrgInstagram) {
			if(useOrgInstagram.checked){
				serInstagram.disabled = true;
				serInstagram.value = orgInstagram.value;
			}

			useOrgInstagram.addEventListener("change", () => {
				serInstagram.disabled = !serInstagram.disabled;

				if(serInstagram.disabled)
				{
					serInstagram.value = orgInstagram.value;
				}
				else
				{
					serInstagram.value = "";
				}
			})
		}
	}
}

export default ServiceRepresentative;
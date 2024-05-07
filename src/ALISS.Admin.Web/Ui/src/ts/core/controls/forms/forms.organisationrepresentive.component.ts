class OrganisationRepresentive {
	constructor() {

		enum ID {
			checkbox = 'OrganisationRepresentative',
			content = 'organisation-representative-info'
		}

		const init = () => {
			var getCheckbox = document.getElementById(ID.checkbox) as HTMLInputElement;
			var getContent = document.getElementById(ID.content) as HTMLElement;

			if (typeof(getCheckbox) != 'undefined' && getCheckbox != null) {
				if(getCheckbox.checked) {
					getContent.classList.remove("hide")
					getContent.setAttribute("aria-hidden", "false");
				} else {                
					getContent.classList.add("hide")
					getContent.setAttribute("aria-hidden", "true");
				}
			}
		}

		init();
	}
}

export default OrganisationRepresentive;
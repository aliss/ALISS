
import Aliss from '../../index';

class PageNavigation {
	constructor() {

		enum ID {
			navigationInput = 'page-navigation-input',
			navigationButton = 'page-navigation-button',
			navigationUrl = 'page-navigation-url',
			navigationMaxPages = 'page-navigation-max-pages',
			tooltip = 'page-navigation-input-tooltip'
		}

		enum ClASSES {
			tooltipActive = 'tooltip--active'
		}

		let pageNavigationInput = document.getElementById(ID.navigationInput) as HTMLInputElement;
		let pageNavigationButton = document.getElementById(ID.navigationButton) as HTMLButtonElement;
		let pageNavigationUrl = document.getElementById(ID.navigationUrl) as HTMLInputElement;
		let pageNavigationMaxPages = document.getElementById(ID.navigationMaxPages) as HTMLInputElement;
		let tooltip = document.getElementById(ID.tooltip) as HTMLSpanElement;

		let pageNumber : number;
		if(pageNavigationInput) {
			pageNumber = Number(pageNavigationInput.value);
		}
		
		let maxPages : number 
		if(pageNavigationMaxPages){
			maxPages = Number(pageNavigationMaxPages.value);
		}
		
		const init = () => {
			if(pageNavigationButton && pageNavigationInput) {
				pageNavigationInput.addEventListener('input', () => {
					if(pageNavigationInput.value == "" || isNaN(pageNumber)) {
						pageNumber = Number(pageNavigationInput.placeholder);
						tooltip.classList.add(ClASSES.tooltipActive);
						setTimeout(function() {
							tooltip.classList.remove(ClASSES.tooltipActive);
						}, 5000);
					} else {
						tooltip.classList.remove(ClASSES.tooltipActive);
					}
				});

				pageNavigationButton.addEventListener('click', e => {
					e.preventDefault();
					pageNumber = Number(pageNavigationInput.value);
					if(pageNavigationInput.value == "" || isNaN(pageNumber)){
						pageNumber = Number(pageNavigationInput.placeholder);
					} 
					else if (pageNumber < 1) {
						pageNumber = 1;
					} else if (pageNumber > maxPages) {
						pageNumber = maxPages;
					}
					window.location.href = '?page=' + pageNumber + pageNavigationUrl.value;
				});
			}
		}

		init();
	}
}

export default PageNavigation;
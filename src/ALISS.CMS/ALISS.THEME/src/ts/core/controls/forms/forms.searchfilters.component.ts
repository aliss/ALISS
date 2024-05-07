/** @format */

import GetCategoryData from "../components/site-get-category";

class SearchFilters {
	constructor() {
		enum CLASSES {
			SearchFilter = 'service-search-filter-radio',
			FiltersToggleActive = 'aliss-accordion__trigger--active',
			FiltersPanelActive = 'aliss-accordion__content--active',
			hide = 'hide'
		}

		enum ID {
			FormSubmit = 'search-filters-submit-form',
            CustomDistance = 'Radius4',
			CustomeDistanceInput = "custom-radius-input",
            CustomDistanceSubmit = 'custom-distance-button',
            Query = 'filter-what-entry',
            QuerySubmit = 'filter-what-submit',
            QueryHidden = 'Query',
			RemoveQuery = 'selected-what',
			PostcodeHidden = 'postcode',
			RemovePostcode = 'selected-postcode',
			FiltersToggle = 'search-filters-toggle',
			FiltersPanel = 'filter-results-accordion',
			ViewResultsBtn = 'search-filters-view-results'
		}	

		enum INPUT {
			searchClear = "span[id=search-clear]"
		}

		const filtersToggle = document.getElementById(ID.FiltersToggle);
		const filtersPanel = document.getElementById(ID.FiltersPanel);
		let windowWidthOnLoad: number;
		let showResults = document.getElementById(ID.ViewResultsBtn) as HTMLButtonElement;

		const submitSearch = () => {
			let form = document.getElementById(ID.FormSubmit) as HTMLButtonElement;
			form.click();
		}

		const removeSelectedFilters = () => {
			let removePostcode = document.getElementById(ID.RemovePostcode) as HTMLLabelElement;
			if(removePostcode){
				removePostcode.addEventListener('click', () => {
					let postcode = document.getElementById(ID.PostcodeHidden) as HTMLInputElement;
					postcode.value = '';
					// submitSearch();
				})
			}

			let removeQuery = document.getElementById(ID.RemoveQuery) as HTMLLabelElement;
			if(removeQuery){
				removeQuery.addEventListener('click', () => {
					let query = document.getElementById(ID.QueryHidden) as HTMLInputElement;
					query.value = '';
					// submitSearch();
				})
			}

			
		}

		const updateFiltersPanel = () => {
			console.log("update filters panel called");
			if (window.innerWidth <= 992) {
				filtersToggle?.classList.remove(CLASSES.FiltersToggleActive);
				filtersPanel?.classList.remove(CLASSES.FiltersPanelActive);
				showResults.classList.remove(CLASSES.hide);
			} else {
				filtersToggle?.classList.add(CLASSES.FiltersToggleActive);
				filtersPanel?.classList.add(CLASSES.FiltersPanelActive);
				showResults.classList.add(CLASSES.hide);
			}
		}

		const init = () => {
			removeSelectedFilters();

			if (filtersToggle != null) {
				window.addEventListener('load', function (event) {
					windowWidthOnLoad = window.innerWidth;
					updateFiltersPanel();
				}, true);
			
				window.addEventListener('resize', function (event) {
					if (windowWidthOnLoad != window.innerWidth) {
						windowWidthOnLoad = window.innerWidth;
						updateFiltersPanel();
					}
				}, true);
			}

			let input = document.querySelectorAll('.' + CLASSES.SearchFilter) as NodeList;
			for(var i = 0; i < input.length; i++) {
				let radioButton = input[i] as HTMLInputElement;
				radioButton.addEventListener('change', () => {
					// submitSearch();
				})
			}

			let customDistance = document.getElementById(ID.CustomDistance) as HTMLInputElement;
			let customDistanceInput = document.getElementById(ID.CustomeDistanceInput) as HTMLInputElement;
			let customDistanceSubmit = document.getElementById(ID.CustomDistanceSubmit) as HTMLDivElement;

			if(customDistance){
				customDistance.addEventListener('change', () => {
					customDistanceSubmit.hidden = !customDistance.checked;
				})

				if(customDistanceInput){
					customDistanceInput.addEventListener('click', () => {
						customDistance.checked = true;
						customDistanceSubmit.hidden = false;
					})
				}
			}

			let query = document.getElementById(ID.QuerySubmit) as HTMLButtonElement;
			if(query){
				query.addEventListener('click', (el) => {
					el.preventDefault();
					let whatEntry = document.getElementById(ID.Query) as HTMLInputElement;
					let queryHidden = document.getElementById(ID.QueryHidden) as HTMLInputElement;
					queryHidden.value = whatEntry.value;
					// submitSearch();
				})
			}

			let clear = document.querySelectorAll(INPUT.searchClear);
			clear.forEach(button => {
				button.addEventListener('click', e => {
					var input = button.previousElementSibling as HTMLInputElement;
					input.value = "";
					input.focus();
				})
			});

			if (filtersPanel?.classList.contains(CLASSES.FiltersPanelActive)) {
				showResults?.addEventListener('click', (e) => {
					e.preventDefault();
					filtersToggle?.classList.remove(CLASSES.FiltersToggleActive);
					filtersPanel?.classList.remove(CLASSES.FiltersPanelActive);
					window.scrollTo(0, 0);
				});
			}

		};

		init();
	}
}

export default SearchFilters;

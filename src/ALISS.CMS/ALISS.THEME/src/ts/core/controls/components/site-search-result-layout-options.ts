/** @format */

class SearchResultLayoutOptions {
	constructor() {
		enum CLASSES {
			Active = 'aliss-search-layout-options-container__option--active',
            SearchResultItem = 'aliss-component-master__search-result-item',
            GridWidth = 'aliss-component-master__search-result-item--grid-layout'
		}

		enum ID {
			FormSubmit = 'search-filters-submit-form',
			View = 'View',
			ListView = 'list-view',
            GridView = 'grid-view'
		}

		let ListView = document.getElementById(ID.ListView);
		let GridView = document.getElementById(ID.GridView);
		let SearchResultItems = document.getElementsByClassName(CLASSES.SearchResultItem);
		let viewOption = document.getElementById(ID.View) as HTMLInputElement;
		let i;

		const submitSearch = () => {
			let form = document.getElementById(ID.FormSubmit) as HTMLButtonElement;
			form.click();
		}

		const init = () => {
			if (viewOption && GridView && ListView) {
				if (viewOption.value === "Gridview") {
					GridView?.classList.add(CLASSES.Active);
					ListView?.classList.remove(CLASSES.Active);

					for (i = 0; i < SearchResultItems.length; i++) {
						let item = SearchResultItems[i] as HTMLElement;
						item.classList.add(CLASSES.GridWidth);
					}
				} else {
					viewOption.value = "Listview";
				}
				toggleLayout(ListView, GridView);
			}
		};

		const toggleLayout = (ListView: any, GridView: any) => {
			ListView.addEventListener('click', (e: any) => {
				e.preventDefault();
				ListView.classList.add(CLASSES.Active);
				GridView?.classList.remove(CLASSES.Active);

				for (i = 0; i < SearchResultItems.length; i++) {
					let item = SearchResultItems[i] as HTMLElement;
					item.classList.remove(CLASSES.GridWidth);
				}
				viewOption.value = "Listview";
				// submitSearch();
			});
			GridView.addEventListener('click', (e: any) => {
				e.preventDefault();
				GridView.classList.add(CLASSES.Active);
				ListView?.classList.remove(CLASSES.Active);

				for (i = 0; i < SearchResultItems.length; i++) {
					let item = SearchResultItems[i] as HTMLElement;
					item.classList.add(CLASSES.GridWidth);
				}
				viewOption.value = "Gridview";
				// submitSearch();
			});
		}

		init();
	}
}

export default SearchResultLayoutOptions;
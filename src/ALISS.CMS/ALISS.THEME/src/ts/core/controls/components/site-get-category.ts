/** @format */

class GetCategoryData {
	constructor() {
		enum CLASSES {
			CategoriesInputP = 'search-category-primary',
			CategoryCheckbox = "category-checkbox",
			SelectedCategories = 'aliss-selected-category',
			hide = 'hide'
		}

		enum ID {
			FullPrimaryCategoryList = 'full-primary-category-list',
			FormSubmit = 'search-filters-submit-form',
			CategoriesHidden = 'Categories',
			RemoveAllCategories = 'selected-categories-clear',
		}

		let categoriesList;

		const submitSearch = () => {
			let form = document.getElementById(ID.FormSubmit) as HTMLButtonElement;
			// form.click();
			// reloadedResults.init;
			// loadResults.refreshSearchResults();
			// checkCategoriesFilter();
		}

		const removeSubCategories = (slug) => {
			var subCategories = '';
			var selectedCategoriesInput = document.getElementById(ID.CategoriesHidden) as HTMLInputElement;
			var selectedCategories: string[] = selectedCategoriesInput.value.toLowerCase().split(';');

			for (let p = 0; p < categoriesList.data.length; p++) {
				for (let s = 0; s < categoriesList.data[p].sub_categories.length; s++) {
					if(categoriesList.data[p].slug == slug && selectedCategories.includes(categoriesList.data[p].sub_categories[s].slug)){
						if(subCategories == ''){
							subCategories += categoriesList.data[p].sub_categories[s].slug;
						}
						else {
							subCategories += ';' + categoriesList.data[p].sub_categories[s].slug;
						}
					}

					for (let t = 0; t < categoriesList.data[p].sub_categories[s].sub_categories.length; t++) {
						if((categoriesList.data[p].sub_categories[s].slug == slug || categoriesList.data[p].slug == slug) && selectedCategories.includes(categoriesList.data[p].sub_categories[s].sub_categories[t].slug)){
							if(subCategories == ''){
								subCategories += categoriesList.data[p].sub_categories[s].sub_categories[t].slug;
							}
							else {
								subCategories += ';' + categoriesList.data[p].sub_categories[s].sub_categories[t].slug;
							}
						}
					}
				}
			}
			
			var subCategoriesArray: string[] = subCategories.split(';');
			var categoryList = '';
			for(var i = 0; i < selectedCategories.length; i++){
				if(selectedCategories[i] != slug && !subCategoriesArray.includes(selectedCategories[i])){
					if(categoryList == ''){
						categoryList += selectedCategories[i];
					}
					else {
						categoryList += ';' + selectedCategories[i];
					}
				}
			}
			
			return categoryList;
		}

		const updateCategories = (node : HTMLInputElement) => {
			var selectedCategoriesInput = document.getElementById(ID.CategoriesHidden) as HTMLInputElement;
			if(node.checked) {
				if(selectedCategoriesInput.value == ''){
					selectedCategoriesInput.value += node.value;
				}
				else {
					selectedCategoriesInput.value += ';' + node.value;
				}
			}
			else{
				selectedCategoriesInput.value = removeSubCategories(node.value);
			}
		}

		const checkboxDisplay = () => {
			var request = new XMLHttpRequest();
			request.open('GET', apiBaseUrl + 'categories', true);

			request.onload = function() {
				if (this.status >= 200 && this.status < 400) {
					// Success!
					var data = JSON.parse(this.response);
					categoriesList = data;

					var selectedCategoriesInput = document.getElementById(ID.CategoriesHidden) as HTMLInputElement;
					var selectedCategories: string[] = selectedCategoriesInput.value.toLowerCase().split(';');

					for(var p = 0; p < data.data.length; p++){
						if(selectedCategories.includes(data.data[p].slug)){
							var checkboxPrimary = document.getElementById('checkbox-' + data.data[p].slug) as HTMLInputElement;
							checkboxPrimary.checked = true;

							var secondaryCategories = document.getElementById(data.data[p].slug + "-categorys") as HTMLFieldSetElement;
							if(secondaryCategories){
								secondaryCategories.classList.remove(CLASSES.hide);
								secondaryCategories.hidden = false;
							}
						}

						for(var s = 0; s < data.data[p].sub_categories.length; s++){
							if(selectedCategories.includes(data.data[p].sub_categories[s].slug)){
								var checkboxPrimary = document.getElementById('checkbox-' + data.data[p].slug) as HTMLInputElement;
								checkboxPrimary.checked = true;

								var checkboxSecondary = document.getElementById('checkbox-' + data.data[p].sub_categories[s].slug) as HTMLInputElement;
								checkboxSecondary.checked = true;

								var secondaryCategories = document.getElementById(data.data[p].slug + "-categorys") as HTMLFieldSetElement;
								if(secondaryCategories){
									secondaryCategories.classList.remove(CLASSES.hide);
									secondaryCategories.hidden = false;
								}

								var tertiaryCategories = document.getElementById(data.data[p].sub_categories[s].slug + "-categoryt") as HTMLFieldSetElement;
								if(tertiaryCategories){
									tertiaryCategories.classList.remove(CLASSES.hide);
									tertiaryCategories.hidden = false;
								}
							}

							for(var t = 0; t < data.data[p].sub_categories[s].sub_categories.length; t++){
								if(selectedCategories.includes(data.data[p].sub_categories[s].sub_categories[t].slug)){
									var checkboxPrimary = document.getElementById('checkbox-' + data.data[p].slug) as HTMLInputElement;
									checkboxPrimary.checked = true;
	
									var checkboxSecondary = document.getElementById('checkbox-' + data.data[p].sub_categories[s].slug) as HTMLInputElement;
									checkboxSecondary.checked = true;
	
									var secondaryCategories = document.getElementById(data.data[p].slug + "-categorys") as HTMLFieldSetElement;
									if(secondaryCategories){
										secondaryCategories.classList.remove(CLASSES.hide);
										secondaryCategories.hidden = false;
									}

									var checkboxTertiary = document.getElementById('checkbox-' + data.data[p].sub_categories[s].sub_categories[t].slug) as HTMLInputElement;
									checkboxTertiary.checked = true;
	
									var tertiaryCategories = document.getElementById(data.data[p].sub_categories[s].slug + "-categoryt") as HTMLFieldSetElement;
									if(tertiaryCategories){
										tertiaryCategories.classList.remove(CLASSES.hide);
										tertiaryCategories.hidden = false;
									}
								}
							}
						}
					}
				}
				else {
					// We reached our target server, but it returned an error
					console.log('We reached our target server, but it returned an error');
				}
			};

			request.onerror = function() {
				// There was a connection error of some sort
				console.log('error');
			};
			request.send();
		}

		const init = () => {
			
			checkboxDisplay();
			let input = document.querySelectorAll('.' + CLASSES.CategoryCheckbox) as NodeList;
			for(var i = 0; i < input.length; i++) {
				let checkbox = input[i] as HTMLInputElement;
				checkbox.addEventListener('change', ()=> {
					// console.log("checkbox changed/clicked");
					updateCategories(checkbox);
					submitSearch();
				})
			}

			let categories = document.getElementById(ID.CategoriesHidden) as HTMLInputElement;
			let removeAllCategories = document.getElementById(ID.RemoveAllCategories) as HTMLButtonElement;
			if(removeAllCategories){
				removeAllCategories.addEventListener('click', (el) => {
					el.preventDefault();
					categories.value = '';
					submitSearch();
				})
			}

			let selectedCategoryList = document.querySelectorAll('.' + CLASSES.SelectedCategories) as NodeList;
			selectedCategoryList.forEach(category => {
				let removeCategory = category as HTMLLabelElement;
				removeCategory.addEventListener('click', () => {
					let categoryList = categories.value.toLowerCase().split(';');
					var slug = category.id?.split('--').pop();
					for(var i = 0; i < categoryList.length; i++){
						if(categoryList[i] == slug){
							let updateCategories = categories.value.toLowerCase().split(';');
							if(updateCategories.length > 1){
								var categoryIndex = updateCategories.indexOf(slug);
								updateCategories.splice(categoryIndex, 1);
								categories.value = updateCategories.join(';');
								categories.value = removeSubCategories(slug);
							}
							else{
								categories.value = '';
							}
							submitSearch();
						}
					}
				})
			});
		};

		!!document.querySelector('.' + CLASSES.CategoriesInputP) && init();
	}
}

export default GetCategoryData;

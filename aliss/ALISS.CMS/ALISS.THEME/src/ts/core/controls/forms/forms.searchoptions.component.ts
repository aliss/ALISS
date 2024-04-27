
import Aliss from '../../index';

class SearchOptions {
	constructor() {
		enum IDS {
			categoryList = 'category_list',
			whoList = 'who_list',
			categoriesInput = 'categories',
			whoInput = 'communitygroups',
			whatInput = 'query',
			searchSuggestionsList = 'search-suggestions-list',
			whoForm = 'search-form-who',
			categoriesForm = 'search-form-category',
			whatForm = 'search-form-what'
		}

		enum CLASSES {
			searchOption = 'search-options__option',
			hidden = 'hidden',
			active = 'search-options__option--active',
			searchSuggestions = 'aliss-search-suggestions__items',
			searchSuggestionsItem = 'aliss-search-suggestions__item'
		}

		enum APIS {
			categories = 'categories-flat',
			communitygroups = 'community-groups-flat'
		}

		enum KEYS {
			arrowDown = 'ArrowDown',
			arrowUp = 'ArrowUp',
			enter = 'Enter'
		}

		const searchOptions = document.querySelectorAll("." + CLASSES.searchOption);
		let categoriesSearchList = [] as any;
		let whoSearchList = [] as any;


		const init = () => {
			searchOptionButtons();
			getCategoryList();
			getWhoList();
		}

		const getCategoryList = () => {
			const request = new XMLHttpRequest();

			request.open("GET", apiBaseUrl + APIS.categories, true);
			request.onload = function () {
				if (this.status >= 200 && this.status < 400) {
					let data = JSON.parse(this.response);

					if (data) {
						for (let c = 0; c < data.data.length; c++) {
							categoriesSearchList.push(data.data[c]);
						}
					}
				} else {
					console.log('We reached our target server, but it returned an error');
				}
			}
			request.send();
		}

		const getWhoList = () => {
			const request = new XMLHttpRequest();

			request.open("GET", apiBaseUrl + APIS.communitygroups, true);
			request.onload = function () {
				if (this.status >= 200 && this.status < 400) {
					let data = JSON.parse(this.response);

					if (data) {
						for (let c = 0; c < data.data.length; c++) {
							whoSearchList.push(data.data[c]);
						}
					}
				} else {
					console.log('We reached our target server, but it returned an error');
				}
			}
			request.send();
		}

		const searchOptionButtons = () => {
			searchOptions.forEach(e => {
				let searchOptionButton = e as HTMLElement;
				searchOptionButton.addEventListener("click", (el) => {
					if (!searchOptionButton.classList.contains(CLASSES.active)) {
						let activeButton = document.querySelectorAll("." + CLASSES.searchOption + "." + CLASSES.active)
						activeButton.forEach(b => {
							let activeButton = b as HTMLElement;
							activeButton.classList.remove(CLASSES.active);
							let fieldId = activeButton.dataset.field as string;
							let activeField = document.getElementById(fieldId) as HTMLElement;
							activeField.classList.add(CLASSES.hidden);
						});

						searchOptionButton.classList.add(CLASSES.active);

						let fieldId = searchOptionButton.dataset.field as string;
						let searchField = document.getElementById(fieldId) as HTMLElement;
						let inputID = searchField.querySelectorAll("input")[0].id;
						let inputField = document.getElementById(inputID);

						searchField.classList.remove(CLASSES.hidden);

						if (inputID == IDS.categoriesInput) {
							autoCompleteLookUp(inputField, categoriesSearchList);
						} else if (inputID == IDS.whoInput) {
							autoCompleteLookUp(inputField, whoSearchList);
						}
					}
				});
			});

			var whoForm = document.getElementById(IDS.whoForm) as HTMLFormElement;
			if (whoForm) {
				whoForm.addEventListener("submit", (el) => {
					el.preventDefault();
					var input = document.getElementById(IDS.whoInput) as HTMLInputElement;
					input.value = input.value.trimEnd().trimStart();
					whoForm.submit();
				});
			}

			var whatForm = document.getElementById(IDS.whatForm) as HTMLFormElement;
			if (whatForm) {
				whatForm.addEventListener("submit", (el) => {
					el.preventDefault();
					var input = document.getElementById(IDS.whatInput) as HTMLInputElement;
					input.value = input.value.trimEnd().trimStart();
					whatForm.submit();
				});
			}

			var categoriesForm = document.getElementById(IDS.categoriesForm) as HTMLFormElement;
			if (categoriesForm) {
				categoriesForm.addEventListener("submit", (el) => {
					el.preventDefault();
					var input = document.getElementById(IDS.categoriesInput) as HTMLInputElement;
					input.value = input.value.trimEnd().trimStart();
					categoriesForm.submit();
				});
			}
		}

		const autoCompleteLookUp = (input: any, arr: any) => {
			let currentSuggestionFocus: any;
			input.addEventListener("input", function () {
				console.log(input);
				let listItemContainer,
					itemContainer,
					item,
					itemValue = input.value;

				closeSearchSuggestions();
				if (!itemValue) {
					return false;
				}
				currentSuggestionFocus = -1;
				// DIV element created to contain items
				listItemContainer = document.createElement("div");
				listItemContainer.setAttribute("id", input.id + IDS.searchSuggestionsList);
				listItemContainer.setAttribute("class", CLASSES.searchSuggestions);
				input.parentNode.appendChild(listItemContainer);

				for (item = 0; item < arr.length; item++) {
					// Check if the item starts with the same letters as the input field
					if (arr[item].name.substr(0, itemValue.length).toUpperCase() == itemValue.toUpperCase()) {
						// DIV element created for each matching element
						itemContainer = document.createElement("div");
						itemContainer.setAttribute("class", CLASSES.searchSuggestionsItem);
						itemContainer.innerHTML = "<strong>" + arr[item].name.substr(0, itemValue.length) + "</strong>";
						itemContainer.innerHTML += arr[item].name.substr(itemValue.length);
						// Input field for suggested value
						itemContainer.innerHTML += "<input type='hidden' value='" + arr[item].slug + "'>";
						// Value inserted into input field once clicked
						itemContainer.addEventListener("click", function (e) {
							input.value = this.getElementsByTagName("input")[0].value;
							closeSearchSuggestions();
						});
						listItemContainer.appendChild(itemContainer);
					}
				}
			});

			input.addEventListener("keydown", function (e: any) {
				let searchSuggestionsList = document.getElementById(input.id + IDS.searchSuggestionsList) as any;

				if (searchSuggestionsList) {
					searchSuggestionsList = searchSuggestionsList.getElementsByTagName("div");
				}
				if (e.key == KEYS.arrowDown) {
					currentSuggestionFocus++;
					addActive(searchSuggestionsList);
				} else if (e.key == KEYS.arrowUp) {
					currentSuggestionFocus--;
					addActive(searchSuggestionsList);
				} else if (e.key == KEYS.enter) {
					// e.preventDefault();
					if (currentSuggestionFocus > -1) {
						// Simulate click on active item
						if (searchSuggestionsList) {
							searchSuggestionsList[currentSuggestionFocus].click();
						}
					}
				}
				input.addEventListener("mouseover", function (e: any) {
					removeActive(searchSuggestionsList);
				});
			});



			const addActive = (list: any) => {
				if (!list) return false;
				// Initially, active class is removed from all items in list
				removeActive(list);
				if (currentSuggestionFocus >= list.length) {
					currentSuggestionFocus = 0;
				}
				if (currentSuggestionFocus < 0) {
					currentSuggestionFocus = (list.length - 1);
				}
				// Active class is added
				list[currentSuggestionFocus].classList.add("aliss-search-suggestions__item--active");
			}
			const removeActive = (list: any) => {
				// Active class is removed from all items in list
				for (var i = 0; i < list?.length; i++) {
					list[i]?.classList.remove("aliss-search-suggestions__item--active");
				}
			}

			const closeSearchSuggestions = (el?: any) => {
				let x = document.getElementsByClassName(CLASSES.searchSuggestions);
				for (let i = 0; i < x.length; i++) {
					if (el != x[i] && el != input) {
						x[i].parentNode?.removeChild(x[i]);
					}
				}
			}
			document.addEventListener("click", function (e) {
				closeSearchSuggestions(e.target);
			});
		}

		init();
	}
}

export default SearchOptions;
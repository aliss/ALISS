import Aliss from '../../';
import Template from '../forms/forms.category.template';

class Categories {
	constructor() {

		enum ID {
			filter = 'category-filter',
			selectedCount = 'count-categories-selected',
			selectedTitle = 'title-categories-selected',
			categoriesSelected = 'categories-selected'
		}

		enum INPUT {
			checkbox = 'input[type=checkbox][name=categories]'
		}

		enum CLASSES {
			filter = "aliss-categories--filter",
			accHead = "aliss-accordion__trigger",
			accHeadActive = "aliss-accordion__trigger--active",
			accContent = "aliss-accordion__content",
			accContentActive = "aliss-accordion__content--active",
			thirdSubClass = "aliss-categories__sub--three",
			subClass = "aliss-categories__sub",
			parentClass = "aliss-categories__parent",
			noResult = "aliss-categories__noresult",
			selectedItems = "aliss-selected__items",
			selectedItem = "aliss-selected__item",
			selectedTitle = "aliss-selected__title",
			remove = "aliss-selected__remove",
			helptext = "aliss-accordion__content--helptext"
		}

		enum EDITS {
			id = "##ID##",
			name = "##NAME##",
			value = "##VALUE##",
			idText = "##IDTEXT##"
		}

		const init = () => {
			checkSelected();
			filtering();
			toggleSelected();
			toggleCount();
			initRemoveSelected();
		}

		let temp = Template;
		
		const filter = document.getElementById(ID.filter) as HTMLInputElement;
		const itemsContainer = document.getElementById(ID.categoriesSelected) as HTMLElement;	
		const noresult = document.querySelector('.' + CLASSES.noResult) as HTMLElement;			
		const hidden = document.getElementById('SelectedCategories') as HTMLInputElement;
		const selectedCount = document.getElementById(ID.selectedCount) as HTMLElement;
		const selectedTitle = document.getElementById(ID.selectedTitle) as HTMLElement;
		const checkBoxesSelected = document.querySelectorAll(INPUT.checkbox + ':checked');
		const arrayValues = [] as any;

		const filtering = () => {
			const items = document.querySelectorAll("[data-filter]");
			const accHead = document.querySelectorAll("." + CLASSES.accHead);
			const accCont = document.querySelectorAll("." + CLASSES.accContent);
			const accHelp = document.querySelectorAll("." + CLASSES.helptext);

			if (typeof(filter) != 'undefined' && filter != null) {
				let filterTextArray = [] as any;
				
				items.forEach(e => {
					const filteredItem = e as HTMLElement;
					let filterText = filteredItem.dataset.filter as String;
					filterTextArray.push(filterText);
				});

				filter.addEventListener("keyup", (e) => {
					let _this = e.target as HTMLInputElement;
					let lowercase = filter.value.toLowerCase();
					if(filter.value != "") {
						accHead.forEach((head) => {
							head.classList.add(CLASSES.accHeadActive);
						});
						accCont.forEach((cont) => {
							cont.classList.add(CLASSES.accContentActive);
						});
						accHelp.forEach((cont) => {
							var div = cont as HTMLDivElement;
							div.style.display = "none";
						});
					} else {
						accHead.forEach((head) => {
							head.classList.remove(CLASSES.accHeadActive);
						});
						accCont.forEach((cont) => {
							cont.classList.remove(CLASSES.accContentActive);
						});
						accHelp.forEach((cont) => {
							var div = cont as HTMLDivElement;
							div.removeAttribute("style");
						});
					}
					items.forEach(e => {
						const filteredItem = e as HTMLElement;
						let filterText = filteredItem.dataset.filter as String;
						if(filterText.toLowerCase().indexOf(filter.value.toLowerCase()) > -1) {				
							filteredItem.removeAttribute("style");	
							const parent = filteredItem.parentElement?.parentElement as HTMLElement;
							const root = filteredItem.parentElement?.parentElement?.parentElement?.parentElement as HTMLElement;
							parent.removeAttribute("style");
							root.removeAttribute("style");
							if (filteredItem.className.indexOf("__sub") > -1) {
								filteredItem.parentElement?.firstElementChild?.removeAttribute("style");
							}
							if (filteredItem.className.indexOf("--three") > -1) {
								filteredItem.parentElement?.parentElement?.parentElement?.firstElementChild?.removeAttribute("style");
							}
						} else {			
							filteredItem.style.display = 'none';
						}			
										
					});
					let convertString = filterTextArray.toString();
					let updatedString = convertString.replace(new RegExp(',', 'g'), ' ');
					if(updatedString.toLowerCase().indexOf(_this.value.toLowerCase()) > -1) {
						noresult.removeAttribute("style");
					} else {			
						noresult.style.display = 'block';	
					}	
				});
				
			}
		}

		const checkSelected = () => {
			for (var i = 0; i < checkBoxesSelected.length; i++) {
				let item = checkBoxesSelected[i] as HTMLInputElement;
				let dataId = item.dataset.id as string;
				const html = temp;
				const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), item.value).replace(new RegExp(EDITS.id, 'g'), dataId.toString()).replace(EDITS.name, "categories").replace(EDITS.idText, "category");
				var domElement = document.createElement("div");
				domElement.classList.add(CLASSES.selectedItem);
				domElement.innerHTML = updatedHtml;
				itemsContainer.appendChild(domElement);
				arrayValues.push(item.dataset.id);
			}			
			if(arrayValues.length > 0) {
				hidden.value = arrayValues.toString();
			}
			toggleCount();
		}

		const toggleCount = () => {
			var selectedLength = arrayValues.length;
			if(selectedCount != null) {
				if(selectedLength > 0) {
					selectedCount.innerHTML = selectedLength.toString();
					selectedTitle.removeAttribute("style");
				} else {
					selectedTitle.style.display = "none";
				}
			}
		}

		const toggleSelected = () => {
			document.addEventListener("click", (el) => {
				let element = el.target as HTMLInputElement;	
				let dataId = element.dataset.id as string;	
				let query = ".aliss-accordion__header"
				if (element.id == "category-" + dataId) {
				let thirdCategory = Aliss.Helpers.closest(element, ".aliss-categories__sub--three");
				let ifThird = Aliss.Helpers.hasClass(element, "aliss-categories__sub--three") ? thirdCategory : element.parentNode?.parentNode?.parentNode;	
				let subCategory;
				let parentCategory;
				if (ifThird != null) {
					subCategory = Aliss.Helpers.closest(ifThird, ".aliss-categories__sub");
					parentCategory = Aliss.Helpers.closest(ifThird, ".aliss-categories__primary");
				}
				let i3 = thirdCategory?.querySelector(query);
				let i2 = subCategory?.querySelector(query);
				let i1 = parentCategory?.querySelector(query);	
					if (element.checked) {						
						const html = temp;
						const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), element.value).replace(new RegExp(EDITS.id, 'g'), dataId).replace(EDITS.name, "categories").replace(EDITS.idText, "category");
						var domElement = document.createElement("div");
						domElement.classList.add(CLASSES.selectedItem);
						domElement.innerHTML = updatedHtml;
						itemsContainer.appendChild(domElement);
						hidden.value = arrayValues.toString();
						arrayValues.push(dataId);
						i1 != null ? Aliss.Helpers.addClass(i1, "aliss-accordion__header--selected") : null;
						i2 != null ? Aliss.Helpers.addClass(i2, "aliss-accordion__header--selected") : null;
						i3 != null ? Aliss.Helpers.addClass(i3, "aliss-accordion__header--selected") : null;
					}
					else {
						let selectedItem = document.getElementById("selected-" + dataId) as HTMLButtonElement;
						let selectedItemContainer = selectedItem.parentNode as HTMLElement;
						Aliss.Helpers.removeElementByNode(selectedItemContainer);						
						i1 != null ? Aliss.Helpers.removeClass(i1, "aliss-accordion__header--selected") : null;
						i2 != null ? Aliss.Helpers.removeClass(i2, "aliss-accordion__header--selected") : null;
						i3 != null ? Aliss.Helpers.removeClass(i3, "aliss-accordion__header--selected") : null;
						removeFromArray(dataId);
					}	
					initRemoveSelected();	
					hidden.value = arrayValues.toString();	
				}				
				toggleCount();
			});	
		}

		const removeFromArray = (dataId: string) => {
			const index = arrayValues.indexOf(dataId);
			if (index > -1) {
				arrayValues.splice(index, 1);
			}	
		}

		const initRemoveSelected = () => {
			var remove = document.querySelectorAll('.'+CLASSES.remove);
			for (var i = 0; i < remove.length; i++) {
				remove[i].addEventListener("click", (el) => {
					el.preventDefault();
					let element = el.target as HTMLButtonElement;
					if (element.dataset.input != null && element.dataset.input.startsWith("category-")) {
					let dataId = element.dataset.id as string;
					let input = element.dataset.input as string;
					let item = document.getElementById(input) as HTMLInputElement;
					let parent = element.parentNode as HTMLElement;
					item.checked = false;
					Aliss.Helpers.removeElementByNode(parent);
					toggleCount();
					removeFromArray(dataId);					
					hidden.value = arrayValues.toString();	
					}
				});
			}
		}

		init();
	}
}

export default Categories;
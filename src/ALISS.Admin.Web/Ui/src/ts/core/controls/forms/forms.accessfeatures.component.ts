import Aliss from '../../';
import Template from '../forms/forms.accessfeature.template';

class AccessibilityFeatures {
	constructor() {

		enum ID {
			filter = 'accessibility-filter-',
			selectedCount = 'count-accessibility-features-selected-',
			selectedTitle = 'title-accessibility-features-selected-',
			accessibilityFeaturesSelected = 'accessibility-features-selected-',
			selectedAccessibilityFeaturesList = 'SelectedAccessibilityFeatures',
			noResult = "no-result-"
		}

		enum INPUT {
			checkboxName = 'accessibility-features-'
		}

		enum CLASSES {
			filter = "aliss-accessibility-features--filter",
			accHead = "aliss-accordion__trigger",
			accHeadActive = "aliss-accordion__trigger--active",
			accContent = "aliss-accordion__content",
			accContentActive = "aliss-accordion__content--active",
			thirdSubClass = "aliss-datainput-accessibility-features__sub--three",
			subClass = "aliss-datainput-accessibility-features__sub",
			parentClass = "aliss-datainput-accessibility-features__parent",
			selectedItems = "aliss-selected__items",
			selectedItem = "aliss-selected__item",
			selectedTitle = "aliss-selected__title",
			remove = "aliss-selected__remove-",
			ServiceLocations = "service-locations",
			dataFilter = "data-filter-",
			checkboxes = "accessibility-features-"
		}

		enum EDITS {
			id = "##ID##",
			name = "##NAME##",
			value = "##VALUE##",
			idText = "##IDTEXT##",
			location = "##LOCATION##"
		}

		const init = () => {
			for (let i = 0; i < serviceLocations.length; i++) {
				const location = serviceLocations[i] as HTMLInputElement;
				checkSelected(location.value, i);
				filtering(location.value, i);
				toggleSelected(location.value, i);
				toggleCount(location.value, i);
				updatedAdditionalInfo(location.value);
				initRemoveSelected(location.value, i);
			}
		}

		let temp = Template;
		let filterList: HTMLInputElement[] = new Array();
		let itemsContainerList: HTMLElement[] = new Array();
		let noResultsList: HTMLElement[] = new Array(); 
		const hidden = document.getElementById(ID.selectedAccessibilityFeaturesList) as HTMLInputElement;
		let selectedCountList: HTMLElement[] = new Array();
		let selectedTitleList: HTMLElement[] = new Array();
		let checkBoxesSelectedList: NodeListOf<Element>[] = new Array();
		let arrayValues = [] as any;
		const serviceLocations = document.querySelectorAll('.' + CLASSES.ServiceLocations) as NodeListOf<HTMLInputElement>;

		serviceLocations.forEach(location => {
			const filter = document.getElementById(ID.filter + location.value) as HTMLInputElement;
			filterList.push(filter);

			const itemsContainer = document.getElementById(ID.accessibilityFeaturesSelected + location.value) as HTMLElement;
			itemsContainerList.push(itemsContainer);

			const noresult = document.getElementById(ID.noResult + location.value) as HTMLElement;	
			noResultsList.push(noresult);

			const selectedCount = document.getElementById(ID.selectedCount + location.value) as HTMLElement;
			selectedCountList.push(selectedCount);

			const selectedTitle = document.getElementById(ID.selectedTitle + location.value) as HTMLElement;
			selectedTitleList.push(selectedTitle);

			const selectedCheckboxes: string = 'input[type=checkbox][name="' + INPUT.checkboxName + location.value + '"]';
			const checkBoxesSelected = document.querySelectorAll(selectedCheckboxes + ':checked');
			checkBoxesSelectedList.push(checkBoxesSelected)
		});
		

		const filtering = (location: string, index: number) => {
			const filter = filterList.at(index) as HTMLInputElement;
			let noresult = noResultsList.at(index) as HTMLElement;

			const itemClass: string = CLASSES.dataFilter + location;
			const items = document.querySelectorAll('div.' + itemClass);
			const accHead = document.querySelectorAll("." + CLASSES.accHead);
			const accCont = document.querySelectorAll("." + CLASSES.accContent);

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
					} else {
						accHead.forEach((head) => {
							head.classList.remove(CLASSES.accHeadActive);
						});
						accCont.forEach((cont) => {
							cont.classList.remove(CLASSES.accContentActive);
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
						} else {			
							filteredItem.style.display = 'none';
						}			
										
					});
					let convertString = filterTextArray.toString();
					let updatedString = convertString.replace(new RegExp(',', 'g'), ' ');
					if(noresult){
						if(updatedString.toLowerCase().indexOf(_this.value.toLowerCase()) > -1) {
							noresult.removeAttribute("style");
						} else {			
							noresult.style.display = 'block';	
						}	
					}
				});
				
			}
		}

		const checkSelected = (location: string, index: number) => {
			const checkBoxesSelected = checkBoxesSelectedList.at(index) as NodeListOf<Element>;
			let itemsContainer = itemsContainerList.at(index) as HTMLElement;

			if(checkBoxesSelected && itemsContainer && hidden){
				for (var i = 0; i < checkBoxesSelected.length; i++) {
					let item = checkBoxesSelected[i] as HTMLInputElement;
					let dataId = item.dataset.id as string;
					let itemAdditionalInfo = document.getElementById(item.id + "-moreinfo") as HTMLInputElement;
					const html = temp;
					const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), item.value).replace(new RegExp(EDITS.id, 'g'), dataId.toString()).replace(EDITS.name, "accessibility-features-" + location).replace(EDITS.idText, "accessibility").replace(new RegExp(EDITS.location, 'g'), location);
					var domElement = document.createElement("div");
					domElement.classList.add(CLASSES.selectedItem);
					domElement.innerHTML = updatedHtml;
					itemsContainer.appendChild(domElement);
					var idAndAdditionalInfo = location + "||" + dataId + "|" + itemAdditionalInfo.value;
					arrayValues.push(idAndAdditionalInfo);
					document.getElementById("accessibility-" + dataId + "-moreinfo-container-" + location)?.classList.remove("hidden");
				}			
				if (arrayValues.length > 0) {
					hidden.value = arrayValues.join('¬');
				}
				toggleCount(location, index);
			}
		}

		const toggleCount = (location: string, index: number) => {
			const selectedCount = selectedCountList.at(index) as HTMLElement;
			const selectedTitle = selectedTitleList.at(index) as HTMLElement;

			var locationArray: string[] = new Array();
			arrayValues.forEach(val => {
				if(val.startsWith(location + '||')){
					locationArray.push(val);
				}
			});

			var selectedLength = locationArray.length;
			if(selectedCount != null) {
				if(selectedLength > 0) {
					selectedCount.innerHTML = selectedLength.toString();
					selectedTitle.removeAttribute("style");
				} else {
					selectedTitle.style.display = "none";
				}
			}
		}

		// const toggleDisabled = () => {
		// 	const checkBoxesSelected = document.querySelectorAll(INPUT.checkbox + ':checked');
		// 	const checkBoxesNotSelected = document.querySelectorAll(INPUT.checkbox + ':not(:checked)');
		// 	for (var i = 0; i < checkBoxesNotSelected.length; i++) {
		// 		let item = checkBoxesNotSelected[i] as HTMLInputElement;
		// 		if(checkBoxesSelected.length > 7) {
		// 			item.disabled = true;
		// 		} else {
		// 			item.disabled = false;
		// 		}
		// 	}
		// }

		const updatedAdditionalInfo = (location: string) => {

			let moreInfoClass : string = "moreinfo-" + location
			var moreinfoFields = document.querySelectorAll('textarea.' + moreInfoClass);
			for (let i = 0; i < moreinfoFields.length; i++) {
				let item = moreinfoFields[i];
				item.addEventListener("change", (el) => {
					let element = el.target as HTMLInputElement;
					let parentId = element.dataset.parent;
					var index = arrayValues.findIndex((arrayItem) => { return arrayItem.startsWith(location+"||"+parentId+"|"); }, parentId)
					if (index > -1) {
						arrayValues[index] = location + "||" + parentId + "|" + element.value;
					}
					hidden.value = arrayValues.join('¬');
				});
			}
		}

		const toggleSelected = (location: string, index: number) => {
			let itemsContainer = itemsContainerList.at(index) as HTMLElement;

			document.addEventListener("click", (el) => {
				let element = el.target as HTMLInputElement;	
				let dataId = element.dataset.id as string;	
				let query = ".aliss-accordion__header"
				if (element.id == "accessibility-" + dataId + "-" + location) {
					let additionalInfo = document.getElementById("accessibility-" + dataId + "-" + location + "-moreinfo") as HTMLInputElement;
					if (element.checked) {
						const html = temp;
						const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), element.value).replace(new RegExp(EDITS.id, 'g'), dataId).replace(EDITS.name, "accessibility-features-" + location).replace(EDITS.idText, "accessibility").replace(new RegExp(EDITS.location, 'g'), location);
						var domElement = document.createElement("div");
						domElement.classList.add(CLASSES.selectedItem);
						domElement.innerHTML = updatedHtml;
						itemsContainer.appendChild(domElement);
						arrayValues.push(location + "||" + dataId + "|" + additionalInfo.value);
						document.getElementById("accessibility-" + dataId + "-moreinfo-container-" + location)?.classList.remove("hidden");
					}
					else {
						let selectedItem = document.getElementById("selected-" + dataId + "-" + location) as HTMLButtonElement;
						let selectedItemContainer = selectedItem.parentNode as HTMLElement;
						Aliss.Helpers.removeElementByNode(selectedItemContainer);						
						removeFromArray(location + "||" + dataId + "|" + additionalInfo.value);
						document.getElementById("accessibility-" + dataId + "-moreinfo-container-" + location)?.classList.add("hidden");
					}	
					initRemoveSelected(location, index);	
					hidden.value = arrayValues.join('¬');	
				}				
				toggleCount(location, index);	
				// toggleDisabled();
			});	
		}

		const removeFromArray = (dataId: string) => {
			const index = arrayValues.indexOf(dataId);
			if (index > -1) {
				arrayValues.splice(index, 1);
			}	
		}

		const initRemoveSelected = (location: string, index: number) => {
			var remove = document.querySelectorAll('.' + CLASSES.remove + location);
			for (var i = 0; i < remove.length; i++) {
				remove[i].addEventListener("click", (el) => {
					el.preventDefault();
					let element = el.target as HTMLButtonElement;
					if (element.dataset.input != null && element.dataset.input.startsWith("accessibility-")) {
						let dataId = element.dataset.id as string;
						let input = element.dataset.input as string;
						let item = document.getElementById(input) as HTMLInputElement;
						let moreInfo = document.getElementById(item.id + "-moreinfo") as HTMLInputElement;
						let parent = element.parentNode as HTMLElement;
						item.checked = false;
						document.getElementById("accessibility-" + dataId + "-moreinfo-container-" + location)?.classList.add("hidden");
						Aliss.Helpers.removeElementByNode(parent);
						toggleCount(location, index);	
						// toggleDisabled();
						removeFromArray(location + "||" + dataId + "|" + moreInfo.value);
						if (hidden != null) {
							hidden.value = arrayValues.join('¬');
						}
					}
				});
			}
		}

		init();
	}
}

export default AccessibilityFeatures;
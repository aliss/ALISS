import Aliss from '../..';
import Template from '../forms/forms.servicearea.template';

class ServiceAreas {
	constructor() {

		enum ID {
			filter = 'servicearea-filter',
			selectedCount = 'count-serviceareas-selected',
			selectedTitle = 'title-serviceareas-selected',
			serviceareasSelected = 'serviceareas-selected',
			regionsViewAllBtn = "view-selected-regions",
			selectedRegions = "serviceareas-selected",
			selectedRegionsContainer = "serviceSelectedRegions",
		}

		enum INPUT {
			checkbox = 'input[type=checkbox][name=serviceareas]'
		}

		enum CLASSES {
			filter = "aliss-serviceareas--filter",
			accHead = "aliss-accordion__trigger",
			accHeadActive = "aliss-accordion__trigger--active",
			accContent = "aliss-accordion__content",
			accContentActive = "aliss-accordion__content--active",
			thirdSubClass = "aliss-serviceareas__sub--three",
			subClass = "aliss-serviceareas__sub",
			parentClass = "aliss-serviceareas__parent",
			noResult = "aliss-serviceareas__noresult",
			selectedItems = "aliss-selected__items",
			selectedItem = "aliss-selected__item",
			selectedTitle = "aliss-selected__title",
			remove = "aliss-selected__remove"
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
			// toggleDisabled();
			initRemoveSelected();
		}

		let temp = Template;

		const filter = document.getElementById(ID.filter) as HTMLInputElement;
		const itemsContainer = document.getElementById(ID.serviceareasSelected) as HTMLElement;
		const noresult = document.querySelector('.' + CLASSES.noResult) as HTMLElement;
		const hidden = document.getElementById('SelectedServiceAreas') as HTMLInputElement;
		const selectedCount = document.getElementById(ID.selectedCount) as HTMLElement;
		const selectedTitle = document.getElementById(ID.selectedTitle) as HTMLElement;
		const checkBoxesSelected = document.querySelectorAll(INPUT.checkbox + ':checked');
		const arrayValues = [] as any;
		const serviceRegionsSection = document.getElementById(ID.selectedRegionsContainer) as HTMLElement;

		const filtering = () => {
			const items = document.querySelectorAll("[data-filter]");
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
				const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), item.value).replace(new RegExp(EDITS.id, 'g'), dataId.toString()).replace(EDITS.name, "serviceareas").replace(EDITS.idText, "servicearea");
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

		const toggleViewAllSelectedRegions = (oldValue: number, newValue: number) => {
			let regionsViewAllBtn = document.getElementById(ID.regionsViewAllBtn) as HTMLElement;
			let selectedRegions = document.getElementById(ID.selectedRegions) as HTMLElement;
			if (newValue > oldValue) {
				if (selectedRegions.classList.contains("hide")) {
					selectedRegions.classList.toggle("hide");
					regionsViewAllBtn.textContent = "Hide all";
				}
			} else if (newValue === 0 && !selectedRegions.classList.contains("hide")) {
				selectedRegions.classList.add("hide");
				regionsViewAllBtn.textContent = "View all";
			}
		}

		const toggleCount = () => {
			var selectedLength = arrayValues.length;
			if(selectedCount != null) {
				if(selectedLength > 0) {
					selectedCount.innerHTML = selectedLength.toString();
					selectedTitle.removeAttribute("style");
					serviceRegionsSection.removeAttribute("style");
				} else {
					selectedCount.innerHTML = selectedLength.toString();
					selectedTitle.style.display = "none";
					serviceRegionsSection.style.display = "none";
				}
			}
		}

		const toggleDisabled = () => {
			const checkBoxesSelected = document.querySelectorAll(INPUT.checkbox + ':checked');
			const checkBoxesNotSelected = document.querySelectorAll(INPUT.checkbox + ':not(:checked)');
			for (var i = 0; i < checkBoxesNotSelected.length; i++) {
				let item = checkBoxesNotSelected[i] as HTMLInputElement;
				if(checkBoxesSelected.length > 7) {
					item.disabled = true;
				} else {
					item.disabled = false;
				}
			}
		}

		const toggleSelected = () => {
			document.addEventListener("click", (el) => {
				let element = el.target as HTMLInputElement;
				let dataId = element.dataset.id as string;
				let query = ".aliss-accordion__header"
				if (element.id == "servicearea-" + dataId) {
				let thirdServiceArea = Aliss.Helpers.closest(element, ".aliss-serviceareas__sub--three");
				let ifThird = Aliss.Helpers.hasClass(element, "aliss-serviceareas__sub--three") ? thirdServiceArea : element.parentNode?.parentNode?.parentNode;
				let subServiceArea;
				let parentServiceArea;
				if (ifThird != null) {
					subServiceArea = Aliss.Helpers.closest(ifThird, ".aliss-serviceareas__sub");
					parentServiceArea = Aliss.Helpers.closest(ifThird, ".aliss-serviceareas__primary");
				} 
				let i3 = thirdServiceArea?.querySelector(query);
				let i2 = subServiceArea?.querySelector(query);
				let i1 = parentServiceArea?.querySelector(query);
					const orgSelectedCount = arrayValues.length;
					if (element.checked) {
						const html = temp;
						const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), element.value).replace(new RegExp(EDITS.id, 'g'), dataId).replace(EDITS.name, "serviceareas").replace(EDITS.idText, "servicearea");
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
					toggleViewAllSelectedRegions(orgSelectedCount, arrayValues.length);
					initRemoveSelected();
					hidden.value = arrayValues.toString();
				}
				toggleCount();
				// toggleDisabled();
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
					if (element.dataset.input != null && element.dataset.input.startsWith("servicearea-")) {
					let dataId = element.dataset.id as string;
					let input = element.dataset.input as string;
					let item = document.getElementById(input) as HTMLInputElement;
					let parent = element.parentNode as HTMLElement;
					item.checked = false;
					Aliss.Helpers.removeElementByNode(parent);
					toggleCount();
					// toggleDisabled();
					removeFromArray(dataId);
					hidden.value = arrayValues.toString();
					}
				});
			}
		}

		init();
	}
}

export default ServiceAreas;
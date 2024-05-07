import Aliss from '../..';
import CommGroupsTemplate from '../forms/forms.commgroup.template';

class CommGroups {
	constructor() {

		enum ID {
			filter = 'community-group-filter',
			selectedCount = 'count-community-groups-selected',
			selectedTitle = 'title-community-groups-selected',
			commGroupsSelected = 'community-groups-selected'
		}

		enum INPUT {
			checkbox = 'input[type=checkbox][name=community-groups]'
		}

		enum CLASSES {
			filter = "aliss-datainput-community-groups--filter",
			accHead = "aliss-accordion__trigger",
			accHeadActive = "aliss-accordion__trigger--active",
			accContent = "aliss-accordion__content",
			accContentActive = "aliss-accordion__content--active",
			thirdSubClass = "aliss-datainput-community-groups__sub--three",
			subClass = "aliss-datainput-community-groups__sub",
			parentClass = "aliss-datainput-community-groups__primary",
			noResult = "aliss-datainput-community-groups__noresult",
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
			updatedMinMax();
			// toggleDisabled();
			initRemoveSelected();
		}

		let temp = CommGroupsTemplate;

		const filter = document.getElementById(ID.filter) as HTMLInputElement;
		const itemsContainer = document.getElementById(ID.commGroupsSelected) as HTMLElement;
		const noresult = document.querySelector('.' + CLASSES.noResult) as HTMLElement;
		const hidden = document.getElementById('SelectedCommunityGroups') as HTMLInputElement;
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
				let minValue = document.getElementById("commgroup-" + dataId + "-minvalue") as HTMLInputElement;
				let maxValue = document.getElementById("commgroup-" + dataId + "-maxvalue") as HTMLInputElement;
				const html = temp;
				const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), item.value).replace(new RegExp(EDITS.id, 'g'), dataId.toString()).replace(EDITS.name, "community-groups").replace(EDITS.idText, "community-group");
				var domElement = document.createElement("div");
				domElement.classList.add(CLASSES.selectedItem);
				domElement.innerHTML = updatedHtml;
				itemsContainer.appendChild(domElement);
				
				if(minValue && maxValue){
					document.getElementById("commgroup-" + dataId + "-minmax-container")?.classList.remove("hidden");
					arrayValues.push(dataId + "|" + minValue.value + "|" + maxValue.value);
				}
				else{
				arrayValues.push(item.dataset.id);
			}
			}
			if(arrayValues.length > 0) {
				hidden.value = arrayValues.toString();
			}
			toggleCount();
		}

		const updatedMinMax = () => {
			var minFields = document.querySelectorAll('[id$="-minvalue"]');
			var maxFields = document.querySelectorAll('[id$="-maxvalue"]');

			for (let i = 0; i < minFields.length; i++) {
				let item = minFields[i];
				item.addEventListener("change", (el) => {
					let min = el.target as HTMLInputElement;
					let max = maxFields[i] as HTMLInputElement;
					let parentId = min.dataset.parent;
					var index = arrayValues.findIndex((arrayItem) => { return arrayItem.startsWith(parentId+"|"); }, parentId)
					if (index > -1) {
						arrayValues[index] = parentId + "|" + min.value + "|" + max.value;
					}
					hidden.value = arrayValues.toString();
				});
			}

			for (let i = 0; i < maxFields.length; i++) {
				let item = maxFields[i];
				item.addEventListener("change", (el) => {
					let max = el.target as HTMLInputElement;
					let min = minFields[i] as HTMLInputElement;
					let parentId = max.dataset.parent;
					var index = arrayValues.findIndex((arrayItem) => { return arrayItem.startsWith(parentId+"|"); }, parentId)
					if (index > -1) {
						arrayValues[index] = parentId + "|" + min.value + "|" + max.value;
					}
					hidden.value = arrayValues.toString();
				});
			}
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
				if (element.id == "community-group-" + dataId) {
					let thirdCommunityGroup = Aliss.Helpers.closest(element, ".aliss-datainput-community-groups__sub--three");
					let ifThird = Aliss.Helpers.hasClass(element, "aliss-datainput-community-groups__sub--three") ? thirdCommunityGroup : element.parentNode?.parentNode?.parentNode;
					let subCommunityGroup = Aliss.Helpers.closest(ifThird, ".aliss-datainput-community-groups__sub");
					let parentCommunityGroup = Aliss.Helpers.closest(ifThird, ".aliss-datainput-community-groups__primary");
					let i3 = thirdCommunityGroup?.querySelector(query);
					let i2 = subCommunityGroup?.querySelector(query);
					let i1 = parentCommunityGroup?.querySelector(query);
					let minValue = document.getElementById("commgroup-" + dataId + "-minvalue") as HTMLInputElement;
					let maxValue = document.getElementById("commgroup-" + dataId + "-maxvalue") as HTMLInputElement;
					if (element.checked) {
						const html = temp;
						const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), element.value).replace(new RegExp(EDITS.id, 'g'), dataId).replace(EDITS.name, "community-groups").replace(EDITS.idText, "community-group");
						var domElement = document.createElement("div");
						domElement.classList.add(CLASSES.selectedItem);
						domElement.innerHTML = updatedHtml;
						itemsContainer.appendChild(domElement);
						hidden.value = arrayValues.toString();

						if(minValue && maxValue){
							arrayValues.push(dataId + "|" + minValue.value + "|" + maxValue.value);
							document.getElementById("commgroup-" + dataId + "-minmax-container")?.classList.remove("hidden");
						}
						else{
						arrayValues.push(dataId);

						i1 != null ? Aliss.Helpers.addClass(i1, "aliss-accordion__header--selected") : null;
						i2 != null ? Aliss.Helpers.addClass(i2, "aliss-accordion__header--selected") : null;
						i3 != null ? Aliss.Helpers.addClass(i3, "aliss-accordion__header--selected") : null;
					}
					}
					else {
						let selectedItem = document.getElementById("selected-" + dataId) as HTMLButtonElement;
						let selectedItemContainer = selectedItem.parentNode as HTMLElement;
						Aliss.Helpers.removeElementByNode(selectedItemContainer);
						if(minValue && maxValue){
							document.getElementById("commgroup-" + dataId + "-minmax-container")?.classList.add("hidden");
							removeFromArray(dataId + "|" + minValue.value + "|" + maxValue.value);
						}
						else{
						i1 != null ? Aliss.Helpers.removeClass(i1, "aliss-accordion__header--selected") : null;
						i2 != null ? Aliss.Helpers.removeClass(i2, "aliss-accordion__header--selected") : null;
						i3 != null ? Aliss.Helpers.removeClass(i3, "aliss-accordion__header--selected") : null;
						removeFromArray(dataId);
					}
					}
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
					if (element.dataset.input != null && element.dataset.input.startsWith("community-group-")) {
						let dataId = element.dataset.id as string;
						let input = element.dataset.input as string;
						let item = document.getElementById(input) as HTMLInputElement;
						let parent = element.parentNode as HTMLElement;
						let minValue = document.getElementById("commgroup-" + dataId + "-minvalue") as HTMLInputElement;
						let maxValue = document.getElementById("commgroup-" + dataId + "-maxvalue") as HTMLInputElement;
						let minMaxContainer = document.getElementById("commgroup-" + dataId + "-minmax-container") as HTMLDivElement;
						item.checked = false;
						Aliss.Helpers.removeElementByNode(parent);
						toggleCount();
						// toggleDisabled();
						if(minValue && maxValue){
							removeFromArray(dataId + "|" + minValue.value + "|" + maxValue.value);
							document.getElementById("commgroup-" + dataId + "-minmax-container")?.classList.add("hidden");
						}
						else{
						removeFromArray(dataId);
						}
						hidden.value = arrayValues.toString();
					}
				});
			}
		}

		init();
	}
}

export default CommGroups;
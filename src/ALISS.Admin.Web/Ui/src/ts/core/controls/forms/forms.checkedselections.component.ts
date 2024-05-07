import Aliss from '../..';
import Template from './forms.checkedselection.template';

class CheckedSelections {
	init() {
		throw new Error('Method not implemented.');
	}
	constructor() {

		enum CLASSES {
			selectedItem = "aliss-selected__item",
			remove = "aliss-selected__remove--location"
		}

		enum EDITS {
			id = "##ID##",
			name = "##NAME##",
			value = "##VALUE##"
		}

		enum ID {
			locationsViewAllBtn = "view-selected-addresses",
			selectedLocations = "selected-locations",
			selectedLocationsContainer = "serviceSelectedLocations"
		}

		const checkboxContainer = document.querySelector('[data-checked-selection]') as HTMLElement;
		let checkboxContainerName = checkboxContainer?.dataset.name as string;
		const hiddenInput = document.getElementById(checkboxContainerName) as HTMLInputElement;
		const serviceLocationsSection = document.getElementById(ID.selectedLocationsContainer) as HTMLElement;

		var array = [] as any;
		var checkboxes = checkboxContainer?.querySelectorAll('input[type=checkbox]');

		const init = () => {
			if (checkboxContainer) {
				array = getCheckedValues();
				hiddenInput.value = array.toString();

				initRemoveSelected();
				for (var i = 0; i < checkboxes.length; i++) {
					var item = checkboxes[i] as HTMLInputElement;
					onSelectChange(item, hiddenInput);
				}
				toggleCount(array);
			}
		}

		const getCheckedValues = () => {
			return Array.from(checkboxes)
				.filter((checkbox: any) => checkbox.checked)
				.map((checkbox: any) => checkbox.value);
		}

		const onSelectChange = (checkbox: HTMLInputElement, hiddenInput: HTMLInputElement) => {
			checkbox.addEventListener(Aliss.Enums.Events.Change, function () {
				const orgSelectedCount = array.length;
				array = getCheckedValues();
				toggleViewAllSelectedAddresses(orgSelectedCount, array.length);
				toggleCount(array);
				toggleCheckBox(checkbox);
				hiddenInput.value = array.toString();
				initRemoveSelected();
			});
		}

		const toggleViewAllSelectedAddresses = (oldValue: number, newValue: number) => {
			let locationsViewAllBtn = document.getElementById(ID.locationsViewAllBtn) as HTMLElement;
			let selectedLocations = document.getElementById(ID.selectedLocations) as HTMLElement;
			if (newValue > oldValue) {
				if (selectedLocations.classList.contains("hide")) {
					selectedLocations.classList.toggle("hide");
					locationsViewAllBtn.textContent = "Hide all";
				}
			} else if (newValue === 0 && !selectedLocations.classList.contains("hide")) {
				selectedLocations.classList.add("hide");
				locationsViewAllBtn.textContent = "View all";
			}
		}

		const toggleCount = (arrayValues: any) => {
			let getString = checkboxContainer.dataset.checkedSelection as string;
			let getTitle = "title-" + getString;
			let getCount = "count-" + getString;
			const selectedCount = document.getElementById(getCount) as HTMLElement;
			const selectedTitle = document.getElementById(getTitle) as HTMLElement;
			var selectedLength = arrayValues.length;
			if (selectedCount != null) {
				if (selectedLength > 0) {
					selectedCount.innerHTML = selectedLength.toString();
					selectedTitle.removeAttribute("style");
					serviceLocationsSection.removeAttribute("style");
				} else {
					selectedCount.innerHTML = selectedLength.toString();
					selectedTitle.style.display = "none";
					serviceLocationsSection.style.display ="none";
				}
			}
		}

		const removeFromArray = (dataId: string) => {
			const index = array.indexOf(dataId);
			if (index > -1) {
				array.splice(index, 1);
			}
		}

		const initRemoveSelected = () => {
			var remove = document.querySelectorAll('.'+CLASSES.remove);
			for (var i = 0; i < remove.length; i++) {
				remove[i].addEventListener("click", (el) => {
					el.preventDefault();
					let element = el.target as HTMLButtonElement;
					let dataId = element.dataset.id as string;
					let input = element.dataset.input as string;
					let item = document.getElementById(input) as HTMLInputElement;
					let parent = element.parentNode as HTMLElement;
					item.checked = false;
					Aliss.Helpers.removeElementByNode(parent);
					removeFromArray(dataId);
					toggleCount(array);
					hiddenInput.value = array.toString();
				});
			}
		}

		const toggleCheckBox = (checkbox: any) => {
			let hiddenBoxId = checkboxContainer.dataset.checkedSelection as string;
			const hiddenBox = document.getElementById(hiddenBoxId) as HTMLInputElement;
			var cId = "check-" + checkbox.value;
			const cCheck = document.getElementById(cId) as HTMLElement;

			if (cCheck == null && checkbox.checked) {
				const html = Template;
				let selectName = checkboxContainer.dataset.name as string;
				const updatedHtml = html
					.replace(new RegExp(EDITS.value, 'g'), checkbox.dataset.label)
					.replace(new RegExp(EDITS.id, 'g'), checkbox.value)
					.replace(EDITS.name, selectName);
				var domElement = document.createElement("div");
				domElement.id = "check-" + checkbox.value;
				domElement.classList.add(CLASSES.selectedItem);
				domElement.innerHTML = updatedHtml;
				// console.log(domElement);
				hiddenBox.appendChild(domElement);
			} else if (!checkbox.checked) {
				cCheck?.remove();
			}

		}
		
		init();
	}
}

export default CheckedSelections;
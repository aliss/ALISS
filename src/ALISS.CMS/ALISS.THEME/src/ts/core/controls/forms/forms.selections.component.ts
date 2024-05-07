import Aliss from '../../';
import Template from '../forms/forms.selection.template';

class Selections {
	constructor() {

		enum CLASSES {
			selectedItem = "aliss-selected__item",
			remove = "aliss-selected__remove"
		}

		enum EDITS {
			id = "##ID##",
			name = "##NAME##",
			value = "##VALUE##"
		}

		const init = () => {
			const selectBoxes = document.querySelectorAll('[data-selection]');

			for (var i = 0; i < selectBoxes.length; i++) {				
				if (typeof(selectBoxes[i]) != 'undefined' && selectBoxes[i] != null) {
					const selectBox = selectBoxes[i] as HTMLSelectElement;
					let selectName = selectBox.dataset.name as string;
					const hiddenInput = document.getElementById(selectName) as HTMLInputElement;
					const arrayValues = [] as any;
					checkSelections(selectBox, arrayValues);
					toggleCount(selectBox, arrayValues);
					onSelectChange(selectBox, arrayValues, hiddenInput);
					removeSelectedOption(selectBox, arrayValues, hiddenInput);
				}
			}
		}

		const onSelectChange = (selectBox: HTMLSelectElement, arrayValues: any, hiddenInput: HTMLInputElement) => {
			let hiddenBoxId = selectBox.dataset.selection as string;
			const hiddenBox = document.getElementById(hiddenBoxId) as HTMLInputElement;
			selectBox.addEventListener(Aliss.Enums.Events.Change, function() {
				if(this.value != ""){
					arrayValues.push(this.value);
					this.options[this.selectedIndex].disabled = true;
					const html = Template;
					let selectName = selectBox.dataset.name as string;
					const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), this.options[this.selectedIndex].text).replace(new RegExp(EDITS.id, 'g'), this.value).replace(EDITS.name, selectName);
					var domElement = document.createElement("div");
					domElement.classList.add(CLASSES.selectedItem);
					domElement.innerHTML = updatedHtml;
					hiddenBox.appendChild(domElement);
					hiddenInput.value = arrayValues.toString();		
				}		
				toggleCount(this, arrayValues);
			});
		}

		const toggleCount = (select: HTMLSelectElement, arrayValues: any) => {
			let getString = select.dataset.selection as string;
			let getTitle = "title-" + getString;
			let getCount = "count-" + getString;
			const selectedCount = document.getElementById(getCount) as HTMLElement;
			const selectedTitle = document.getElementById(getTitle) as HTMLElement;
			var selectedLength = arrayValues.length;
			if(selectedCount != null) {
				if(selectedLength > 0) {
					selectedCount.innerHTML = selectedLength.toString();
					selectedTitle.removeAttribute("style");
				} else {
					select.options[0].selected = true;
					selectedTitle.style.display = "none";
				}
			}
		}
		
		const removeFromArray = (dataId: string, arrayValues: any) => {
			const index = arrayValues.indexOf(dataId);
			if (index > -1) {
				arrayValues.splice(index, 1);
			}	
		}

		const checkSelections = (selectBox: HTMLSelectElement, arrayValues: any) => {			
			let hiddenBoxId = selectBox.dataset.selection as string;
			const hiddenBox = document.getElementById(hiddenBoxId) as HTMLInputElement;
			for(var x = 0; x < selectBox.options?.length; x++){
				if(selectBox.options[x].disabled) {				
					arrayValues.push(selectBox.options[x].value);
					const html = Template;
					let selectName = selectBox.dataset.name as string;
					const updatedHtml = html.replace(new RegExp(EDITS.value, 'g'), selectBox.options[x].text).replace(new RegExp(EDITS.id, 'g'), selectBox.options[x].value).replace(EDITS.name, selectName);
					var domElement = document.createElement("div");
					domElement.classList.add(CLASSES.selectedItem);
					domElement.innerHTML = updatedHtml;
					hiddenBox.appendChild(domElement);
				}
			}
		}

		const removeSelectedOption = (selectBox: HTMLSelectElement, arrayValues: any, hiddenInput: HTMLInputElement) => {
			document.addEventListener(Aliss.Enums.Events.Click, (el)=> {
				let element = el.target as HTMLButtonElement;				
				if(element.classList.contains(CLASSES.remove)){
					el.preventDefault();
					let parentElement = element.parentNode as HTMLElement;
					let dataId = element.dataset.id as string;
					Aliss.Helpers.removeElementByNode(parentElement);
					for(var x = 0; x < selectBox.options?.length; x++){
						if(selectBox.options[x].value == dataId) {
							selectBox.options[x].disabled = false;
							removeFromArray(dataId, arrayValues);
							hiddenInput.value = arrayValues.toString();
						}
					}
					toggleCount(selectBox, arrayValues);
				}
			}, true);
		}

		init();
	}
}

export default Selections;
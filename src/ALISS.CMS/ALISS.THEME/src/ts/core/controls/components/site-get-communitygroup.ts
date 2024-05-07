/** @format */

class GetCommunityGroupData {
	constructor() {
		enum CLASSES {
			CommunityGroupsInputP = 'search-community-groups-primary',
			CommunityGroupCheckbox = "communityGroup-checkbox",
			CommunityGroupRangeCheckbox = "communityGroup-checkbox-range",
			SelectedCommunityGroups = 'aliss-selected-community-group',
			hide = 'hide'
		}

		enum ID {
			FullPrimaryCommunityGroupList = 'full-primary-community-group-list',
			FormSubmit = 'search-filters-submit-form',
			CommunityGroupsHidden = 'CommunityGroups',
			RemoveAllCommunityGroups = 'selected-community-groups-clear',
			RangeDiv = "range-button-",
			RangeApply = "button-",
			RangeInput = "range-",
			RangeError = "range-error-"
		}

		let communityGroupsList;

		const submitSearch = () => {
			let form = document.getElementById(ID.FormSubmit) as HTMLButtonElement;
			// form.click();
		}

		const rangeErrorCheck = (range) => {
			if(!isNaN(range)){
				if(range >= 0){
					return false;
				}
			}
			return true;
		}

		const removeSubCommunityGroup = (slug, range = null) => {
			var subCommunityGroups = '';
			var selectedCommunityGroupsInput = document.getElementById(ID.CommunityGroupsHidden) as HTMLInputElement;
			var selectedCommunityGroups: string[] = selectedCommunityGroupsInput.value.toLowerCase().split(';');

			if(range == null){
				for (let p = 0; p < communityGroupsList.data.length; p++) {
					for (let s = 0; s < communityGroupsList.data[p].sub_communitygroups.length; s++) {
						if(communityGroupsList.data[p].slug == slug && selectedCommunityGroups.includes(communityGroupsList.data[p].sub_communitygroups[s].slug)){
							if(subCommunityGroups == ''){
								subCommunityGroups += communityGroupsList.data[p].sub_communitygroups[s].slug;
							}
							else {
								subCommunityGroups += ';' + communityGroupsList.data[p].sub_communitygroups[s].slug;
							}
						}

						for (let t = 0; t < communityGroupsList.data[p].sub_communitygroups[s].sub_communitygroups.length; t++) {
							if((communityGroupsList.data[p].sub_communitygroups[s].slug == slug || communityGroupsList.data[p].slug == slug) && selectedCommunityGroups.includes(communityGroupsList.data[p].sub_communitygroups[s].sub_communitygroups[t].slug)){
								if(subCommunityGroups == ''){
									subCommunityGroups += communityGroupsList.data[p].sub_communitygroups[s].sub_communitygroups[t].slug;
								}
								else {
									subCommunityGroups += ';' + communityGroupsList.data[p].sub_communitygroups[s].sub_communitygroups[t].slug;
								}
							}
						}
					}
				}
			}
				
			var subCommunityGroupsArray: string[] = subCommunityGroups.split(';');
			var communityGroupList = '';

			for(var i = 0; i < selectedCommunityGroups.length; i++){
				
				if(selectedCommunityGroups[i] != slug && !subCommunityGroupsArray.includes(selectedCommunityGroups[i])){
					if(communityGroupList == '' && (range == null || range == "")){
						communityGroupList += selectedCommunityGroups[i];
					}
					else if(range == null || range == "") {
						communityGroupList += ';' + selectedCommunityGroups[i];
					}
					else if(communityGroupList == ''){
						selectedCommunityGroupsInput.value += selectedCommunityGroups[i] + '|' + range;
					}
					else {
						selectedCommunityGroupsInput.value += ';' + selectedCommunityGroups[i] + '|' + range;
					}
				}
			}
			
			return communityGroupList;
		}

		const updateCommunityGroups = (node : HTMLInputElement, range: any = null) => {
			console.log('UPDATE COMMUNITY GROUPS');
			console.log(node);
			var selectedCommunityGroupsInput = document.getElementById(ID.CommunityGroupsHidden) as HTMLInputElement;
			var selectedCommunityGroupsList = selectedCommunityGroupsInput.value.toLowerCase().split(';');
			var index = selectedCommunityGroupsList.findIndex((arrayItem) => { return arrayItem.startsWith(node.value); }, node.value)
			if(node.checked) {
				if(index > -1){
					if(range == null || range == ""){
						selectedCommunityGroupsList[index] = node.value;
						selectedCommunityGroupsInput.value = selectedCommunityGroupsList.join(';');
					}
					else{
						selectedCommunityGroupsList[index] = node.value + '|' + range;
						selectedCommunityGroupsInput.value = selectedCommunityGroupsList.join(';');
					}
				}
				else {
					if(selectedCommunityGroupsInput.value == '' && (range == null || range == "")){
						selectedCommunityGroupsInput.value += node.value;
					}
					else if (range == null || range == "") {
						selectedCommunityGroupsInput.value += ';' + node.value;
					}
					else if(selectedCommunityGroupsInput.value == ''){
						selectedCommunityGroupsInput.value += node.value + '|' + range;
					}
					else {
						selectedCommunityGroupsInput.value += ';' + node.value + '|' + range;
					}
				}
			}
			else{
				selectedCommunityGroupsInput.value = removeSubCommunityGroup(node.value, range);
			}
		}

		const checkboxDisplay = () => {
			var request = new XMLHttpRequest();
			request.open('GET', apiBaseUrl + 'community-groups', true);

			request.onload = function() {
				if (this.status >= 200 && this.status < 400) {
					// Success!
					var data = JSON.parse(this.response);
					communityGroupsList = data;

					var selectedCommunityGroupsInput = document.getElementById(ID.CommunityGroupsHidden) as HTMLInputElement;
					var selectedCommunityGroups: string[] = selectedCommunityGroupsInput.value.toLowerCase().split(';');
					
					for(var p = 0; p < data.data.length; p++){
						var index = selectedCommunityGroups.findIndex((arrayItem) => { return arrayItem.startsWith(data.data[p].slug + "|"); }, data.data[p].slug)
						if(selectedCommunityGroups.includes(data.data[p].slug) || index != -1){
							var checkboxPrimary = document.getElementById('checkbox-' + data.data[p].slug) as HTMLInputElement;
							checkboxPrimary.checked = true;

							if(data.data[p].isrange) {
								let rangeSubmit = document.getElementById(ID.RangeDiv + data.data[p].slug) as HTMLDivElement;
								rangeSubmit.classList.remove(CLASSES.hide);
								rangeSubmit.hidden = false;

								let rangeInput = document.getElementById(ID.RangeInput + data.data[p].slug) as HTMLButtonElement;
								rangeInput.value = selectedCommunityGroups[index].split('|')[1];
							}

							var secondaryCommunityGroups = document.getElementById(data.data[p].slug + "-communityGroups") as HTMLFieldSetElement;
							if(secondaryCommunityGroups){
								secondaryCommunityGroups.classList.remove(CLASSES.hide);
								secondaryCommunityGroups.hidden = false;
							}
						}

						for(var s = 0; s < data.data[p].sub_communitygroups.length; s++){
							if(selectedCommunityGroups.includes(data.data[p].sub_communitygroups[s].slug)){
								var checkboxPrimary = document.getElementById('checkbox-' + data.data[p].slug) as HTMLInputElement;
								checkboxPrimary.checked = true;

								var checkboxSecondary = document.getElementById('checkbox-' + data.data[p].sub_communitygroups[s].slug) as HTMLInputElement;
								checkboxSecondary.checked = true;

								var secondaryCommunityGroups = document.getElementById(data.data[p].slug + "-communityGroups") as HTMLFieldSetElement;
								if(secondaryCommunityGroups){
									secondaryCommunityGroups.classList.remove(CLASSES.hide);
									secondaryCommunityGroups.hidden = false;
								}

								var tertiaryCommunityGroups = document.getElementById(data.data[p].sub_communitygroups[s].slug + "-communityGroupt") as HTMLFieldSetElement;
								if(tertiaryCommunityGroups){
									tertiaryCommunityGroups.classList.remove(CLASSES.hide);
									tertiaryCommunityGroups.hidden = false;
								}
							}

							for(var t = 0; t < data.data[p].sub_communitygroups[s].sub_communitygroups.length; t++){
								if(selectedCommunityGroups.includes(data.data[p].sub_communitygroups[s].sub_communitygroups[t].slug)){
									var checkboxPrimary = document.getElementById('checkbox-' + data.data[p].slug) as HTMLInputElement;
									checkboxPrimary.checked = true;
	
									var checkboxSecondary = document.getElementById('checkbox-' + data.data[p].sub_communitygroups[s].slug) as HTMLInputElement;
									checkboxSecondary.checked = true;
	
									var secondaryCommunityGroups = document.getElementById(data.data[p].slug + "-communityGroups") as HTMLFieldSetElement;
									if(secondaryCommunityGroups){
										secondaryCommunityGroups.classList.remove(CLASSES.hide);
										secondaryCommunityGroups.hidden = false;
									}

									var checkboxTertiary = document.getElementById('checkbox-' + data.data[p].sub_communitygroups[s].sub_communitygroups[t].slug) as HTMLInputElement;
									checkboxTertiary.checked = true;
	
									var tertiaryCommunityGroups = document.getElementById(data.data[p].sub_communitygroups[s].slug + "-communityGroupt") as HTMLFieldSetElement;
									if(tertiaryCommunityGroups){
										tertiaryCommunityGroups.classList.remove(CLASSES.hide);
										tertiaryCommunityGroups.hidden = false;
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
			let input = document.querySelectorAll('.' + CLASSES.CommunityGroupCheckbox) as NodeList;
			for(var i = 0; i < input.length; i++) {
				let checkbox = input[i] as HTMLInputElement;
				checkbox.addEventListener('change', ()=> {
					updateCommunityGroups(checkbox);
					submitSearch();
				})
			}

			let rangeInputs = document.querySelectorAll('.' + CLASSES.CommunityGroupRangeCheckbox) as NodeList;
			for(var i = 0; i < rangeInputs.length; i++) {
				let rangeCheckbox = rangeInputs[i] as HTMLInputElement;
				let applyRange = document.getElementById(ID.RangeApply + rangeCheckbox.value) as HTMLButtonElement;
				let rangeSubmit = document.getElementById(ID.RangeDiv + rangeCheckbox.value) as HTMLDivElement;

				rangeCheckbox.addEventListener('change', ()=> {
					rangeSubmit.hidden = !rangeCheckbox.checked

					if(rangeCheckbox.checked == false){
						let rangeInput = document.getElementById(ID.RangeInput + rangeCheckbox.value) as HTMLButtonElement;
						updateCommunityGroups(rangeCheckbox, rangeInput.value);
						submitSearch();
					}
				})

				applyRange.addEventListener('click', (el) => {
					el.preventDefault();
					let rangeInput = document.getElementById(ID.RangeInput + rangeCheckbox.value) as HTMLButtonElement;
					
					if(rangeErrorCheck(rangeInput.value)){
						var rangeErrorText = document.getElementById(ID.RangeError + rangeCheckbox.value) as HTMLLabelElement;
						if(!rangeErrorText){
							rangeSubmit.classList.add("aliss-form__input-container--error");
							var errorText = document.createElement('p');
							errorText.textContent = "Please insert a positive number to search by range";
							errorText.id = ID.RangeError + rangeCheckbox.value;
							rangeSubmit.appendChild(errorText);
						}

					}
					else{
						updateCommunityGroups(rangeCheckbox, rangeInput.value);
						submitSearch();
					}
				})
			}

			let communityGroups = document.getElementById(ID.CommunityGroupsHidden) as HTMLInputElement;
			let removeAllCommunityGroups = document.getElementById(ID.RemoveAllCommunityGroups) as HTMLButtonElement;
			if(removeAllCommunityGroups){
				removeAllCommunityGroups.addEventListener('click', (el) => {
					el.preventDefault();
					communityGroups.value = '';
					submitSearch();
				})
			}

			let selectedCommunityGroupList = document.querySelectorAll('.' + CLASSES.SelectedCommunityGroups) as NodeList;
			selectedCommunityGroupList.forEach(communityGroup => {
				let removeCommunityGroup = communityGroup as HTMLLabelElement;
				removeCommunityGroup.addEventListener('click', () => {
					let communityGroupList = communityGroups.value.toLowerCase().split(';');
					var slug = communityGroup.id?.split('--').pop();
					for(var i = 0; i < communityGroupList.length; i++){
						if(communityGroupList[i] == slug){
							let updateCommunityGroups = communityGroups.value.split(';');
							if(updateCommunityGroups.length > 1){
								var communityGroupIndex = updateCommunityGroups.indexOf(slug);
								updateCommunityGroups.splice(communityGroupIndex, 1);
								communityGroups.value = updateCommunityGroups.join(';');
								communityGroups.value = removeSubCommunityGroup(slug);
							}
							else{
								communityGroups.value = '';
							}
							submitSearch();
						}
					}
				})
			});
		};

		!!document.querySelector('.' + CLASSES.CommunityGroupsInputP) && init();
	}
}

export default GetCommunityGroupData;

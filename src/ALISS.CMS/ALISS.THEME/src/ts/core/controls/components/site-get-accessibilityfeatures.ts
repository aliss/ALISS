/** @format */

class GetAccessibilityFeatureData {
	constructor() {
		enum CLASSES {
			AccessibilityFeaturesInputP = 'search-accessibility-features-primary',
			AccessibilityFeatureCheckbox = "accessibilityFeature-checkbox",
			SelectedAccessibilityFeatures = 'aliss-selected-accessibility-feature'
		}

		enum ID {
			FullPrimaryAccessibilityFeatureList = 'full-primary-accessibility-feature-list',
			FormSubmit = 'search-filters-submit-form',
			AccessibilityFeaturesHidden = 'AccessibilityFeatures',
			RemoveAllAccessibilityFeatures = 'selected-accessibility-features-clear',
		}

		let accessibilityFeaturesList;

		const submitSearch = () => {
			let form = document.getElementById(ID.FormSubmit) as HTMLButtonElement;
			// form.click();
		}

		const removeAccessibilityFeature = (slug) => {
			var selectedAccessibilityFeaturesInput = document.getElementById(ID.AccessibilityFeaturesHidden) as HTMLInputElement;
			var selectedAccessibilityFeatures: string[] = selectedAccessibilityFeaturesInput.value.split(';');
			
			var accessibilityFeatureList = '';
			for(var i = 0; i < selectedAccessibilityFeatures.length; i++){
				if(selectedAccessibilityFeatures[i] != slug){
					if(accessibilityFeatureList == ''){
						accessibilityFeatureList += selectedAccessibilityFeatures[i];
					}
					else {
						accessibilityFeatureList += ';' + selectedAccessibilityFeatures[i];
					}
				}
			}
			
			return accessibilityFeatureList;
		}

		const updateAccessibilityFeatures = (node : HTMLInputElement) => {
			console.log('UPDATE ACCESSIBILITY FEATURES');
			console.log(node);
			var selectedAccessibilityFeaturesInput = document.getElementById(ID.AccessibilityFeaturesHidden) as HTMLInputElement;
			if(node.checked) {
				if(selectedAccessibilityFeaturesInput.value == ''){
					selectedAccessibilityFeaturesInput.value += node.value;
				}
				else {
					selectedAccessibilityFeaturesInput.value += ';' + node.value;
				}
			}
			else{
				selectedAccessibilityFeaturesInput.value = removeAccessibilityFeature(node.value);
			}
		}

		const checkboxDisplay = () => {
			var request = new XMLHttpRequest();
			request.open('GET', apiBaseUrl + 'accessibility-features', true);

			request.onload = function() {
				if (this.status >= 200 && this.status < 400) {
					// Success!
					var data = JSON.parse(this.response);
					accessibilityFeaturesList = data;

					var selectedAccessibilityFeaturesInput = document.getElementById(ID.AccessibilityFeaturesHidden) as HTMLInputElement;
					var selectedAccessibilityFeatures: string[] = selectedAccessibilityFeaturesInput.value.split(';');

					for(var p = 0; p < data.data.length; p++){
						if(selectedAccessibilityFeatures.includes(data.data[p].slug)){
							var checkboxPrimary = document.getElementById('checkbox-' + data.data[p].slug) as HTMLInputElement;
							checkboxPrimary.checked = true;
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
			let input = document.querySelectorAll('.' + CLASSES.AccessibilityFeatureCheckbox) as NodeList;
			for(var i = 0; i < input.length; i++) {
				let checkbox = input[i] as HTMLInputElement;
				checkbox.addEventListener('change', ()=> {
					updateAccessibilityFeatures(checkbox);
					submitSearch();
				})
			}

			let accessibilityFeatures = document.getElementById(ID.AccessibilityFeaturesHidden) as HTMLInputElement;
			let removeAllAccessibilityFeatures = document.getElementById(ID.RemoveAllAccessibilityFeatures) as HTMLButtonElement;
			if(removeAllAccessibilityFeatures){
				removeAllAccessibilityFeatures.addEventListener('click', (el) => {
					el.preventDefault();
					accessibilityFeatures.value = '';
					submitSearch();
				})
			}

			let selectedAccessibilityFeatureList = document.querySelectorAll('.' + CLASSES.SelectedAccessibilityFeatures) as NodeList;
			selectedAccessibilityFeatureList.forEach(accessibilityFeature => {
				let removeAccessibilityFeatureLabel = accessibilityFeature as HTMLLabelElement;
				removeAccessibilityFeatureLabel.addEventListener('click', () => {
					let accessibilityFeatureList = accessibilityFeatures.value.split(';');
					var slug = accessibilityFeature.id?.split('--').pop();
					for(var i = 0; i < accessibilityFeatureList.length; i++){
						if(accessibilityFeatureList[i] == slug){
							let updateAccessibilityFeatures = accessibilityFeatures.value.split(';');
							if(updateAccessibilityFeatures.length > 1){
								var accessibilityFeatureIndex = updateAccessibilityFeatures.indexOf(slug);
								updateAccessibilityFeatures.splice(accessibilityFeatureIndex, 1);
								accessibilityFeatures.value = updateAccessibilityFeatures.join(';');
								accessibilityFeatures.value = removeAccessibilityFeature(slug);
							}
							else{
								accessibilityFeatures.value = '';
							}
							submitSearch();
						}
					}
				})
			});
		};

		!!document.querySelector('.' + CLASSES.AccessibilityFeaturesInputP) && init();
	}
}

export default GetAccessibilityFeatureData;

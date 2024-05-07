import Aliss from '../../';

class DataInputWhere {
	constructor() {

		enum ID {
			HowServiceAccessed = 'HowServiceAccessedFieldset',
            serviceLocationsAndRegions = 'serviceLocationsAndRegions',
            whereIsYourServiceLocated = 'whereIsYourServiceLocated',
            serviceSelectionsFieldset = 'serviceSelectionsFieldset',
            serviceSelectedLocations = 'serviceSelectedLocations',
            serviceSelectedRegions = 'serviceSelectedRegions',
            serviceLocationsAndRegionsTabs = 'serviceLocationsAndRegionsTabs',
            locationsFieldset = 'locationsFieldset',
            regionsFieldset = 'regionsFieldset',
			addLocationsTab = 'add-locations',

			inPersonWhereIsYourServiceText = 'inperson-where-is-your-service-text',
			hybridWhereIsYourServiceText = 'hybrid-where-is-your-service-text',
			inPersonAddressDetailsText = 'inperson-address-details-text',
			hybridAddressDetailsText = 'hybrid-address-details-text',
			inPersonRegionDetailsText = 'inperson-region-details-text',
			hybridRegionDetailsText = 'hybrid-region-details-text',

			manualAddressCheckbox = 'manual-address',
			manualAddressContainer = 'manualAddressContainer'
		}

        enum CLASSES {
            serviceAccess = '.aliss-form__input__service-access',
            hideTabs = 'aliss-tabs--hide',
            hideElement = 'hide'
        }

		enum INPUT {
			checkbox = 'input[type=checkbox][name=HowServiceAccessed]',
		}

		enum ATTRIBUTES {
			AriaHidden = 'aria-hidden'
		}

		const serviceAccessOptions = document.querySelectorAll(CLASSES.serviceAccess);
		const serviceAccessOptionsContainer = document.getElementById(ID.HowServiceAccessed) as HTMLElement;
		const serviceSelectedLocations = document.getElementById(ID.serviceSelectedLocations) as HTMLElement;
		const serviceLocationsAndRegions = document.getElementById(ID.serviceLocationsAndRegions) as HTMLElement;
		const serviceLocationsAndRegionsTabs = document.getElementById(ID.serviceLocationsAndRegionsTabs) as HTMLElement;
		const locationsFieldset = document.getElementById(ID.locationsFieldset) as HTMLElement;
		const regionsFieldset = document.getElementById(ID.regionsFieldset) as HTMLElement;
		const addLocationsTab = document.getElementById(ID.addLocationsTab) as HTMLInputElement;

		const whereIsYourServiceLocatedContainer = document.getElementById(ID.whereIsYourServiceLocated) as HTMLElement;
		const inPersonWhereIsYourServiceText = document.getElementById(ID.inPersonWhereIsYourServiceText) as HTMLParagraphElement;
		const hybridWhereIsYourServiceText = document.getElementById(ID.hybridWhereIsYourServiceText) as HTMLParagraphElement;
		const inPersonAddressDetailsText = document.getElementById(ID.inPersonAddressDetailsText) as HTMLParagraphElement;
		const hybridAddressDetailsText = document.getElementById(ID.hybridAddressDetailsText) as HTMLParagraphElement;
		const inPersonRegionDetailsText = document.getElementById(ID.inPersonRegionDetailsText) as HTMLParagraphElement;
		const hybridRegionDetailsText = document.getElementById(ID.hybridRegionDetailsText) as HTMLParagraphElement;

		const manualAddressCheckbox = document.getElementById(ID.manualAddressCheckbox) as HTMLElement;
		const manualAddressContainer = document.getElementById(ID.manualAddressContainer) as HTMLElement;

		const init = () => {
			getSelectedServiceAccess();
			onSelectChange();

			if (manualAddressCheckbox) {
				manualAddressCheckbox.onchange = function (e) {
					if (manualAddressContainer.classList.contains(CLASSES.hideElement)) {
						manualAddressContainer.classList.remove(CLASSES.hideElement);
					} else {
						manualAddressContainer.classList.add(CLASSES.hideElement);
					}
				};
			}
		}

		const getSelectedServiceAccess = () => {
			if (serviceAccessOptionsContainer != null) {
				serviceAccessOptions.forEach((e: any) => {
					if (e.checked) {
						showServiceAccessFlow(e.value);
					}
				});
			}
		}

		const onSelectChange = () => {
			serviceAccessOptions.forEach((element: any) => {
				element.addEventListener('change', function() {
					showServiceAccessFlow(element.value);
				});
			});
		}

		const showTabs = () => {
			addLocationsTab.checked = true;
			serviceLocationsAndRegionsTabs.classList.remove(CLASSES.hideTabs);
			locationsFieldset.classList.remove(CLASSES.hideElement);
			regionsFieldset.classList.add(CLASSES.hideElement);
		}

		const showServiceAccessFlow = (serviceAccess: any) => {
			if (serviceAccess === "remote") {
				whereIsYourServiceLocatedContainer.classList.add(CLASSES.hideElement);
				serviceSelectedLocations.classList.add(CLASSES.hideElement);
				serviceLocationsAndRegionsTabs.classList.add(CLASSES.hideTabs);
				locationsFieldset.classList.add(CLASSES.hideElement);
				regionsFieldset.classList.remove(CLASSES.hideElement);
			} else if (serviceAccess === "hybrid") {
				showTabs();
				whereIsYourServiceLocatedContainer.classList.remove(CLASSES.hideElement);
				inPersonWhereIsYourServiceText.classList.add(CLASSES.hideElement);
				inPersonAddressDetailsText.classList.add(CLASSES.hideElement);
				inPersonRegionDetailsText.classList.add(CLASSES.hideElement);
				hybridWhereIsYourServiceText.classList.remove(CLASSES.hideElement);
				hybridAddressDetailsText.classList.remove(CLASSES.hideElement);
				hybridRegionDetailsText.classList.remove(CLASSES.hideElement);
				serviceSelectedLocations.classList.remove(CLASSES.hideElement);
			} else if (serviceAccess === "inPerson") {
				showTabs();
				whereIsYourServiceLocatedContainer.classList.remove(CLASSES.hideElement);
				inPersonWhereIsYourServiceText.classList.remove(CLASSES.hideElement);
				inPersonAddressDetailsText.classList.remove(CLASSES.hideElement);
				inPersonRegionDetailsText.classList.remove(CLASSES.hideElement);
				hybridWhereIsYourServiceText.classList.add(CLASSES.hideElement);
				hybridAddressDetailsText.classList.add(CLASSES.hideElement);
				hybridRegionDetailsText.classList.add(CLASSES.hideElement);
				serviceSelectedLocations.classList.remove(CLASSES.hideElement);
			}
			serviceLocationsAndRegions.classList.remove(CLASSES.hideElement);
		}

		init();
	}
}

export default DataInputWhere;
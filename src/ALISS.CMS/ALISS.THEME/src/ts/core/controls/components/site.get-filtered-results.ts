/** @format */
import Aliss from '../..';
import Template from '../components/site-get-filtered-results-template';
import FilterTagTemplate from '../components/site-filter-tag-template';

class GetFilteredResults {
	constructor() {
		enum CLASSES {
			radioInput = "aliss-form__radio input",
			checkboxInput = "aliss-form__checkbox input",
			checkboxInitialInput = "aliss-form__checkbox--initial",
			buttonInput = "aliss-form__input-container button",
			hide = "hide",
			searchItemContainer = "aliss-organism aliss-component-master aliss-component-master__search-result-item aliss-content-spacer",
			searchItemHeader = "aliss-component-master__contents aliss-component-master__contents__search-results",
			searchItemLinksContainer = "aliss-component-master__links aliss-component-master__links__search-results",
			SearchItemLinksIconList = "aliss-icon-list",
			GridWidth = 'aliss-component-master__search-result-item--grid-layout',
			inputCategorySecondary = 'aliss-form__select search-category-secondary',
			categoryInput = 'aliss-form__input category-checkbox',
			filterTagContainer = 'aliss-form__selected-value-tag-container',
			filterTag = 'aliss-form__selected-value-tag',
			clearSelected = 'aliss-form__clear-selected',
			radioLabelDisabled = 'aliss-form__radio__label--disabled',
			inputDisabled = 'aliss-form__input--disabled',
			deprioritisedServiceItem = 'aliss-deprioritised-service__item',
			deprioritisedServicesMessage = 'aliss-deprioritised-service__message',
			marginBottom15 = 'mb--15',
			accordionActive = 'aliss-accordion__content--active',
			accordionTrigger = 'aliss-accordion__trigger',
			accordionTriggerActive = 'aliss-accordion__trigger--active'
		}

		enum ID {
			servicesSortFilter = "services-sort",
			whereFilter = "where-filter",
			whatFilter = "what-filter",
			distanceFilter = "distance-filter",
			locNatFilter = "loc-nat-filter",
			categoriesFilter = "search-categories",
			whoFilter = "search-community-groups",
			accessibilityFilter = "search-accessibility-features",
			postcodeInput = "search-postcode",
			selectedPostcodeContainer = "aliss-selected-postcode-container",
			selectedPostcode = "selected-postcode",
			selectedPostcodeValue = "selected-postcode-value",
			postcodeSubmitSearchBtn = "search-button-postcode",
			whatInput = "filter-what-entry",
			selectedWhatContainer = "aliss-selected-what-container",
			selectedWhat = "selected-what",
			selectedWhatValue = "selected-what-value",
			whatSubmitSearchBtn = "filter-what-submit",
			resultsTotal = "search-results-total",
			resultsPageTotal = "search-results-page-count",
			pageNavigationInput = "page-navigation-input",
			filtersPanel = "filters-panel",
			searchPageNavigation = "aliss-search-page-navigation",
			loader = "search-results-loading",
			searchResultsContainer = "aliss-search-results-filtered",
			ListView = 'list-view',
			GridView = 'grid-view',
			FormSubmit = 'search-filters-submit-form',
			selectedCategories = 'display-selected-categories',
			selectedCommunityGroups = 'display-selected-community-groups',
			selectedAccessibilityFeatures = 'display-selected-accessibility-features',
			clearAllCommunityGroupsSelections = 'selected-community-groups-clear',
			clearAllCategorySelections = 'selected-categories-clear',
			clearAllAccessibilityFeaturesSelections = 'selected-accessibility-features-clear',
			previousLink = 'services-search-results-previous-link',
			nextLink = 'services-search-results-next-link',
			previousPage = 'aliss-search-previous-page',
			nextPage = 'aliss-search-next-page',
			view = 'View',
			headerText = 'aliss-search-results-header-text',
			sortDistanceInput = 'SortDistance',
			sortCustomInput = 'Radius4',
			sortDistanceLabel = 'SortDistance-label',
			sortCustomDistanceLabel = 'custom-radius-input-value',
			sortLastReviewedInput = 'SortLastReviewed',
			sortRelevanceInput = 'SortRelevance',
			sortRelevanceLabel = 'SortRelevance-label',
			defaultRadius = 'Radius2',
			customDistanceSubmitButton = 'custom-distance-button',
			pageNavigationGoBtn = 'page-navigation-button',
			viewResultsBtn = 'search-filters-view-results',
			defaultPaginationPrevious = 'search-results-page-previous',
			defaultPaginationNext = 'search-results-page-next',
			servicesPaginationPrevious = 'services-search-results-page-previous',
			servicesPaginationNext = 'services-search-results-page-next',
			filterListToggleCategory = 'aliss-search-filter-list-toggle-category',
			filterListToggleWho = 'aliss-search-filter-list-toggle-Who',
			filterListToggleAccessibility = 'aliss-search-filter-list-toggle-accessibility',
			fullPrimaryCategoryList = 'full-primary-category-list',
			fullPrimaryCommunityGroupList = 'full-primary-community-group-list',
			fullPrimaryAccessibilityFeatureList = 'full-primary-accessibility-feature-list'
		}

		enum EDITS {
			id = "##ID##",
			name = "##NAME##",
			slug = "##SLUG##",
			orgSlug = "##ORGSLUG##",
			orgName = "##ORGNAME##",
			summary = "##SUMMARY##",
			weblink = "##WEB##",
			referral = "##REF##",
			phone = "##PHONE##",
			email = "##EMAIL##",
			dataName = "##DATANAME##",
			postcode = "##POSTCODE##",
			what = "##WHAT##",
			reviewDate = "##LASTREVIEWED##"
		}

		enum KEYS {
			enter = 'Enter'
		}

		const html = Template;
		const filterTagHTML = FilterTagTemplate;
		let url = '';
		let pageTotal: any;

		let allCategories = [] as any;
		let allCommunityGroups = [] as any;
		let allAccessibilityFeatures = [] as any;
		let allAggregations = [] as any;

		let filtersPanel = document.getElementById(ID.filtersPanel) as HTMLFormElement;
		let headerText = document.getElementById(ID.headerText) as HTMLElement;
		let loader = document.getElementById(ID.loader) as HTMLElement;
		let searchPageNavigation = document.getElementById(ID.searchPageNavigation) as HTMLElement;
		let sortBy = document.getElementById(ID.servicesSortFilter) as HTMLElement;
		let where = document.getElementById(ID.whereFilter) as HTMLElement;
		let whereSubmit = document.getElementById(ID.postcodeSubmitSearchBtn) as HTMLButtonElement;
		let what = document.getElementById(ID.whatFilter) as HTMLElement;
		let whatSubmit = document.getElementById(ID.whatSubmitSearchBtn) as HTMLButtonElement;
		let distance = document.getElementById(ID.distanceFilter) as HTMLElement;
		let coverage = document.getElementById(ID.locNatFilter) as HTMLElement;
		let categories = document.getElementById(ID.categoriesFilter) as HTMLElement;
		let who = document.getElementById(ID.whoFilter) as HTMLElement;
		let accessibility = document.getElementById(ID.accessibilityFilter) as HTMLElement;
		let ListView = document.getElementById(ID.ListView) as HTMLElement;
		let GridView = document.getElementById(ID.GridView) as HTMLElement;
		let viewValue = document.getElementById(ID.view) as HTMLInputElement;
		let selectedCategories = document.getElementById(ID.selectedCategories) as HTMLElement;
		let selectedCommunityGroups = document.getElementById(ID.selectedCommunityGroups) as HTMLElement;
		let selectedAccessibilityFeatures = document.getElementById(ID.selectedAccessibilityFeatures) as HTMLElement;
		let viewResultsBtn = document.getElementById(ID.viewResultsBtn) as HTMLButtonElement;
		let customDistanceSubmitButton = document.getElementById(ID.customDistanceSubmitButton) as HTMLElement;

		let clearAllCommunityGroupsSelections = document.getElementById(ID.clearAllCommunityGroupsSelections) as HTMLElement;
		let communityGroupsFilterTagContainer = selectedCommunityGroups?.querySelector('.' + CLASSES.filterTagContainer) as HTMLElement;
		let filterListToggleWho = document.getElementById(ID.filterListToggleWho) as HTMLElement;
		let fullPrimaryCommunityGroupList = document.getElementById(ID.fullPrimaryCommunityGroupList) as HTMLElement;
		let whoInputs = who?.querySelectorAll('.' + CLASSES.checkboxInput) as NodeList;
		let whoInitialInputs = who?.querySelectorAll('.' + CLASSES.checkboxInitialInput) as NodeList;

		let clearAllCategorySelections = document.getElementById(ID.clearAllCategorySelections) as HTMLElement;
		let categoryFilterTagContainer = selectedCategories?.querySelector('.' + CLASSES.filterTagContainer) as HTMLElement;
		let filterListToggleCategory = document.getElementById(ID.filterListToggleCategory) as HTMLElement;
		let fullPrimaryCategoryList = document.getElementById(ID.fullPrimaryCategoryList) as HTMLElement;
		let categoryInputs = categories?.querySelectorAll('.' + CLASSES.checkboxInput) as NodeList;
		let categoryInitialInputs = categories?.querySelectorAll('.' + CLASSES.checkboxInitialInput) as NodeList;

		let clearAllAccessibilityFeaturesSelections = document.getElementById(ID.clearAllAccessibilityFeaturesSelections) as HTMLElement;
		let accessibilityFeaturesFilterTagContainer = selectedAccessibilityFeatures?.querySelector('.' + CLASSES.filterTagContainer) as HTMLElement;
		let filterListToggleAccessibility = document.getElementById(ID.filterListToggleAccessibility) as HTMLElement;
		let fullPrimaryAccessibilityFeatureList = document.getElementById(ID.fullPrimaryAccessibilityFeatureList) as HTMLElement;
		let accessibilityInputs = accessibility?.querySelectorAll('.' + CLASSES.checkboxInput) as NodeList;
		let accessibilityInitialInputs = accessibility?.querySelectorAll('.' + CLASSES.checkboxInitialInput) as NodeList;

		let defaultPaginationPrevious = document.getElementById(ID.defaultPaginationPrevious);
		let defaultPaginationNext = document.getElementById(ID.defaultPaginationNext);
		let servicesPaginationPrevious = document.getElementById(ID.servicesPaginationPrevious) as HTMLElement;
		let servicesPaginationNext = document.getElementById(ID.servicesPaginationNext) as HTMLElement;

		let previousLink = document.getElementById(ID.previousLink) as HTMLLinkElement;
		let nextLink = document.getElementById(ID.nextLink) as HTMLLinkElement;
		let pageNavigationInput = document.getElementById(ID.pageNavigationInput) as HTMLInputElement;
		let pageNumber = Number(pageNavigationInput?.placeholder);

		let pageNavigationGoBtn = document.getElementById(ID.pageNavigationGoBtn) as HTMLButtonElement;

		let _selectedCats = document.querySelectorAll('.selected-cats') as any;
		let _selectedCommunityGroups = document.querySelectorAll('.selected-comm-groups') as any;
		let _selectedAccessibilityFeatures = document.querySelectorAll('.selected-acc-features') as any;

		let filters = {
			"sort": '' as string,
			"postcode": '' as string,
			"what": '' as string,
			"radius": '10000' as string,
			"categories": [] as Array<string>,
			"communityGroups": [] as Array<string>,
			"accessibilityFeatures": [] as Array<string>,
			"locationType": '' as string,
			"view": '' as string,
			"pageNumber": '' as string,
			"pageSize": window.pageSize
		}

		const init = () => {
			if (filtersPanel != null) {
				filters.pageNumber = pageNumber.toString();
				filters.view = viewValue?.value;
				updateCommunityGroupsOnLoad();
				updateCategoriesOnLoad();
				updateAccessibilityFeaturesOnLoad();
				checkInputField(where, ID.postcodeInput, ID.selectedPostcode);
				checkInputField(what, ID.whatInput, ID.selectedWhat);
				checkForInputChange(sortBy, distance, categories, who, accessibility, coverage);
				checkSearchResultsView(ListView, GridView);
				paginationGoClicked();

				if (filters.postcode === '') {
					let radiusDefaultInput = document.getElementById(ID.defaultRadius) as HTMLInputElement;
					radiusDefaultInput?.click();
					updateUrl();
				}

				defaultPaginationPrevious?.classList.add(CLASSES.hide);
				defaultPaginationNext?.classList.add(CLASSES.hide);
				servicesPaginationPrevious.classList.remove(CLASSES.hide);
				servicesPaginationNext.classList.remove(CLASSES.hide);

				window.addEventListener('load', function () {
					adjustLayoutForMobile();
				}, true);

				window.addEventListener('resize', function () {
					adjustLayoutForMobile();
				}, true);

			}
		};

		const adjustLayoutForMobile = () => {
			if (window.innerWidth <= 992 && viewResultsBtn != null) {
				whereSubmit?.classList.add(CLASSES.hide);
				whatSubmit?.classList.add(CLASSES.hide);
				viewResultsBtn.addEventListener('click', (e) => {
					e.preventDefault();
					whereSubmit?.click();
					whatSubmit?.click();
				});
			}
			else {
				whereSubmit?.classList.remove(CLASSES.hide);
				whatSubmit?.classList.remove(CLASSES.hide);
			}
		}

		const paginationGoClicked = () => {
			if (pageNavigationGoBtn != null && filtersPanel != null) {
				pageNavigationGoBtn.addEventListener('click', (e) => {
					e.preventDefault();
					pageNumber = Number(pageNavigationInput.value);
					if (pageNavigationInput.value == "" || isNaN(pageNumber)) {
						pageNumber = Number(pageNavigationInput.placeholder);
					}
					else if (pageNumber < 1) {
						pageNumber = 1;
					} else if (pageNumber > pageTotal) {
						pageNumber = pageTotal;
					}
					filters.pageNumber = pageNumber.toString();
					updateUrl();
					window.location.href = url;
				});
			}
		}

		const updateCommunityGroupsOnLoad = () => {
			if (_selectedCommunityGroups.length > 0) {
				clearAllCommunityGroupsSelections.classList.remove(CLASSES.hide);
			}
			for (let s = 0; s < _selectedCommunityGroups.length; s++) {
				let selected = _selectedCommunityGroups[s];
				let value = selected.innerHTML;
				let selectedInput = document.getElementById('checkbox-' + value) as HTMLInputElement;
				filters.communityGroups.push(value);
				toggleClearFilterBtn(filters.communityGroups, whoInputs, clearAllCommunityGroupsSelections);
				handleFilterTag(selectedInput, value, filters.communityGroups, 'communityGroup');
			}
		}

		const updateCategoriesOnLoad = () => {
			if (_selectedCats.length > 0) {
				clearAllCategorySelections.classList.remove(CLASSES.hide);
			}
			for (let s = 0; s < _selectedCats.length; s++) {
				let selected = _selectedCats[s];
				let value = selected.innerHTML;
				let selectedInput = document.getElementById('checkbox-' + value) as HTMLInputElement;
				filters.categories.push(value);
				toggleClearFilterBtn(filters.categories, categoryInputs, clearAllCategorySelections);
				handleFilterTag(selectedInput, value, filters.categories, 'category');
			}
		}

		const updateAccessibilityFeaturesOnLoad = () => {
			if (_selectedAccessibilityFeatures.length > 0) {
				clearAllAccessibilityFeaturesSelections.classList.remove(CLASSES.hide);
			}
			for (let s = 0; s < _selectedAccessibilityFeatures.length; s++) {
				let selected = _selectedAccessibilityFeatures[s];
				let value = selected.innerHTML;
				let selectedInput = document.getElementById('checkbox-' + value) as HTMLInputElement;
				filters.accessibilityFeatures.push(value);
				toggleClearFilterBtn(filters.accessibilityFeatures, accessibilityInputs, clearAllAccessibilityFeaturesSelections);
				handleFilterTag(selectedInput, value, filters.accessibilityFeatures, 'accessibilityFeature');
			}
		}

		const checkFilterListToggle = (filterList: any, filterAccordion: HTMLElement, filterListToggle: HTMLElement, id: any) => {
			let initialInputs = [];
			setTimeout(() => {
				for (let i = 0; i < filterList?.length; i++) {
					let el = filterList[i] as HTMLElement;
					if (!el?.classList.contains(CLASSES.hide) && el.parentElement?.id === id) {
						initialInputs.push(el);
					}
				}
	
				if (initialInputs.length < 6 ) {
					if (!filterAccordion?.classList.contains(CLASSES.accordionActive) && !filterListToggle?.classList.contains(CLASSES.accordionTriggerActive)) {
						let btn = filterListToggle?.querySelector('.' + CLASSES.accordionTrigger) as HTMLButtonElement;
						btn?.click();
					}
				}
			}, 1000);
		}

		const updateUrl = () => {
			url = '?sort=' + filters.sort + '&postcode=' + filters.postcode + '&query=' + filters.what + '&radius=' + filters.radius + '&categories=' + filters.categories.join(';') + '&community_groups=' + filters.communityGroups.join(';') + '&accessibility_features=' + filters.accessibilityFeatures.join(';') + '&location_type=' + filters.locationType + '&view=' + filters.view + '&page=' + filters.pageNumber;
			window.history.pushState({}, "", url);
		}

		const checkSearchResultsView = (listView: HTMLElement, gridView: HTMLElement) => {
			listView?.addEventListener('click', () => {
				filters.view = 'Listview';
				updateUrl();
			});
			gridView?.addEventListener('click', () => {
				filters.view = 'Gridview';
				updateUrl();
			});
		}

		const checkInputField = (fieldset: HTMLElement, inputId: string, selectedId: string) => {
			let button = fieldset?.querySelector('.' + CLASSES.buttonInput) as HTMLButtonElement;
			let input = document.getElementById(inputId) as HTMLInputElement;
			let selectedPostcodeContainer = document.getElementById(ID.selectedPostcodeContainer) as HTMLElement;
			let selectedWhatContainer = document.getElementById(ID.selectedWhatContainer) as HTMLElement;
			let selected = document.getElementById(selectedId) as HTMLElement;
			let selectedPostcodeValue = document.getElementById(ID.selectedPostcodeValue) as HTMLElement;
			let selectedWhatValue = document.getElementById(ID.selectedWhatValue) as HTMLElement;
			let postcodeTag = document.createElement('div') as HTMLElement;
			let whatTag = document.createElement('div') as HTMLElement;

			if (selected != null && fieldset.id === ID.whereFilter) {
				selected.addEventListener('click', () => {
					selected.parentElement?.remove();
					filters.postcode = '';
					handlePostcodeSearch();
					refreshSearchResults();
					updateUrl();
				});
				filters.postcode = selectedPostcodeValue.innerHTML;
				handlePostcodeSearch();
			}
			if (selected != null && fieldset.id === ID.whatFilter) {
				selected.addEventListener('click', () => {
					selected.parentElement?.remove();
					filters.what = '';
					handleWhatSearch();
					refreshSearchResults();
					updateUrl();
				});
				filters.what = selectedWhatValue.innerHTML;
				handleWhatSearch();
			}

			input?.addEventListener("keypress", (e) => {
				if (where != null && what != null) {
					if (e.key === KEYS.enter) {
						e.preventDefault();
						button.click();
					}
				}

			});

			button?.addEventListener('click', (e) => {
				e.preventDefault();
				setTimeout(() => {
					console.log("called timeout function");
					if (input.value != '' && fieldset.id === ID.whereFilter) {
						if (selected != null) {
							selected.parentElement?.remove();
						}
						let postcodeTagHtml = filterTagHTML.postcode.replace(new RegExp(EDITS.postcode, 'g'), input.value);
						postcodeTag.classList.add(CLASSES.filterTag);
						postcodeTag.innerHTML = postcodeTagHtml
						selectedPostcodeContainer.append(postcodeTag);
						filters.postcode = input.value;
						handlePostcodeSearch();
					}
					if (input.value != '' && fieldset.id === ID.whatFilter) {
						if (selected != null) {
							selected.parentElement?.remove();
						}
						let whatTagHtml = filterTagHTML.what.replace(new RegExp(EDITS.what, 'g'), input.value);
						whatTag.classList.add(CLASSES.filterTag);
						whatTag.innerHTML = whatTagHtml;
						selectedWhatContainer.append(whatTag);
						filters.what = input.value;
						handleWhatSearch();
					}
					input.value = '';
					removeFilterTag(postcodeTag as HTMLElement, whatTag as HTMLElement);
					refreshSearchResults();
					updateUrl();
				}, 1000);
			});

		}

		const removeFilterTag = (postcodeTag: HTMLElement, whatTag: HTMLElement) => {
			if (postcodeTag != null) {
				postcodeTag.addEventListener('click', () => {
					postcodeTag.remove();
					filters.postcode = '';
					handlePostcodeSearch();
					refreshSearchResults();
					updateUrl();
				});
			}
			if (whatTag != null) {
				whatTag.addEventListener('click', () => {
					whatTag.remove();
					filters.what = '';
					handleWhatSearch();
					refreshSearchResults();
					updateUrl();
				});
			}
		}

		const checkForInputChange = (sortBy: HTMLElement, distance: HTMLElement, categories: HTMLElement, who: HTMLElement, accessibility: HTMLElement, coverage: HTMLElement) => {
			let fieldsets = [{ "id": 'sortBy', "fieldset": sortBy, "class": CLASSES.radioInput, "type": 'radio' }, { "id": 'distance', "fieldset": distance, "class": CLASSES.radioInput, "type": 'radio' }, { "id": 'categories', "fieldset": categories, "class": CLASSES.checkboxInput, "type": 'checkbox' }, { "id": 'community-groups', "fieldset": who, "class": CLASSES.checkboxInput, "type": 'checkbox' }, { "id": 'accessibility-features', "fieldset": accessibility, "class": CLASSES.checkboxInput, "type": 'checkbox' }, { "id": 'coverage', "fieldset": coverage, "class": CLASSES.radioInput, "type": 'radio' }];
			fieldsets.forEach(element => {
				let inputs = element.fieldset?.querySelectorAll('.' + element.class) as NodeList;
				checkRadioFilters(element.fieldset);
				for (let i = 0; i < inputs?.length; i++) {
					let el = inputs[i] as HTMLInputElement;
					el.addEventListener('change', (e) => {
						if (element.type === 'radio') {
							checkRadioFilters(element.fieldset);
						} else if (element.type === 'checkbox') {
							checkCheckboxFilters(element.fieldset);
							checkFilterSelections(element.id, inputs);
							refreshSearchResults();
						}
						updateUrl();
					});
				}
			});
		}

		const checkFilterSelections = (id: any, inputs: any) => {
			if (id === 'categories') {
				toggleClearFilterBtn(filters.categories, inputs, clearAllCategorySelections);
			} else if (id === 'community-groups') {
				toggleClearFilterBtn(filters.communityGroups, inputs, clearAllCommunityGroupsSelections);
			} else if (id === 'accessibility-features') {
				toggleClearFilterBtn(filters.accessibilityFeatures, inputs, clearAllAccessibilityFeaturesSelections);
			}
		};

		const toggleClearFilterBtn = (array: Array<string>, checkboxes: any, clearBtn: HTMLElement) => {
			if (array.length < 1) {
				clearBtn.classList.add(CLASSES.hide);
			} else {
				clearBtn.classList.remove(CLASSES.hide);
				clearBtn.addEventListener('click', (e) => {
					e.preventDefault;
					for (let i = 0; i < checkboxes.length; i++) {
						let el = checkboxes[i] as HTMLInputElement;
						if (el.checked) {
							el.click();
						}
					}
				});
			}
		}

		const checkRadioFilters = (filter: HTMLElement) => {
			let inputs = filter?.querySelectorAll('.' + CLASSES.radioInput) as any;

			switch (filter?.id) {
			case ID.servicesSortFilter:
				return getCheckedRadioValues(inputs, ID.servicesSortFilter);
			case ID.distanceFilter:
				return getDistanceValue(inputs, ID.distanceFilter);
			case ID.locNatFilter:
				return getCheckedRadioValues(inputs, ID.locNatFilter);
			}
		}

		const getCheckedRadioValues = (inputs: any, id: string) => {
			for (let i = 0; i < inputs?.length; i++) {
				let _radio = inputs[i] as HTMLInputElement;
				if (_radio.checked && id === ID.servicesSortFilter) {
					filters.sort = _radio.value;
				} else if (_radio.checked && id === ID.locNatFilter) {
					filters.locationType = _radio.value;
				}
			}
			refreshSearchResults();
		}

		const getDistanceValue = (inputs: any, id: string) => {
			let customInputLabel = document.getElementById(ID.sortCustomDistanceLabel)?.innerHTML;
			let customValue = customInputLabel?.substring(0, customInputLabel.length - 2);
			let numericValue = Number(customValue);
			let isCustomValue = false;
			let checkedValue = '';
			let customInput = document.getElementById(ID.sortCustomInput) as HTMLInputElement;

			for (let i = 0; i < inputs?.length; i++) {
				let _radio = inputs[i] as HTMLInputElement;
				if (_radio.type === 'radio') {
					if (_radio.checked) {
						if (_radio.id === ID.sortCustomInput) {
							isCustomValue = true;
							checkedValue = (numericValue * 1000).toString();
							customInput.value = checkedValue;
							filters.radius = checkedValue;
						} else {
							isCustomValue = false;
							checkedValue = _radio.value;
						}
					}

				} else if (_radio.type === 'range') {
					_radio.addEventListener('change', () => {
						customInput.click();
					});
				}
			}

			if (isCustomValue) {
				customDistanceSubmitButton?.classList.remove(CLASSES.hide);
				customDistanceSubmitButton?.addEventListener('click', (e) => {
					e.preventDefault();
					filters.radius = checkedValue;
					refreshSearchResults();
				});
			}
			else {
				customDistanceSubmitButton?.classList.add(CLASSES.hide);
				filters.radius = checkedValue;
				refreshSearchResults();
			}
		}

		const checkCheckboxFilters = (filter: HTMLElement) => {
			let inputs = filter?.querySelectorAll('.' + CLASSES.checkboxInput) as any;
			for (let i = 0; i < inputs.length; i++) {
				let _checkbox = inputs[i] as HTMLInputElement;
				if (filter.id === ID.categoriesFilter) {
					handleCheckboxSelections(_checkbox, filters.categories, 'category');
				}
				if (filter.id === ID.whoFilter) {
					handleCheckboxSelections(_checkbox, filters.communityGroups, 'communityGroup');
				}
				if (filter.id === ID.accessibilityFilter) {
					handleCheckboxSelections(_checkbox, filters.accessibilityFeatures, 'accessibilityFeature');
				}
			}
		}

		const handleCheckboxSelections = (checkbox: HTMLInputElement, selectionsArray: Array<string>, dataName: string) => {
			let pCheckbox = document.getElementById(dataName + "-" + checkbox.value) as HTMLElement;
			let childInputs = pCheckbox?.querySelectorAll('input') as NodeList;
			let childFieldset = pCheckbox?.querySelector('fieldset') as HTMLElement;

			if (checkbox.checked && !selectionsArray.includes(checkbox.value)) {
				selectionsArray.push(checkbox.value);
				if (childFieldset != null) {
					childFieldset.classList.remove(CLASSES.hide);
				}
				handleFilterTag(checkbox, checkbox.value, selectionsArray, dataName);
			} else if (!checkbox.checked && selectionsArray.includes(checkbox.value)) {
				let index = selectionsArray.indexOf(checkbox.value);
				if (childFieldset != null) {
					childFieldset.classList.add(CLASSES.hide);
				}
				if (index > -1) {
					let filterTag = document.getElementById('selected-' + dataName + '--' + checkbox.value)?.parentElement as HTMLElement;
					selectionsArray.splice(index, 1);
					for (let i = 0; i < childInputs.length; i++) {
						let input = childInputs[i] as HTMLInputElement;
						if (input.checked) {
							input.click();
						}
					}
					filterTag.remove();
				}
			}
		}

		const handleFilterTag = (checkbox: HTMLInputElement, checkboxValue: string, selectionsArray: Array<string>, dataName: string) => {
			if (selectionsArray.length > 0) {
				let filterTag = document.createElement('div') as HTMLElement;
				let inputLabel = document.getElementById(dataName + "-" + checkboxValue)?.querySelector('label')?.innerHTML as string;
				let inputLabelAggregationIndex = inputLabel.indexOf('(');
				let tagName = inputLabel.substring(0, inputLabelAggregationIndex - 0);
				let tagHtml = filterTagHTML.default.replace(new RegExp(EDITS.slug, 'g'), checkboxValue).replace(new RegExp(EDITS.name, 'g'), tagName).replace(new RegExp(EDITS.dataName, 'g'), dataName);
				filterTag.className = CLASSES.filterTag;
				filterTag.innerHTML = tagHtml;

				if (dataName === 'category') {
					categoryFilterTagContainer.appendChild(filterTag);
					selectedCategories.append(categoryFilterTagContainer);
				}
				if (dataName === 'communityGroup') {
					communityGroupsFilterTagContainer.append(filterTag);
					selectedCommunityGroups.append(communityGroupsFilterTagContainer);
				}
				if (dataName === 'accessibilityFeature') {
					accessibilityFeaturesFilterTagContainer.append(filterTag);
					selectedAccessibilityFeatures.append(accessibilityFeaturesFilterTagContainer);
				}

				let removeTag = document.getElementById('selected-' + dataName + '--' + checkboxValue) as HTMLElement;
				removeTag.addEventListener('click', (e) => {
					e.preventDefault();
					checkbox.click();
					filterTag.remove();
				});
			}
		}

		const GetCategoriesList = () => {
			let request = new XMLHttpRequest();
			request.open('GET', apiBaseUrl + '/categories', true);
			request.onload = function () {
				if (this.status >= 200 && this.status < 400) {
					// Success!
					let data = JSON.parse(this.response);
					allCategories = [];
					for (let cat = 0; cat < data.data.length; cat++) {
						let i = data.data[cat];
						if (i.sub_categories != null && i.sub_categories.length > 0) {
							allCategories.push({ "key": i.id, "slug": i.slug, "name": i.name, "type": "categories", "sub_categories": i.sub_categories });
						} else {
							allCategories.push({ "key": i.id, "slug": i.slug, "name": i.name, "type": "categories" });
						}
					}
				}
				else {
					// We reached our target server, but it returned an error
					console.log('We reached our target server, but it returned an error');
				}
			};
			request.onerror = function () {
				// There was a connection error of some sort
				console.log('error');
			};
			request.send();

		}

		const GetCommunityGroupList = () => {
			let request = new XMLHttpRequest();
			request.open('GET', apiBaseUrl + 'community-groups', true);
			request.onload = function () {
				if (this.status >= 200 && this.status < 400) {
					// Success!
					let data = JSON.parse(this.response);
					allCommunityGroups = [];
					for (let cat = 0; cat < data.data.length; cat++) {
						let i = data.data[cat];
						if (i.sub_communitygroups != null && i.sub_communitygroups.length > 0) {
							allCommunityGroups.push({ "key": i.id, "slug": i.slug, "name": i.name, "type": "communitygroups", "sub_communitygroups": i.sub_communitygroups });
						} else {
							allCommunityGroups.push({ "key": i.id, "slug": i.slug, "name": i.name, "type": "communitygroups", });
						}
					}
				}
				else {
					// We reached our target server, but it returned an error
					console.log('We reached our target server, but it returned an error');
				}
			};

			request.onerror = function () {
				// There was a connection error of some sort
				console.log('error');
			};
			request.send();
		}

		const GetAccessibilityList = () => {
			let request = new XMLHttpRequest();
			request.open('GET', apiBaseUrl + 'accessibility-features', true);
			request.onload = function () {
				if (this.status >= 200 && this.status < 400) {
					// Success!
					let data = JSON.parse(this.response);
					allAccessibilityFeatures = [];
					for (let cat = 0; cat < data.data.length; cat++) {
						let i = data.data[cat];
						allAccessibilityFeatures.push({ "key": i.id, "slug": i.slug, "name": i.name, });
					}
				}
				else {
					// We reached our target server, but it returned an error
					console.log('We reached our target server, but it returned an error');
				}
			};

			request.onerror = function () {
				// There was a connection error of some sort
				console.log('error');
			};
			request.send();
		}

		const refreshAggregations = () => {
			let request = new XMLHttpRequest();
			request.open('GET', apiBaseUrl + 'searchaggregations/?postcode=' + filters.postcode + '&q=' + filters.what + '&radius=' + filters.radius + '&categories=' + filters.categories.join(';') + '&community_groups=' + filters.communityGroups.join(';') + '&accessibility_features=' + filters.accessibilityFeatures.join(';') + '&location_type=' + filters.locationType, true)

			request.onload = function () {
				if (this.status >= 200 && this.status < 400) {
					// Success!
					let data = JSON.parse(this.response);
					allAggregations = [];
					allAggregations.acc_agg = data.aggregations.acc_agg.buckets;
					allAggregations.cat_agg = data.aggregations.cat_agg.buckets;
					allAggregations.who_agg = data.aggregations.who_agg.buckets;
					updateCategoryFilterAggregations(allAggregations.cat_agg, allCategories);
					updateWhoFilterAggregations(allAggregations.who_agg, allCommunityGroups);
					updateAccessibilityFilterAggregations(allAggregations.acc_agg, allAccessibilityFeatures);
				}
				else {
					// We reached our target server, but it returned an error
					console.log('We reached our target server, but it returned an error');
				}
			};

			request.onerror = function () {
				// There was a connection error of some sort
				console.log('error');
			};
			request.send();
		}

		const updateInputVisibility = (pKeyMatches: Array<any>, sKeyMatches: Array<any>, tKeyMatches: Array<any>, filterField:HTMLElement, dataname: string, datanames: string, datanamet: string) => {
			// if (pKeyMatches.length > 0) {
			let parentInputs = filterField.querySelectorAll(dataname) as NodeList;
			let sInputs = filterField.querySelectorAll(datanames) as NodeList;
			let tInputs = filterField.querySelectorAll(datanamet) as NodeList;
			let hasFilters = false;
				
			for (let i = 0; i < parentInputs?.length; i++) {
				let input = parentInputs[i].parentElement as HTMLElement;
				if (pKeyMatches.includes(input.id)) {
					input.classList.remove(CLASSES.hide);
					hasFilters = true;
				} else {
					input.classList.add(CLASSES.hide);
				}

				for (let i = 0; i < sInputs?.length; i++) {
					let input = sInputs[i].parentElement as HTMLElement;
					if (sKeyMatches.includes(input.id)) {
						input.classList.remove(CLASSES.hide);
					} else {
						input.classList.add(CLASSES.hide);
					}
				}

				for (let i = 0; i < tInputs?.length; i++) {
					let input = tInputs[i].parentElement as HTMLElement;
					if (tKeyMatches.includes(input.id)) {
						input.classList.remove(CLASSES.hide);
					} else {
						input.classList.add(CLASSES.hide);
					}
				}
			}

			if (hasFilters) {
				filterField.classList.remove(CLASSES.hide);
			}
			else {
				filterField.classList.add(CLASSES.hide);
			}
			// }
		}

		const updateCategoryFilterAggregations = (allAggregations: Array<any>, filtersArray: Array<any>) => {
			let pKeyMatches = [] as any;
			let sKeyMatches = [] as any;
			let tKeyMatches = [] as any;
			allAggregations.forEach((element: any) => {
				let aggKey = element.key;
				let aggCount = element.doc_count;

				for (let i = 0; i < filtersArray.length; i++) {
					let filter = filtersArray[i];
					let input = document.getElementById("category-" + filter.slug) as HTMLElement;
					let inputLabel = input?.querySelector('label') as HTMLLabelElement;

					if (aggKey === filter.key && inputLabel != null) {
						input?.classList.remove(CLASSES.hide);
						inputLabel.innerHTML = filter.name + ' ' + '(' + aggCount + ')';
						pKeyMatches.push("category-" + filter.slug);
					}

					if (filter.type === "categories" && filter.sub_categories != null && filter.sub_categories.length > 0) {
						for (let sI = 0; sI < filter.sub_categories.length; sI++) {
							let sub_categorys = filter.sub_categories[sI];
							let sInput = document.getElementById("category-" + sub_categorys.slug) as HTMLElement;
							let sInputLabel;
							if (sInput != null) {
								sInputLabel = sInput?.querySelector('label') as HTMLLabelElement;
							}
							if (aggKey === sub_categorys.id && sInputLabel != null) {
								sInputLabel.innerHTML = sub_categorys.name + ' ' + '(' + aggCount + ')';
								sInputLabel?.parentElement?.classList.remove(CLASSES.hide);
								sKeyMatches.push("category-" + sub_categorys.slug);
							}

							if (sub_categorys.sub_categories != null && sub_categorys.sub_categories.length > 0) {
								for (let ssI = 0; ssI < sub_categorys.sub_categories.length; ssI++) {
									let sub_categoryt = sub_categorys.sub_categories[ssI];
									let ssInput = document.getElementById("category-" + sub_categoryt.slug) as HTMLElement;
									let ssInputLabel;
									if (ssInput != null) {
										ssInputLabel = ssInput?.querySelector('label') as HTMLLabelElement;
									}
									if (aggKey === sub_categoryt.id && ssInputLabel != null) {
										ssInputLabel.innerHTML = sub_categoryt.name + ' ' + '(' + aggCount + ')';
										ssInputLabel?.parentElement?.classList.remove(CLASSES.hide);
										tKeyMatches.push("category-" + sub_categoryt.slug);
									}
								}
							}
						}
					}
				}
			});

			updateInputVisibility(pKeyMatches, sKeyMatches, tKeyMatches, categories, "[data-name='category']", "[data-name='categorys']", "[data-name='categoryt']");
			checkFilterListToggle(categoryInitialInputs, fullPrimaryCategoryList, filterListToggleCategory, 'categories-display');
		}

		const updateWhoFilterAggregations = (allAggregations: Array<any>, filtersArray: Array<any>) => {
			let pKeyMatches = [] as any;
			let sKeyMatches = [] as any;
			let tKeyMatches = [] as any;
			allAggregations.forEach((element: any) => {
				let aggKey = element.key;
				let aggCount = element.doc_count;

				for (let i = 0; i < filtersArray.length; i++) {
					let filter = filtersArray[i];
					let input = document.getElementById("communityGroup-" + filter.slug) as HTMLElement;
					let inputLabel = input?.querySelector('label') as HTMLLabelElement;

					if (aggKey === filter.key && inputLabel != null) {
						input?.classList.remove(CLASSES.hide);
						inputLabel.innerHTML = filter.name + ' ' + '(' + aggCount + ')';
						pKeyMatches.push("communityGroup-" + filter.slug);
					}

					if (filter.type === "communitygroups" && filter.sub_communitygroups != null && filter.sub_communitygroups.length > 0) {
						for (let sI = 0; sI < filter.sub_communitygroups.length; sI++) {
							let sub_communitygroups = filter.sub_communitygroups[sI];
							let sInput = document.getElementById("communityGroup-" + sub_communitygroups.slug) as HTMLElement;
							let sInputLabel;
							if (sInput != null) {
								sInputLabel = sInput?.querySelector('label') as HTMLLabelElement;
							}
							if (aggKey === sub_communitygroups.id && sInputLabel != null) {
								sInputLabel.innerHTML = sub_communitygroups.name + ' ' + '(' + aggCount + ')';
								sInputLabel?.parentElement?.classList.remove(CLASSES.hide);
								sKeyMatches.push("communityGroup-" + sub_communitygroups.slug);
							}

							if (sub_communitygroups.sub_categories != null && sub_communitygroups.sub_categories.length > 0) {
								for (let ssI = 0; ssI < sub_communitygroups.sub_categories.length; ssI++) {
									let sub_communitygroupt = sub_communitygroups.sub_categories[ssI];
									let ssInput = document.getElementById("communityGroup" + sub_communitygroupt.slug) as HTMLElement;
									let ssInputLabel;
									if (ssInput != null) {
										ssInputLabel = ssInput?.querySelector('label') as HTMLLabelElement;
									}
									if (aggKey === sub_communitygroupt.id && ssInputLabel != null) {
										ssInputLabel.innerHTML = sub_communitygroupt.name + ' ' + '(' + aggCount + ')';
										ssInputLabel?.parentElement?.classList.remove(CLASSES.hide);
										tKeyMatches.push("communityGroup-" + sub_communitygroupt.slug);
									}
								}
							}
						}
					}
				}
			});

			updateInputVisibility(pKeyMatches, sKeyMatches, tKeyMatches, who, "[data-name='communityGroup']", "[data-name='communityGroups']", "[data-name='communityGroupt']");
			checkFilterListToggle(whoInitialInputs, fullPrimaryCommunityGroupList, filterListToggleWho, 'community-groups-display');
		}

		const updateAccessibilityFilterAggregations = (allAggregations: Array<any>, filtersArray: Array<any>) => {
			let pKeyMatches = [] as any;
			allAggregations.forEach((element: any) => {
				let aggKey = element.key;
				let aggCount = element.doc_count;

				for (let i = 0; i < filtersArray.length; i++) {
					let filter = filtersArray[i];
					let input = document.getElementById("accessibilityFeature-" + filter.slug) as HTMLElement;
					let inputLabel = input?.querySelector('label') as HTMLLabelElement;

					if (aggKey === filter.key && inputLabel != null) {
						input?.classList.remove(CLASSES.hide);
						inputLabel.innerHTML = filter.name + ' ' + '(' + aggCount + ')';
						pKeyMatches.push("accessibilityFeature-" + filter.slug);
					}
				}
			});

			let parentInputs = accessibility.querySelectorAll("[data-name='accessibilityFeature']") as NodeList;
			let hasFilters = false;
			for (let i = 0; i < parentInputs?.length; i++) {
				let input = parentInputs[i].parentElement as HTMLElement;
				if (pKeyMatches.includes(input.id)) {
					input.classList.remove(CLASSES.hide);
					hasFilters = true;
				} else {
					input.classList.add(CLASSES.hide);
				}
			}

			if (hasFilters) {
				accessibility.classList.remove(CLASSES.hide);
			}
			else {
				accessibility.classList.add(CLASSES.hide);
			}
			
			checkFilterListToggle(accessibilityInitialInputs, fullPrimaryAccessibilityFeatureList, filterListToggleAccessibility, 'accessibility-features-display');
		}

		const handlePostcodeSearch = () => {
			let sortDistanceInput = document.getElementById(ID.sortDistanceInput) as HTMLInputElement;
			let sortDistanceLabel = document.getElementById(ID.sortDistanceLabel) as HTMLLabelElement;
			let sortLastReviewedInput = document.getElementById(ID.sortLastReviewedInput) as HTMLInputElement;
			let sortRelevanceInput = document.getElementById(ID.sortRelevanceInput) as HTMLInputElement;
			let distanceInputs = distance?.querySelectorAll('input') as NodeList;
			let distanceLabels = distance?.querySelectorAll('label') as NodeList;

			if (filters.postcode === '') {
				if (headerText != null) {
					headerText.innerHTML = '<h1>Services and Resources</h1>';
				}

				if(filters.what === ''){
					sortLastReviewedInput?.click();
				}
				else{
					sortRelevanceInput?.click();
				}
				
				sortDistanceInput?.classList.add(CLASSES.inputDisabled);
				if (sortDistanceInput != null) {
					sortDistanceInput.disabled = true;
				}
				sortDistanceLabel?.classList.add(CLASSES.radioLabelDisabled);
				sortDistanceLabel.innerHTML = 'Distance (nearest to furthest) - location needed';

				for (let i = 0; i < distanceLabels?.length; i++) {
					let label = distanceLabels[i] as HTMLLabelElement;
					label.classList.add(CLASSES.radioLabelDisabled);
				}
				for (let i = 0; i < distanceInputs?.length; i++) {
					let input = distanceInputs[i] as HTMLInputElement;
					input.classList.add(CLASSES.inputDisabled);
					if (input != null && input.id === ID.defaultRadius) {
						input.click();
						input.disabled = true;
					}
				}
			} else {
				if (headerText != null) {
					headerText.innerHTML = '<h1>Services and Resources for' + ' ' + '(' + filters.postcode + ')' + '</h1>';
				}

				if (sortDistanceInput.classList.contains(CLASSES.inputDisabled) && sortDistanceLabel.classList.contains(CLASSES.radioLabelDisabled)) {
					sortDistanceInput?.classList.remove(CLASSES.inputDisabled);
					sortDistanceInput.disabled = false;
					sortDistanceLabel?.classList.remove(CLASSES.radioLabelDisabled);
					sortDistanceLabel.innerHTML = 'Distance (nearest to furthest)';
				}

				for (let i = 0; i < distanceLabels?.length; i++) {
					let label = distanceLabels[i] as HTMLLabelElement;
					label.classList.remove(CLASSES.radioLabelDisabled);
				}
				for (let i = 0; i < distanceInputs?.length; i++) {
					let input = distanceInputs[i] as HTMLInputElement;
					input.classList.remove(CLASSES.inputDisabled);
					input.disabled = false;
					// if (input.id === ID.defaultRadius) {
					// 	input.click();
					// }
				}
			}
		}

		const handleWhatSearch = () => {
			let sortDistanceInput = document.getElementById(ID.sortDistanceInput) as HTMLInputElement;
			let sortLastReviewedInput = document.getElementById(ID.sortLastReviewedInput) as HTMLInputElement;
			let sortRelevanceInput = document.getElementById(ID.sortRelevanceInput) as HTMLInputElement;
			let sortRelevanceLabel =  document.getElementById(ID.sortRelevanceLabel) as HTMLLabelElement;
			if (filters.what === '') {
				if(filters.postcode === ''){
					sortLastReviewedInput?.click();
				}
				else{
					sortDistanceInput?.click();
				}
				
				sortRelevanceInput?.classList.add(CLASSES.inputDisabled);
				if (sortRelevanceInput != null) {
					sortRelevanceInput.disabled = true;
				}
				sortRelevanceLabel?.classList.add(CLASSES.radioLabelDisabled);
				sortRelevanceLabel.innerHTML = 'Relevance - keyword needed';
			} else {
				if (sortRelevanceInput.classList.contains(CLASSES.inputDisabled) && sortRelevanceLabel.classList.contains(CLASSES.radioLabelDisabled)) {
					sortRelevanceInput?.classList.remove(CLASSES.inputDisabled);
					sortRelevanceInput.disabled = false;
					sortRelevanceLabel?.classList.remove(CLASSES.radioLabelDisabled);
					sortRelevanceLabel.innerHTML = 'Relevance';
				}
			}
		}

		const refreshSearchResults = () => {
			let resultsContainer = document.getElementById(ID.searchResultsContainer) as HTMLElement;
			if (resultsContainer != null) {
				resultsContainer.innerHTML = '';
			}
			loader?.classList.remove(CLASSES.hide);
			if (!searchPageNavigation?.classList.contains(CLASSES.hide)) {
				searchPageNavigation?.classList.add(CLASSES.hide);
			}

			let request = new XMLHttpRequest();
			request.open('GET', apiBaseUrl + 'services/?sort=' + filters.sort + '&postcode=' + filters.postcode + '&q=' + filters.what + '&radius=' + filters.radius + '&categories=' + filters.categories.join(';') + '&community_groups=' + filters.communityGroups.join(';') + '&accessibility_features=' + filters.accessibilityFeatures.join(';') + '&location_type=' + filters.locationType + '&view=' + filters.view + '&page_size=' + filters.pageSize + '&page=' + filters.pageNumber, true)

			request.onload = function () {
				if (this.status >= 200 && this.status < 400) {
					// Success!
					let data = JSON.parse(this.response);
					let resultsTotal = document.getElementById(ID.resultsTotal) as HTMLElement;
					let resultsPageTotal = document.getElementById(ID.resultsPageTotal) as HTMLElement;

					pageTotal = Math.ceil(data.count / filters.pageSize);
					setTimeout(() => {
						loader?.classList.add(CLASSES.hide);
						searchPageNavigation?.classList.remove(CLASSES.hide);

						if (resultsContainer != null && resultsTotal != null && resultsPageTotal != null) {
							resultsContainer.innerHTML = '';
							resultsTotal.innerHTML = data.count + ' Results';
							resultsPageTotal.innerHTML = ' of' + ' ' + pageTotal;
						}

						if (filters.pageNumber > pageTotal) {
							filters.pageNumber = pageTotal;
							updateUrl();
							refreshSearchResults();
						}

						if (pageNumber > pageTotal && pageNavigationInput != null) {
							pageNavigationInput.placeholder = pageTotal.toString();
							filters.pageNumber = pageTotal.toString();
							pageNumber = pageTotal;
						} else {
							pageNavigationInput.placeholder = pageNumber.toString();
						}

						if (pageTotal > 1) {
							if (pageNumber === pageTotal) {
								nextLink.classList.add(CLASSES.hide);
							} else {
								nextLink.classList.remove(CLASSES.hide);
								nextLink.addEventListener('click', (e) => {
									filters.pageNumber = (pageNumber + 1).toString();
									updateUrl();
									nextLink.href = url;
								});
							}
							if (pageNumber === 1) {
								previousLink.classList.add(CLASSES.hide);
							} else if (pageNumber > 1) {
								previousLink.classList.remove(CLASSES.hide);
								previousLink.addEventListener('click', (e) => {
									filters.pageNumber = (pageNumber - 1).toString();
									updateUrl();
									previousLink.href = url;
								});
							}
						} else if (pageTotal <= 1) {
							previousLink.classList.add(CLASSES.hide);
							nextLink.classList.add(CLASSES.hide);
						}

						for (let p = 0; p < data.data.length; p++) {
							let i = data.data[p];
							let itemContainer = document.createElement('div') as HTMLElement;
							itemContainer.className = CLASSES.searchItemContainer;
							filters.view === 'Gridview' ? itemContainer.classList.add(CLASSES.GridWidth) : itemContainer.classList.remove(CLASSES.GridWidth);

							let itemHeader = document.createElement('div') as HTMLElement;
							itemHeader.className = CLASSES.searchItemHeader;
							let serviceName = html.title.replace(new RegExp(EDITS.id, 'g'), i.id).replace(new RegExp(EDITS.name, 'g'), i.name).replace(new RegExp(EDITS.slug, 'g'), i.slug);
							let deliveredByContainer = document.createElement('p');
							let deliveredBy = html.DeliveredBy.replace(new RegExp(EDITS.orgSlug, 'g'), i.organisation.slug).replace(new RegExp(EDITS.orgName, 'g'), i.organisation.name);
							let reviewedDateContainer = document.createElement('div') as HTMLElement;
							let reviewDateValue = i.last_reviewed;

							reviewDateValue = new Date(reviewDateValue).toLocaleDateString('en-uk');
							let reviewedDate = html.reviewDate.replace(new RegExp(EDITS.reviewDate, 'g'), reviewDateValue);
							
							let summaryContainer = document.createElement('p');
							let summary;
							let shortSummary;

							if(i.summary) {
								summary = html.summary.replace(new RegExp(EDITS.summary, 'g'), i.summary);
							}
							else {
								if (i.description.length > 210) {
									summary = html.summary.replace(new RegExp(EDITS.summary, 'g'), i.description.substring(0, 200) + "...");
								}
								else {
									summary = html.summary.replace(new RegExp(EDITS.summary, 'g'), i.description);
								}
							}

							let linksContainer = document.createElement('div') as HTMLElement;
							linksContainer.className = CLASSES.searchItemLinksContainer;
							let links = document.createElement('ul') as HTMLElement;
							links.className = CLASSES.SearchItemLinksIconList;

							let webLink = html.webLink.replace(new RegExp(EDITS.weblink, 'g'), i.url);
							let phone = html.phone.replace(new RegExp(EDITS.phone, 'g'), i.phone);
							let email = html.email.replace(new RegExp(EDITS.email, 'g'), i.email);
							let referral = html.referral.replace(new RegExp(EDITS.referral, 'g'), i.referral_url);

							if (i.is_deprioritised) {
								itemContainer.classList.add(CLASSES.deprioritisedServiceItem);
							}

							resultsContainer?.appendChild(itemContainer);
							itemContainer?.appendChild(itemHeader);
							itemHeader.innerHTML = serviceName;

							if (i.is_claimed || i.organisation.is_claimed) {
								parseAndAppend(html.serviceClaimed, itemHeader);
							}

							itemHeader.appendChild(deliveredByContainer);
							deliveredByContainer.innerHTML = deliveredBy;

							if (i.organisation.is_claimed) {
								parseAndAppend(html.orgClaimed, deliveredByContainer);
							}
							itemHeader.appendChild(reviewedDateContainer);
							reviewedDateContainer.innerHTML = reviewedDate;
							itemHeader.appendChild(summaryContainer);
							summaryContainer.innerHTML = summary;

							if (i.url != null || i.phone != null || i.email != null || i.referral_url != null) {
								itemContainer.appendChild(linksContainer);
								linksContainer.appendChild(links);
								if (i.url != null && i.url.length > 0) {
									parseAndAppend(webLink, links);
								}
								if (i.phone != null && i.phone.length > 0) {
									parseAndAppend(phone, links);
								}
								if (i.email != null && i.email.length > 0) {
									parseAndAppend(email, links);
								}
								if (i.referral_url != null && i.referral_url.length > 0) {
									parseAndAppend(referral, links);
								}
							}
						}

						let deprioritiedMessageContainer = document.createElement('div') as HTMLElement;
						let deprioritiedMessageElement = document.createElement('h2') as HTMLElement;
						let deprioritisedServiceElements = document.querySelectorAll('.' + CLASSES.deprioritisedServiceItem);
						deprioritiedMessageContainer.classList.add(CLASSES.deprioritisedServicesMessage);
						deprioritiedMessageContainer.classList.add(CLASSES.marginBottom15);
						deprioritiedMessageElement.textContent = "The following services have not been reviewed in the past 12 months.";
						deprioritiedMessageContainer.append(deprioritiedMessageElement);
						deprioritisedServiceElements.forEach((el, idx) => {
							if (idx === 0) {
								el.before(deprioritiedMessageContainer);
							}
						});
					}, 1000);
				}
				else {
					// We reached our target server, but it returned an error
					console.log('We reached our target server, but it returned an error');
				}
			};

			request.onerror = function () {
				// There was a connection error of some sort
				console.log('error');
			};
			request.send();
			GetCategoriesList();
			GetCommunityGroupList();
			GetAccessibilityList();
			refreshAggregations();
		}

		const parseAndAppend = (string: string, element: HTMLElement) => {
			let newEl = new DOMParser().parseFromString(string, "text/html");
			element.appendChild(newEl.body.firstChild as HTMLElement);
		}

		init();
	}
}

export default GetFilteredResults;

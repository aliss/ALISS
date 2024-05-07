import Selectr from '../../../js/thirdparty/select';

var fontData = require("../forms/icons.json");

class IconSelector {
	constructor() {

		enum CLASSES {
			icon = '.aliss-form__icon'
		}

		const init = () => {
			const iconSelector = document.querySelector(CLASSES.icon) as HTMLSelectElement;
			if (typeof(iconSelector) != 'undefined' && iconSelector != null) {
				const form = document.querySelector('form.aliss-form') as HTMLFormElement;
				var renderer = (data: any) => {
					var template = ['<div class="color-option"><i aria-hidden="true" class="' + data.value + '"></i>' + data.text + '</div>'];
					return template.join('');
				}
				const iconSelectedItem = iconSelector.dataset.iconSelected as string;
				const iconSelectorId = "#" + iconSelector.id;
				let data = {				
					data: fontData,
					renderOption: renderer,
					renderSelection: renderer,
					defaultSelected: false,
					selectedValue: iconSelectedItem
				};
				const selector = new Selectr(iconSelectorId, data);
				selector.on('selectr.init', function() {
					let select = form.elements[5] as HTMLSelectElement;
					select.value = iconSelectedItem;
				});
			}
		}

		init();
	}
}

export default IconSelector;
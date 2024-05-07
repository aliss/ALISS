/** @format */

class Forms {
	constructor() {
		enum EVENTS {
			Click = 'click',
		}

		enum TYPES {
			Radio = '[type="radio"]',
		}

		enum CLASSES {
			RadioButtonLabelClass = 'aliss-component-search-filter__radio__item__label',
			RadioButtonInputClass = 'aliss-component-search-filter__radio__item__input',
			RadioButtonActive = 'js-radio-checked',
		}

		let matches = function(el: any, selector: any) {
			return (
				el.matches ||
				el.matchesSelector ||
				el.msMatchesSelector ||
				el.mozMatchesSelector ||
				el.webkitMatchesSelector ||
				el.oMatchesSelector
			).call(el, selector);
		};

		const form_radio_buttons = () => {
			document.body.addEventListener(EVENTS.Click, (e) => {
				const targets = e.target as Element;
				const radioButtons = document.querySelectorAll(TYPES.Radio) as NodeListOf<HTMLFormElement>;

				if (matches(targets, '.' + CLASSES.RadioButtonInputClass)) {
					for (var i = 0; i < radioButtons.length; i++) {
						if (radioButtons[i].checked == true) {
							radioButtons[i].classList.toggle(CLASSES.RadioButtonActive);
						}
					}
				}

				if (matches(targets, '.' + CLASSES.RadioButtonLabelClass)) {
					const parent = e.target as Element;
					const elements = parent.parentNode!.querySelectorAll('.' + CLASSES.RadioButtonInputClass);
					Array.prototype.forEach.call(elements, function(el, i) {
						el.click();
					});
				}
			});
		};

		const init = () => {
			form_radio_buttons();
		};

		!!document.querySelector('.' + CLASSES.RadioButtonLabelClass) && init();
	}
}

export default Forms;

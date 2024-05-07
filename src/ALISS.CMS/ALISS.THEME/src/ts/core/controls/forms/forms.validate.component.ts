
import Aliss from '../../';

class Validate {
	constructor() {

		enum CLASSES {
			error = "aliss-form__group--error",
            textarea = ".aliss-form__textarea",
            input = ".aliss-form__input",
            group = "aliss-form__group"
		}

		const init = () => {
			const inputs = document.querySelectorAll(CLASSES.input);
			const textareas = document.querySelectorAll(CLASSES.textarea);
			if (typeof(inputs) != 'undefined' && inputs != null) {
				inputs.forEach(e => {
					const input = e as HTMLInputElement;
					input.addEventListener(Aliss.Enums.Events.Change, () => {
						if(input.value != "") {
							let groupHolder = Aliss.Helpers.closest(input, "." + CLASSES.error);
							if(groupHolder != null) {
								Aliss.Helpers.removeClass(groupHolder, CLASSES.error);
							}
						}
					});
				});
			}
			if (typeof(textareas) != 'undefined' && textareas != null) {
				textareas.forEach(e => {
					const textarea = e as HTMLTextAreaElement;
					textarea.addEventListener(Aliss.Enums.Events.Change, () => {
						if(textarea.value != "") {
							Aliss.Helpers.removeClass(textarea.parentNode, CLASSES.error);
						}
					});
				});
			}
		}

		init();
	}
}

export default Validate;
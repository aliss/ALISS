
import Aliss from '../../';

class Validate {
	constructor() {

		enum CLASSES {
			error = "aliss-form__group--error",
            textarea = ".aliss-form__textarea",
			rte = ".ql-editor",
            input = ".aliss-form__input",
            group = "aliss-form__group"
		}

		const init = () => {
			const inputs = document.querySelectorAll(CLASSES.input);
			const textareas = document.querySelectorAll(CLASSES.textarea);
			const rtes = document.querySelectorAll(CLASSES.rte);
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
			if (typeof(rtes) != 'undefined' && rtes != null) {
				rtes.forEach(e => {
					const rte = e as HTMLDivElement;
					const outerDiv = document.getElementById("editorDiv") as HTMLDivElement;
					rte.addEventListener(Aliss.Enums.Events.Keyup, () => {
						Aliss.Helpers.removeClass(outerDiv.parentNode, CLASSES.error);
						console.log("changes");
					});
				});
			}
		}

		init();
	}
}

export default Validate;
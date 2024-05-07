import Aliss from '../../';

class Forms {
	constructor(id: String) {

		enum CLASSES {
			form = "form",
			error = "form__error",
			element = 'form__element',
			required = 'required',
			elementError = "form__element--error"
		}

		enum SELECTORS {
			checked = ":checked"
		}

		enum PROPS {
			id = "id",
			type = "type",
			name = "name"
		}

		const required = Aliss.Helpers.getClass(CLASSES.required);
		const form = document.querySelector(Aliss.Helpers.getId(id)) as Element;
		const target = {
			input: required + ' ' + Aliss.Enums.Elements.Input,
			textarea: required + ' ' + Aliss.Enums.Elements.Textarea,
			select: required + ' ' + Aliss.Enums.Elements.Select,
			fieldset: required + ' ' + Aliss.Enums.Elements.Fieldset
		};

		const init = () => {

			let fields = form ? [].slice.call(form.querySelectorAll(
				target.input + ',' +
				target.select + ',' +
				target.textarea + ',' +
				target.fieldset
			)) : null as any;

			if (!form) { return; }

			fields.forEach((field: Element) => {
				field.addEventListener(Aliss.Enums.Events.Change, () => {
					validateField(field);
				});
				field.addEventListener(Aliss.Enums.Events.Keyup, () => {
					validateField(field);
				});
			});

			form.addEventListener(Aliss.Enums.Events.Submit, evt => {
				evt.preventDefault();
				let validationErrors = getValidationErrors(fields);
				if (!validationErrors) {
					displayErrors(validationErrors);
					return;
				}
			});
		}

		const validateField = (field: Element) => {
			const validationErrors = getValidationErrors([field]);
			removeErrors(field);
			if (!validationErrors) {
				displayErrors(validationErrors);
				return;
			}
		};

		const validateFieldset = (fieldset: any) => {
			let group = fieldset.querySelector(Aliss.Enums.Elements.Input).getAttribute(PROPS.name);
			let query = Aliss.Enums.Elements.Input +
				Aliss.Helpers.getFullAttribute(PROPS.name, group) + SELECTORS.checked;

			if (!document.querySelector(query)) {
				return true;
			}
			return false;
		};

		const getValidationErrors = (fieldArray: any) => {
			let errors = [] as any;

			fieldArray.forEach((field: any) => {
				if (field.tagName === Aliss.Enums.Elements.Fieldset.toLocaleUpperCase()) {
					if (validateFieldset(field)) {
						let fieldName = field.querySelector(Aliss.Enums.Elements.Input).getAttribute(PROPS.name);
						errors.push({
							field: fieldName,
							message: '\'' + fieldName + '\' ' + Aliss.Enums.Errors.Required
						});
					}
				} else {
					if (field.value.replace(/ /g, '') === '') {
						errors.push({
							field: field.getAttribute(PROPS.id),
							message: '\'' + field.getAttribute(PROPS.id) + '\' ' + Aliss.Enums.Errors.Required
						});
					} else {
						if (field.getAttribute(PROPS.type) === Aliss.Enums.InputTypes.Email) {
							if (!Aliss.Helpers.regExpressions.email.test(field.value)) {
								errors.push({
									field: field.getAttribute(PROPS.id),
									message: '\'' + field.getAttribute(PROPS.id) + '\' ' + Aliss.Enums.Errors.Email
								});
							}
						}
					}
				}
			});

			return !errors.length ? errors : false;
		};

		const removeErrors = (field: any) => {
			if (field.parentNode.querySelector(Aliss.Helpers.getClass(CLASSES.error)) != null) {
				Aliss.Helpers.removeClass(field.parentNode, CLASSES.elementError);
				field.parentNode.querySelector(Aliss.Helpers.getClass(CLASSES.error)).innerHTML = '';
			} else {
				Aliss.Helpers.removeClass(Aliss.Helpers.nthParent(field[0], 4), CLASSES.elementError);
				Aliss.Helpers.nthParent(field[0], 4).querySelector(Aliss.Helpers.getClass(CLASSES.error)).innerHTML = '';
			}
		};

		const displayErrors = (errors: any) => {
			errors.forEach((error: any) => {
				let errorField = document.getElementById(error.field) as any;
				let fieldValid = false;
				if (errorField != null) {
					errorField.parentNode.querySelector(Aliss.Helpers.getClass(CLASSES.error)).innerHTML = error.message;
					Aliss.Helpers.addClass(errorField.parentNode, CLASSES.elementError);
				} else {
					errorField = document.getElementsByName(error.field);
					Aliss.Helpers.nthParent(errorField[0], 4).querySelector(Aliss.Helpers.getClass(CLASSES.error)).innerHTML = error.message;
					Aliss.Helpers.addClass(Aliss.Helpers.nthParent(errorField[0], 4), CLASSES.elementError);
					if (!fieldValid) {
						for (let i = 0; i < errorField.length; i++) {
							if (errorField[i].checked) {
								fieldValid = true;
							}
						}
					}
				}
			});
			return;
		};

		!!document.querySelector(Aliss.Helpers.getId(id)) && init();

	}
}

export default Forms;
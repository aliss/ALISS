import Aliss from '../../';
import Options from './site-accordion-nested-options';

class NestedAccordion {
	constructor() {

		enum ATTRIBUTES {
			trigger = 'data-nested-accordion-trigger',
			content = 'data-nested-accordion-content'
		}
		enum ARIA {
			hidden = "aria-hidden"
		}
		enum CLASSES {
			content = 'aliss-nested-accordion__content',
			activeContent = 'aliss-nested-accordion__content--active',
			activeTrigger = 'aliss-nested-accordion__trigger--active'
		}
		const init = () => {
			listener(ATTRIBUTES.trigger);
		};

		const listener = (toggle: string) => {
			document.body.addEventListener(Aliss.Enums.Events.Click, (event: any) => {
				let nestedAccordionTarget = event.target;
				if (nestedAccordionTarget.hasAttribute(toggle)) {
					event.preventDefault();
					toggleAccordion(nestedAccordionTarget);
				}
			});
		};

		const getAccordion = (element: HTMLElement) => {
			let nestedAccordion = new Options();
			let selector = element.dataset.accordionTrigger as string;
			nestedAccordion.trigger = element;
			nestedAccordion.content = document.getElementById(selector);
			return nestedAccordion;
		};

		const addClasses = (header: Element, content: Element) => {
			Aliss.Helpers.addClass(header, CLASSES.activeTrigger);
			Aliss.Helpers.addClass(content, CLASSES.activeContent);
			content.removeAttribute(ARIA.hidden);
		};

		const removeClasses = (header: Element, content: Element) => {
			Aliss.Helpers.removeClass(header, CLASSES.activeTrigger);
			Aliss.Helpers.removeClass(content, CLASSES.activeContent);
			content.setAttribute(ARIA.hidden, "true");
		};

		const isContentActive = (content: Element) => {
			return Aliss.Helpers.hasClass(content, CLASSES.activeContent);
		};

		const isTriggerActive = (trigger: Element) => {
			return Aliss.Helpers.hasClass(trigger, CLASSES.activeTrigger);
		};

		const toggleAccordion = (element: HTMLElement) => {
			let nestedAccordion = getAccordion(element);
			if (!isContentActive(nestedAccordion.content) && isTriggerActive(nestedAccordion.trigger)) {
				addClasses(nestedAccordion.trigger, nestedAccordion.content);
			} else if (isContentActive(nestedAccordion.content) && isTriggerActive(nestedAccordion.trigger)) {
				removeClasses(nestedAccordion.trigger, nestedAccordion.content);
			} else {
				addClasses(nestedAccordion.trigger, nestedAccordion.content);
			}
		};

		init();
	}
}

export default NestedAccordion;

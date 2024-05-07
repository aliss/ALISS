import Aliss from '../../';
import Options from './site-accordion-options';

class Accordion {
	constructor() {

		enum ATTRIBUTES {
			trigger = 'data-accordion-trigger',
			content = 'data-accordion-content'
		}
		enum ARIA {
			hidden = "aria-hidden"
		}
		enum CLASSES {
			content = 'aliss-accordion__content',
			activeContent = 'aliss-accordion__content--active',
			activeTrigger = 'aliss-accordion__trigger--active'
		}
		const init = () => {
			listener(ATTRIBUTES.trigger);
		};

		const listener = (toggle: string) => {
			document.body.addEventListener(Aliss.Enums.Events.Click, (event: any) => {
				let accordionTarget = event.target;
				if (accordionTarget.hasAttribute(toggle)) {
					event.preventDefault();
					toggleAccordion(accordionTarget);
				}
			});
		};

		const getAccordion = (element: HTMLElement) => {
			let accordion = new Options();
			let selector = element.dataset.accordionTrigger as string;
			accordion.trigger = element;
			accordion.content = document.getElementById(selector);
			return accordion;
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
			let accordion = getAccordion(element);
			if (!isContentActive(accordion.content) && isTriggerActive(accordion.trigger)) {
				addClasses(accordion.trigger, accordion.content);
			} else if (isContentActive(accordion.content) && isTriggerActive(accordion.trigger)) {
				removeClasses(accordion.trigger, accordion.content);
			} else {
				addClasses(accordion.trigger, accordion.content);
			}
		};

		init();
	}
}

export default Accordion;

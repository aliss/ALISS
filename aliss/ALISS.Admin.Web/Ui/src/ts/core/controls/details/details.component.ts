import Aliss from '../..';
import Options from './details.options';

class Details {
	constructor() {

		enum ATTRIBUTES {
			trigger = 'data-details-trigger',
			content = 'data-details-content'
		}
		enum ARIA {
			hidden = "aria-hidden"
		}
		enum CLASSES {
			content = 'aliss-details__content',
			activeContent = 'aliss-details__content--active',
			activeTrigger = 'aliss-details__trigger--active'
		}
		const init = () => {
			listener(ATTRIBUTES.trigger);
		};

		const listener = (toggle: string) => {
			document.body.addEventListener(Aliss.Enums.Events.Click, (event: any) => {
				let detailsTarget = event.target;
				if (detailsTarget.hasAttribute(toggle)) {
					event.preventDefault();
					toggleDetails(detailsTarget);
				}
			});
		};

		const getDetails = (element: HTMLElement) => {
			let details = new Options();
			console.log(details);
			let selector = element.dataset.detailsTrigger as string;
			details.trigger = element;
			details.content = document.getElementById(selector);
			
			return details;
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

		const toggleDetails = (element: HTMLElement) => {
			let details = getDetails(element);
			if (!isContentActive(details.content) && isTriggerActive(details.trigger)) {
				addClasses(details.trigger, details.content);
			} else if (isContentActive(details.content) && isTriggerActive(details.trigger)) {
				removeClasses(details.trigger, details.content);
			} else {
				addClasses(details.trigger, details.content);
			}
		};

		init();
	}
}

export default Details;

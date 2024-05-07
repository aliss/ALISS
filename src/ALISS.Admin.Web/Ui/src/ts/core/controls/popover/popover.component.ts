import Aliss from '../../';

class Popover {
	constructor() {

		enum ATTRIBUTES {
			popover = "data-popover"
		}

		enum CLASSES {
			popover = "popover"
		}

		enum IDENTIFY {
			popover = "positioner-popover"
		}

		const generateOptions = (element: any) => {
			let opt = new Aliss.Controls.Positioner.Options();
			opt.className = CLASSES.popover;
			opt.triggerName = ATTRIBUTES.popover;
			opt.offset = 8;
			opt.id = IDENTIFY.popover;
			opt.position = Aliss.Enums.Positions.Bottom;
			opt.content = element.innerHTML || null;
			return opt;
		}

		const init = () => {
			document.body.addEventListener(Aliss.Enums.Events.Click, (e: any) => {
				exists();
				show(e);
			});
		}

		const exists = () => {
			let positioner = document.getElementById(IDENTIFY.popover) as HTMLElement;
			if (positioner) {
				Aliss.Helpers.removeElementById(IDENTIFY.popover);
			}
		}

		const show = (e: any) => {
			let el = e.target;
			if (el.hasAttribute(ATTRIBUTES.popover)) {
				e.preventDefault();
				let
					popover = el.getAttribute(ATTRIBUTES.popover),
					popoverEl = document.getElementById(popover) as HTMLElement,
					opts = generateOptions(popoverEl);
				new Aliss.Controls.Positioner.Service(el, opts);
			}
		}

		

		!!document.querySelector(Aliss.Helpers.getAttribute(ATTRIBUTES.popover)) && init();

	}
}

export default Popover;
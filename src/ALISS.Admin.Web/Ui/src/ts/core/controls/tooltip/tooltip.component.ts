import Aliss from '../../';

class Tooltip {
	constructor() {

		enum ATTRIBUTES {
			tooltip = "data-tip"
		}

		enum CLASSES {
			tooltip = "tooltip"
		}

		enum IDENTIFY {
			tooltip = "positioner-tooltip"
		}

		const generateOptions = (content: string) => {
			let opt = new Aliss.Controls.Positioner.Options();
			opt.className = CLASSES.tooltip;
			opt.triggerName = ATTRIBUTES.tooltip;
			opt.offset = 8;
			opt.id = IDENTIFY.tooltip;
			opt.position = Aliss.Enums.Positions.Top;
			opt.content = content;
			return opt;
		}

		const init = () => {
			document.body.addEventListener(Aliss.Enums.Events.Mouseover, (e: any) => {
				show(e);
			});
		}

		const exists = () => {
			let positioner = document.getElementById(IDENTIFY.tooltip) as HTMLElement;
			if (positioner) {
				Aliss.Helpers.removeElementById(IDENTIFY.tooltip);
			}
		}

		const content = (el: any) => {
			return "<span>" + el.getAttribute(ATTRIBUTES.tooltip) + "</span>";
		}

		const show = (e: any) => {
			let el = e.target;
			if (el.hasAttribute(ATTRIBUTES.tooltip)) {
				e.preventDefault();
				let opts = generateOptions(content(el));
				new Aliss.Controls.Positioner.Service(el, opts);
			} else {
				exists();
			}
		}

		!!document.querySelector(Aliss.Helpers.getAttribute(ATTRIBUTES.tooltip)) && init();
	}
}

export default Tooltip;
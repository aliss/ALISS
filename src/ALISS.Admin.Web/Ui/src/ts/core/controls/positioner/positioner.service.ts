import Template from './positioner.template';
import Options from './positioner.options';
import Aliss from '../../';

class Positioner {
	constructor(trigger: HTMLElement, options: Options) {

		enum REPLACE {
			id = "{{id}}",
			class = "{{class}}",
			content = "{{content}}"
		}

		const init = () => {
			replaceHtml();
			calculate();
			recalculate();
		}

		const replaceHtml = () => {
			let html = Template;
			html = html.replace(REPLACE.id, options.id);
			html = html.replace(REPLACE.class, options.className);
			html = html.replace(REPLACE.content, options.content);
			return document.body.insertAdjacentHTML(Aliss.Enums.Events.Beforeend, html);
		}

		const calculate = () => {
			let offset = getPosition();
			let positioner = document.getElementById(options.id) as HTMLElement;
			positioner.style.top = offset.top as string;
			positioner.style.left = offset.left as string;
			return false;
		}

		const setTriggerParams = () => {
			let triggerEl = trigger.getBoundingClientRect();
			
			let params = {
				offset: options.offset,
				width: triggerEl.width,
				height: triggerEl.height,
				top: triggerEl.top,
				left: triggerEl.left,
				right: triggerEl.right,
				bottom: triggerEl.bottom
			};
			return params;
		}

		const getPosition = () => {

			let positioner = document.getElementById(options.id) as HTMLElement;
			let params = positioner.getBoundingClientRect();
			let trigger = setTriggerParams();

			let offset = {
				top: '0px',
				left: '0px',
				width: params.width + 'px',
				height: params.height + 'px'
			};
			switch (options.position) {
			case Aliss.Enums.Positions.Top:
				offset.left = windowDetectOffsetX(offset.left, params, trigger) + 'px';
				offset.top = windowDetectOffsetY(offset.top, params, trigger) + 'px';
				break;
			case Aliss.Enums.Positions.Bottom:
				offset.left = windowDetectOffsetX(offset.left, params, trigger) + 'px';
				offset.top = windowDetectOffsetY(offset.top, params, trigger) + 'px';
				break;
			default:
				offset.left = windowDetectOffsetX(offset.left, params, trigger) + 'px';
				offset.top = windowDetectOffsetY(offset.top, params, trigger) + 'px';
			}
			return offset;
		}

		const windowDetectOffsetX = ($offsetX: any, params: any, trigger: any) => {
			let center = trigger.left + (trigger.width / 2 - params.width / 2);
			let right = trigger.left - (params.width - trigger.width);
			let left = trigger.left;
			let off = ((center + params.width) < window.innerWidth);
			$offsetX = (center > 0 && off) ? center : (off ? left : right);
			return $offsetX;
		}

		const windowDetectOffsetY = ($offsetY: any, params: any, trigger: any) => {
			let scroll = 'pageYOffset' in window ? window.pageYOffset : document.body.scrollTop;
			let bottom = trigger.top + (trigger.height + trigger.offset);
			let top = trigger.top - (params.height + trigger.offset);
			let off = bottom + (params.height + trigger.offset);
			let $offsetB = off > window.innerHeight ? top + scroll : bottom + scroll;
			let $offsetT = off < 0 ? bottom + scroll : top + scroll;
			$offsetY = options.position == Aliss.Enums.Positions.Top ? $offsetT : $offsetB;
			return $offsetY;
		}

		const recalculate = () => {
			window.addEventListener(Aliss.Enums.Events.Resize, () => {
				calculate();
			});
		}

		init();
	}
}

export default Positioner;
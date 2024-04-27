import Aliss from '../../';

class Options {
	public position: string;
	public triggerName: string;
	public className: string;
	public content: string;
	public toggle: string;
	public id: string;
	public offset: number;
	public closeOnClick: boolean;
	public showToggleState: boolean;

	constructor(
		position: string = Aliss.Enums.Positions.Top,
		triggerName: string = '',
		className: string = '',
		content: string = '',
		toggle: string = '',
		id: string = 'positioner',
		offset: number = 0,
		closeOnClick: boolean = true,
		showToggleState: boolean = false) {
		this.position = position;
		this.triggerName = triggerName;
		this.className = className;
		this.content = content;
		this.toggle = toggle;
		this.id = id;
		this.offset = offset;
		this.closeOnClick = closeOnClick;
		this.showToggleState = showToggleState;
	}
}

export default Options;
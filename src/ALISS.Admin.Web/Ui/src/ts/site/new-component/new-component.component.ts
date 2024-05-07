import Aliss from '../../core/';

class MyNewComponent {
	constructor() {

		enum ATTRIBUTES {
			target = 'data-component',
		}

		enum CLASSES {
			target = 'component',
			active = 'component--active'
		}

		const init = () => {
			console.log("Works");
		};

		init();

	}
}

export default MyNewComponent;

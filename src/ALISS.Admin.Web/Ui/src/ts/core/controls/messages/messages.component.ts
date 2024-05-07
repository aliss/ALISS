
import Aliss from '../../index';

class Messages {
	constructor() {

		enum ID {
			error = 'error',
			errorTrigger = 'error-close',
			success = 'success',
			successTrigger = 'success-close',
			warning = 'warning',
			warningTrigger = 'warning-close'
		}

		const successTimeout = () => {
			let successBox = document.getElementById(ID.success) as HTMLElement;
			if (typeof(successBox) != 'undefined' && successBox != null) {
				setTimeout(()=>{
					Aliss.Helpers.removeElementById(ID.success);
				}, 6000);
			}
		}

		const clickToClose = (holder : string, trigger: string) => {
			let box = document.getElementById(holder) as HTMLElement;
			let close = document.getElementById(trigger) as HTMLElement;
			if (typeof(box) != 'undefined' && box != null) {
				close.addEventListener("click", () => {
					Aliss.Helpers.removeElementById(holder);
				});
			}
		}

		const init = () => {
			successTimeout();
			clickToClose(ID.error, ID.errorTrigger);
			clickToClose(ID.success, ID.successTrigger);
			clickToClose(ID.warning, ID.warningTrigger);
		}

		init();

	}
}

export default Messages;

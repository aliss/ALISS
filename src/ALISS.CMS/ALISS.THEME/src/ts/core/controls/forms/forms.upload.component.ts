class Upload {
	constructor() {

		enum CLASSES {
			upload = '.aliss-form__file-btn'
		}

		const init = () => {
			const buttons = document.querySelectorAll(CLASSES.upload);
			for (const button of buttons) {
				const actualButton = button.nextElementSibling as HTMLInputElement;
				const fileChosen = button.nextElementSibling?.nextElementSibling as HTMLDivElement;
				button.addEventListener("click", function(e){
					e.preventDefault();
					actualButton.click();
				});
				actualButton.addEventListener('change', function(){
					const file = this.files instanceof FileList ? this.files[0].name : "No file selected";
					fileChosen.textContent = file;
				});
			}
		}
		init();
	}
}

export default Upload;
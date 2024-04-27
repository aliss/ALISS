/** @format */
class Banner {
	constructor() {	
		enum IDS  {
			reviewedServiceDiv = "banner-reviewed-service",
            reviewedServiceButton = "banner-reviewed-service-button",
		}

		const init = () => {
            var dismissButton = document.getElementById(IDS.reviewedServiceButton) as HTMLButtonElement;
            if(dismissButton){
                dismissButton.addEventListener("click", () => {
                    var reviewedService = document.getElementById(IDS.reviewedServiceDiv) as HTMLDivElement;
                    reviewedService.classList.add("hide");
                })
            }
		}
	
		init();
	}
}

export default Banner;

/** @format */

class DeprioritisedDataDisclaimerCookie {
	constructor() {	

		enum IDS  {
			disclaimer = "aliss-deprioritised-data-disclaimer",
            dismissBtn = "aliss-deprioritised-data-disclaimer-dismiss",
		}

		enum CLASSES {
			hideBanner = "hide",
			active = "aliss-deprioritised-data-disclaimer--active"
		}

		const banner = document.getElementById(IDS.disclaimer);
		const dismissBtn = document.getElementById(IDS.dismissBtn);


		const init = () => {
			const setDisclaimerCookie = (cname: string, cvalue: string, exdays: number) => {
				let date = new Date();
				date.setTime(date.getTime() + (exdays*24*60*60*1000));
				let expires = "expires="+ date.toUTCString();
				document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
			}
	
			const getCookie = (cname: any) => {
				let name = cname + "=";
				let decodedCookie = decodeURIComponent(document.cookie);
				let ca = decodedCookie.split(';');
				for(let i = 0; i < ca.length; i++) {
				  let c = ca[i];
				  while (c.charAt(0) == ' ') {
						c = c.substring(1);
				  }
				  if (c.indexOf(name) == 0) {
						return c.substring(name.length, c.length);
				  }
				}
				return "";
			  }
	
			  const checkCookie = () => {
				let user = getCookie("deprioritisedDataDisclaimer");
				if (user != "") {
					banner?.classList.add(CLASSES.hideBanner);
				} else {
					banner?.classList.remove(CLASSES.hideBanner);
					banner?.classList.add(CLASSES.active);
					user = "Hide disclaimer";
					dismissBtn?.addEventListener('click', (e) => {
						e.preventDefault();
						banner?.classList.add(CLASSES.hideBanner);
						banner?.classList.remove(CLASSES.active);
						setDisclaimerCookie("deprioritisedDataDisclaimer", user, 180);
					});
				}
			  }
			
			  checkCookie();
			
		}
        
		init();
	}
}

export default DeprioritisedDataDisclaimerCookie;
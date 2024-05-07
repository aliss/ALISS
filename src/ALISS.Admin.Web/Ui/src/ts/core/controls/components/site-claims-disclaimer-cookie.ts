/** @format */

class ClaimsDisclaimerCookie {
	constructor() {	

		enum IDS  {
			disclaimer = "aliss-claims-disclaimer",
            dismissBtn = "aliss-claims-disclaimer-dismiss"
		}

		enum CLASSES {
			hideBanner = "hide"
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
				console.log("checking for cookie");
				let user = getCookie("claimsDisclaimerStatus");
				if (user != "") {
					banner?.classList.add(CLASSES.hideBanner);
				} else {
					banner?.classList.remove(CLASSES.hideBanner);
					user = "Hide disclaimer";
					dismissBtn?.addEventListener('click', (e) => {
						banner?.classList.add(CLASSES.hideBanner);
						setDisclaimerCookie("claimsDisclaimerStatus", user, 180);
					});
				}
			  }
			
			  checkCookie();
			
		}
        
		init();
	}
}

export default ClaimsDisclaimerCookie;
/** @format */

import CookiesEuBanner from '../../../core/libraries/cookie';

class Cookie {
	constructor() {	

		enum IDS  {
			euState = "cookies-eu-state",
			euEdit = "cookies-eu-edit",
			euSettings = "cookie-eu-settings",
			euShowEdit = "cookie-eu-show-edit",
			euCloseEdit = "cookie-eu-close-edit",
			euCustomAccept = "cookie-eu-custom-accept",
			euCustomDecline = "cookie-eu-custom-reject",
			euStatus = "cookie-eu-status"
		}

		enum CLASSES {
			container = "aliss-cookie__editor--active",
			button = "aliss-cookie__settings--active"
		}

		// The live key is G-YRB8K6J28L the beta key is UA-106504389-1
		let trackScript = `			
			window.dataLayer = window.dataLayer || [];
			function gtag(){window.dataLayer.push(arguments);}
			gtag('js', new Date());				
			gtag('config', 'G-YRB8K6J28L');
			console.log("Tracking Enabled");`;

		let gaScriptUrl;
		let gaScript;

		const generateGoogleTag = async () => {
			gaScriptUrl = document.createElement("script");
			gaScriptUrl.async = true;
			gaScriptUrl.src = "https://www.googletagmanager.com/gtag/js?id=G-YRB8K6J28L";			
			gaScript = document.createElement("script");
			gaScript.setAttribute("nonce", "gtm");
			gaScript.innerHTML = trackScript;
			document.head.appendChild(gaScriptUrl);
			document.head.appendChild(gaScript);
		}

		const init = () => {

			var cookiesBanner = new CookiesEuBanner(() => {
				generateGoogleTag();
			}, true) as any;

			function updateCookiesEuState() {
				var hasConsent = cookiesBanner.hasConsent();
				var editor = document.getElementById(IDS.euEdit) as HTMLDivElement;
				var status = document.getElementById(IDS.euStatus) as HTMLSpanElement;
				if (hasConsent === true) {
					editor.removeAttribute("style");
					status.innerHTML = "Analytic cookies accepted"
				} else if (hasConsent === false) {
					editor.removeAttribute("style");
					status.innerHTML = "Analytic cookies declined"
				} else {
					editor.style.display = "none";
				}
			}

			updateCookiesEuState();
			setInterval(updateCookiesEuState, 3000);
			
			let cookieEditButton = document.getElementById(IDS.euShowEdit) as HTMLButtonElement;
			let cookieEditCloseButton = document.getElementById(IDS.euCloseEdit) as HTMLButtonElement;
			let cookieEditAcceptButton = document.getElementById(IDS.euCustomAccept) as HTMLButtonElement;
			let cookieEditRejectButton = document.getElementById(IDS.euCustomDecline) as HTMLButtonElement;
			let cookieSettings = document.getElementById(IDS.euSettings) as HTMLDivElement;

			cookiesBanner.addClickListener(cookieEditButton, () => {
				cookieSettings.classList.add(CLASSES.container);
				cookieEditButton.classList.add(CLASSES.button);
			});
			cookiesBanner.addClickListener(cookieEditCloseButton, () => {
				cookieSettings.classList.remove(CLASSES.container);
				cookieEditButton.classList.remove(CLASSES.button);
			});
			cookiesBanner.addClickListener(cookieEditAcceptButton, () => {
				cookiesBanner.setConsent(true);
				cookiesBanner.removeBanner();
				cookieSettings.classList.remove(CLASSES.container);
				cookieEditButton.classList.remove(CLASSES.button);
				updateCookiesEuState();
			});
			cookiesBanner.addClickListener(cookieEditRejectButton, () => {
				cookiesBanner.setConsent(false);
				cookiesBanner.removeBanner();
				cookieSettings.classList.remove(CLASSES.container);
				cookieEditButton.classList.remove(CLASSES.button);
				updateCookiesEuState();
			});
		}
	
		//init();
	}
}

export default Cookie;

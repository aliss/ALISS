/** @format */

class LogInOut {
	constructor() {		
		
		enum ID {
            mobileLogin = "mobileLogin",
            mobileLogout = "mobileLogout",
            desktopLogin = "desktopLogin",
            desktopLogout = "desktopLogout"
		}
        
        enum CLASSES {
            hide = "hide",
            logout = ".aliss-logout"
        }

        const init = () => {
        	var logout = document.querySelectorAll(CLASSES.logout) as NodeList;
        	for(var i = 0, len = logout.length; i < len; i++) {
        		logout[i].addEventListener('click', (e) => {
        			e.preventDefault();
        			console.log("Logging Out");
        			var hostDomain = window.location.host;
        			var cookieDomain = hostDomain.substring(hostDomain.indexOf("."), hostDomain.length);
        			if(cookieDomain.indexOf(".") == -1) {
        				cookieDomain = hostDomain;
        			}
        			document.cookie = "ALISSAdmin.User=;domain="+ cookieDomain + ";expires=Thu, 01 Jan 1970 00:00:01 GMT;";
        			console.log(cookieDomain);
        			var mobileLogin = document.getElementById(ID.mobileLogin) as HTMLElement;
        			var mobileLogout = document.getElementById(ID.mobileLogout) as HTMLElement;
        			var desktopLogin = document.getElementById(ID.desktopLogin) as HTMLElement;
        			var desktopLogout = document.getElementById(ID.desktopLogout) as HTMLElement;
        			mobileLogin.classList.remove("hide")
        			mobileLogout.classList.add("hide")
        			desktopLogin.classList.remove("hide")
        			desktopLogout.classList.add("hide");
        			setTimeout(() => {
        				window.location.href = "/";
        			}, 500)
        		});
        	}
        }
	
        init();
	}
}

export default LogInOut

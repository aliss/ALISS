import Aliss from "../../";
class PdfViewer {
	constructor() {
    enum ID {
      terms = "terms-pdf-box",
      serviceterms = "service-terms-pdf-box",
      privacy = "privacy-pdf-box",
      conditions = "TermsAndConditions",
    }

    enum TARGETS {
      terms = "#terms",
      serviceterms = "#serviceterms",
      privacy = "#privacy",
    }

    enum CLASSES {
      activeState = "active",
    }

    const init = () => {
    	let termsBox = document.getElementById(ID.terms) as HTMLElement;
    	let servicetermsBox = document.getElementById(ID.serviceterms) as HTMLElement;
    	let privacyBox = document.getElementById(ID.privacy) as HTMLElement;
    	let conditions = document.getElementById(ID.conditions) as any;

    	let checkterms = typeof termsBox != "undefined" && termsBox != null;
    	let checkserviceterms = typeof servicetermsBox != "undefined" && servicetermsBox != null;
    	let checkprivacy = typeof privacyBox != "undefined" && privacyBox != null;
    	let checkconditions = typeof conditions != "undefined" && conditions != null;

    	if (checkconditions) {
    		conditions.disabled = true;
    	}

    	document.body.addEventListener("click", (e) => {
    		let targets = e.target;
    		if (checkterms) {
    			if (Aliss.Helpers.closest(targets, TARGETS.terms)) {
    				Aliss.Helpers.toggleClass(termsBox, CLASSES.activeState);
    				if(checkprivacy) {
    					Aliss.Helpers.removeClass(privacyBox, CLASSES.activeState);
    				}
    				if (checkconditions) {
    					conditions.disabled = false;
    				}
    			}
    		}

    		if (checkserviceterms) {
    			if (Aliss.Helpers.closest(targets, TARGETS.serviceterms)) {
    				Aliss.Helpers.toggleClass(servicetermsBox, CLASSES.activeState);
    				if (checkprivacy) {
    					Aliss.Helpers.removeClass(privacyBox, CLASSES.activeState);
    				}
    				if (checkconditions) {
    					conditions.disabled = false;
    				}
    			}
    		}

    		if (checkprivacy) {
    			if (Aliss.Helpers.closest(targets, TARGETS.privacy)) {
    				Aliss.Helpers.toggleClass(privacyBox, CLASSES.activeState);
    				if(checkterms) {
    					Aliss.Helpers.removeClass(termsBox, CLASSES.activeState);
    				}
    				if (checkconditions) {
    					conditions.disabled = false;
    				}
    			}
    		}
    	});
    };

    init();
	}
}

export default PdfViewer;

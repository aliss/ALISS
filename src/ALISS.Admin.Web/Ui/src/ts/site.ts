/**
 * ALISS UX Framework
 * @namespace
 * @author Paul Stewart - Front End & UI 
 * @author Kenny Steen - Back End
 * @author Stefano Labia - Back End
 */

/**
 * Import Polyfill
 */
import 'core-js/stable'; 

/**
 * Declare Aliss
 */
import Aliss from './core';

/**
 * Declare On DOM Content Loaded
 */
let onDomLoaded = () => {
 
	Aliss.Helpers.setLazy();
	Aliss.Helpers.lazyLoad();
 
	new Aliss.Controls.Accordion;
	new Aliss.Controls.Categories;
	new Aliss.Controls.Popover;
	new Aliss.Controls.Tooltip;
	new Aliss.Controls.IconSelector;
	new Aliss.Controls.Upload;
	new Aliss.Controls.Messages;
	new Aliss.Controls.OrganisationRepresentive;
	new Aliss.Controls.DataInputOrganisation;
	new Aliss.Controls.DataInputWhere;
	new Aliss.Controls.ServiceRepresentive;
	new Aliss.Controls.Locations;
	new Aliss.Controls.PdfViewer;
	new Aliss.Controls.Validate;
	new Aliss.Controls.OrganisationRepresentative;
	new Aliss.Controls.ServiceRepresentative;
	new Aliss.Controls.Slug;
	new Aliss.Controls.ServiceSlug;
	new Aliss.Controls.OrganisationSlug;
	new Aliss.Controls.Selections;
	new Aliss.Controls.CheckedSelections;
	new Aliss.Controls.ServiceAreas;
	new Aliss.Controls.CommunityGroups;
	new Aliss.Controls.AccessibilityFeatures;
	new Aliss.Controls.Email;
	new Aliss.Controls.ClaimsDisclaimerCookie();
	new Aliss.Controls.Map();
	new Aliss.Controls.MenuToggle();
	new Aliss.Controls.Banner();

	onReload();
};
 
/**
 * Declare onReload function
 * All functions inside are used for any reload events to reinitialise;
 */
let onReload = () => {
};
 
/**
 * Declare On Window.Load
 */
let onWinLoad = () => {
};
 
/**
 * Declare On Window.Load
 */
let onWinScroll = () => {
	Aliss.Helpers.lazyLoad();
};
 
/**
 * Declare On Window.Load
 */
let onWinResize = () => { };

/**
 * Execute DOMContent & OnWindowLoad
 */
Aliss.Helpers.isInternetExplorer();
window.addEventListener('DOMContentLoaded', onDomLoaded);
window.addEventListener('load', onWinLoad);
window.addEventListener('scroll', onWinScroll);
window.addEventListener('resize', onWinResize);
window.addEventListener('change', onReload);

/**
 * Export Default
 */
export default {};

/**
 * Disable buttons with type submit after first click
 */
var submitButtons = document.querySelectorAll("button[type=submit]");
var curButton;
addEventListener('submit', e=> {
	for(var i = 0; i < submitButtons.length; i++)
	{
		curButton = submitButtons[i] as HTMLButtonElement;
		curButton.disabled = true;
	}
});

/**
 * Disable tab key in RTE editors & make it move to the next field for accessability
 */
var RTE = document.getElementById("editorDiv") as HTMLDivElement;
const inputs = Array.prototype.slice.call(document.querySelectorAll('.aliss-form__input'));
if(RTE){
	document.addEventListener('keydown', event => {
		if(event.currentTarget?.activeElement.className == "ql-editor" && event.key == "Tab"){
			const currInput = document.activeElement?.parentElement?.parentElement?.nextSibling;
			const currInputIndex = inputs.indexOf(currInput);
			var input = inputs[currInputIndex];
			event.preventDefault();
			if(event.shiftKey){
				var inputIndex = currInputIndex;
				while(inputIndex > 0) {
					const previnputIndex =
					(inputIndex - 1) % inputs.length;
					input = inputs[previnputIndex];
					--inputIndex;
					if(!input.parentElement.parentElement.classList.contains("hide") && !input.parentElement.parentElement.parentElement.classList.contains("hide")){
						break;
					}
					else if(inputIndex == 0){
						input = inputs[currInputIndex];
						break;
					}
				} 
				input.focus();
			}
			else {
				const nextinputIndex =
					(currInputIndex + 1) % inputs.length;
				const input = inputs[nextinputIndex];
				input.focus();
			}
			var description = document.getElementsByClassName("ql-editor")[0] as HTMLDivElement;
			description.innerHTML = description.innerHTML.replace(/\t+/g, "");
			return false;
		}
		return true;
	});
}

/**
 * Disable links with specific IDs after first click
 */
var ids = ["publishLink"];
var curLink: HTMLLinkElement;
for (var i = 0; i < ids.length; i++) {
	var selector = document.querySelectorAll(`a#${ids[i]}`)[0];
	if (selector != null && selector != undefined) {
		curLink = selector as HTMLLinkElement;
		curLink.addEventListener('click', e => {
			if (curLink.classList.contains('clicked')) {
				e.stopImmediatePropagation();
				e.preventDefault();
			} else {
				curLink.classList.add('clicked');
			}
		})
	}	
}

/**
 * Give warning when leaving review pages
 * This currently cant be done as asked for in the AC - Discuss further with ALISS
 */
var serviceReview = document.getElementById('serviceReviewNavigation') as HTMLDivElement
if(serviceReview){
	window.onbeforeunload = function(){
		if(document.activeElement?.text != 'Edit Information' && document.activeElement?.value != 'Return To Review' && document.activeElement?.value != 'Review Complete'){
			return 'Are you sure you want to leave?  If you leave this page without checking the information and selecting the Review Complete button, then we will not be able to update your last reviewed date.  Any information dated older than 12 months is deprioritised on ALISS to ensure data quality for our searchers';
		}
		else{
			return null;
		}
	};
}

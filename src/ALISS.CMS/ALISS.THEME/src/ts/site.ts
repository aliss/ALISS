
/**
 * ALISS UX Framework - 2022
 * @namespace
 * @author Paul Stewart - Front End & UI 
 * @author Kenny Steen - Back End
 * @author Stefano Labia - Back End
 * @author Jordan Smith - Originial Contribution to Front End
 */

/**
 * Import Polyfill
 */
import 'core-js/stable'; 
import Aliss from './core';

const onDomLoaded = () => {
	
	Aliss.Helpers.setLazy();
	Aliss.Helpers.lazyLoad();

	new Aliss.Controls.Menu();
	new Aliss.Controls.Menufooter();
	new Aliss.Controls.Accordion();
	new Aliss.Controls.Map();
	new Aliss.Controls.Form();
	new Aliss.Controls.GetCategoryData();
	new Aliss.Controls.GetCommunityGroupData();
	new Aliss.Controls.GetAccessibilityFeatureData();
	new Aliss.Controls.Paginate();
	new Aliss.Controls.GetRangeValue();
	new Aliss.Controls.ImageGallery();
	new Aliss.Controls.Banners();
	new Aliss.Controls.Validate();
	new Aliss.Controls.SearchType();
	new Aliss.Controls.PageNavigation();
	new Aliss.Controls.SearchOptions();
	new Aliss.Controls.Cookie();
	new Aliss.Controls.Postcode();
	new Aliss.Controls.SearchFilters();
	new Aliss.Controls.SearchResultLayoutOptions();
	new Aliss.Controls.GetFilteredResults();
	new Aliss.Controls.LeaveSite();
	new Aliss.Controls.ClaimsDisclaimerCookie();
	new Aliss.Controls.DeprioritisedDataDisclaimerCookie();
	new Aliss.Controls.HomepageCarousel();

	screen.orientation.onchange = function () {
		if (screen.width < 768) {
			console.log(screen.orientation.type.match(/\w+/)![0], screen.width);
		}
	};
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
let onWinLoad = () => {};
 
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
window.addEventListener('load', onWinLoad);
window.addEventListener('scroll', onWinScroll);
window.addEventListener('resize', onWinResize);
window.addEventListener('change', onReload);
window.addEventListener('DOMContentLoaded', onDomLoaded);
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
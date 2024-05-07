/** @format */

class MenuToggle {
	constructor() {

		enum ID {
			menuToggleOpen = "menu-toggle-open",
            menuToggleClose = "menu-toggle-close",
            menu = "aliss-admin-menu",
			mainContent = "aliss-main-content",
			overlay = "aliss-overlay",
		}

		enum CLASSES {
			MenuActive = "aliss-menu--active",
			HideMain = "aliss-main--hidden",
			OverlayActive = "aliss-overlay--active"
		}

		let menuToggleOpen = document.getElementById(ID.menuToggleOpen);
		let menuToggleClose = document.getElementById(ID.menuToggleClose);
		let menu = document.getElementById(ID.menu);
		let mainContent = document.getElementById(ID.mainContent);
		let overlay = document.getElementById(ID.overlay);

		const toggleMenu = (toggleOpen:any, toggleClose:any) => {
			toggleOpen.addEventListener('click', (e:any) => {
				menu?.classList.add(CLASSES.MenuActive);
				mainContent?.classList.add(CLASSES.HideMain);
				overlay?.classList.add(CLASSES.OverlayActive);
			});

			toggleClose.addEventListener('click', (e:any) => {
				menu?.classList.remove(CLASSES.MenuActive);
				mainContent?.classList.remove(CLASSES.HideMain);
				overlay?.classList.remove(CLASSES.OverlayActive);
			});
		}

		const init = () => {
			toggleMenu(menuToggleOpen, menuToggleClose);
		}

		init();
	}
}

export default MenuToggle;
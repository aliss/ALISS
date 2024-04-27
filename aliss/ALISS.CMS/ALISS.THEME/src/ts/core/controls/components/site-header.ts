/** @format */

class MobileMenu {
	constructor() {
		enum CLASSES {
			Menu = 'navigation-toggle',
			MenuToggleActive = 'js-mobile-menu-active',
			MenuOpen = 'aliss-header-component-tertiary-menu--open',
			ReachdeckBtn = 'reachdeck-btn'
		}

		enum ID {
			MenuContainer = 'menu-container',
			MobileMenu = 'menu',
			MobileMenuToggle = 'menu-toggle',
			mobileAccessibilityToolsBtn = 'mobile-accessibility-tools-btn'
		}

		let mobileAccessibilityToolsBtn = document.getElementById(ID.mobileAccessibilityToolsBtn);

		let ReachdeckBtn = document.getElementsByClassName(CLASSES.ReachdeckBtn)[0] as HTMLElement;

		const init = () => {
			let matches = function(el, selector) {
				return (
					el.matches ||
					el.matchesSelector ||
					el.msMatchesSelector ||
					el.mozMatchesSelector ||
					el.webkitMatchesSelector ||
					el.oMatchesSelector
				).call(el, selector);
			};

			document.body.addEventListener('click', (e) => {
				let targets = e.target;
				let targetParent = e.target.parentNode;
				if (matches(targets, '.' + CLASSES.Menu) || matches(targetParent, '.' + CLASSES.Menu)) {
					let target = e.target.parentNode;
					document.getElementById(ID.MobileMenu)?.classList.toggle(CLASSES.MenuToggleActive);
					document.getElementById(ID.MobileMenuToggle)?.classList.toggle(CLASSES.MenuToggleActive);
					document.getElementById(ID.MenuContainer)?.classList.toggle(CLASSES.MenuOpen);
				}
			});

			mobileAccessibilityToolsBtn?.addEventListener('click', () => {
				let btn = ReachdeckBtn.firstChild as HTMLElement;
				btn.click();
			});
		};

		!!document.querySelector('.' + CLASSES.Menu) && init();
	}
}

export default MobileMenu;

/** @format */

class MobileMenuFooter {
	constructor() {
		enum CLASSES {
			Menu = 'navigation-footer-toggle',
			MenuToggleActive = 'js-mobile-menu-footer-active',
		}

		enum ID {
			MobileMenu = 'menu-footer',
			MobileMenuToggle = 'menu-footer-toggle',
		}

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
					document.getElementById(ID.MobileMenu).classList.toggle(CLASSES.MenuToggleActive);
					document.getElementById(ID.MobileMenuToggle).classList.toggle(CLASSES.MenuToggleActive);
				}
			});
		};

		!!document.querySelector('.' + CLASSES.Menu) && init();
	}
}

export default MobileMenuFooter;

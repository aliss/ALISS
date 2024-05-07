/** @format */

class Paginate {
	constructor() {
		enum CLASSES {
			Pagination = 'aliss-pagination-component',
			Pagination_Wrapper = 'aliss-pagination-component__wrapper',
			Pagination_Item = 'aliss-pagination-component__wrapper__item'
		}

		enum ID {
			Pagination_Active_Page = 'aliss-pagination-active-page'
		}

		const init = () => {

			let getActive = document.getElementById(ID.Pagination_Active_Page);

		};

		!!document.querySelector('.' + CLASSES.Pagination) && init();
	}
}

export default Paginate;

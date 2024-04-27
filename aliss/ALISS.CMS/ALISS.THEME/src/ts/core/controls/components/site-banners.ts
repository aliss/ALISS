/** @format */

class GetRangeValue {
	constructor() {
		enum CLASSES {
			Banner = 'js-banner',
		}

		enum ID {
			banner_BETA = 'banner_beta',
			banner_BETA_close = 'banner_beta_close',
			banner_COVID = 'banner_covid',
			banner_COVID_close = 'banner_covid_close',
		}

		const init = () => {
			const banner_BETA = document.getElementById(ID.banner_BETA),
				banner_BETA_close = document.getElementById(ID.banner_BETA_close),
				banner_COVID = document.getElementById(ID.banner_COVID),
				banner_COVID_close = document.getElementById(ID.banner_COVID_close);

			if (banner_BETA !== null) {
				if (document.cookie.split(';').some((item) => item.trim().startsWith('banner_beta='))) {
					banner_BETA.style.display = 'none';
				} else {
					banner_BETA.style.display = 'block';
				}
				
				banner_BETA_close.addEventListener('click', (event) => {
					banner_BETA.style.display = 'none';
					document.cookie = 'banner_beta=hidden';
				});
			}

			if (banner_COVID !== null) {
				if (document.cookie.split(';').some((item) => item.trim().startsWith('banner_covid='))) {
					banner_COVID.style.display = 'none';
				} else {
					banner_COVID.style.display = 'block';
				}

				banner_COVID_close.addEventListener('click', (event) => {
					banner_COVID.style.display = 'none';
					document.cookie = 'banner_covid=hidden';
				});
			}
		};

		!!document.querySelector('.' + CLASSES.Banner) && init();
	}
}

export default GetRangeValue;

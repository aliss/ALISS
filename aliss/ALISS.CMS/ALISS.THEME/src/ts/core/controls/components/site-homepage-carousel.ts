/** @format */

class HomepageCarousel {
	constructor() {

		enum ID {
			carousel = 'aliss-homepage-carousel',
			leftArrow = 'aliss-homepage-carousel-control-left',
			rightArrow = 'aliss-homepage-carousel-control-right',
			homepageSearchSuggestionsItemCount = 'homepage-search-suggestions-item-count',
			carouselControls = 'aliss-homepage-carousel-controls'
		}

		enum CLASSES {
			carouselImage = 'aliss-search-suggestions__carousel-image',
			slidingTransition = 'sliding-transition',
			carouselWidthAuto = 'aliss-search-suggestions__carousel--width-auto',
			hide = 'hide'
		}

		const carousel = document.getElementById(ID.carousel) as HTMLElement;
		const leftArrow = document.getElementById(ID.leftArrow);
		const rightArrow = document.getElementById(ID.rightArrow);
		const images = document.querySelectorAll("." + CLASSES.carouselImage);
		const totalImages = Object.keys(images).length;
		// let imageWidth = 420;
		// let imageWidth = 372;
		let imageWidth = 340;
		let currentIndex = 0;
		let prevIndex: any;

		const init = () => {
			window.addEventListener('load', function () {
				adjustCarouselLayoutOnLoad();
				checkItemCount();
			}, true);

			window.addEventListener('resize', function () {
				adjustCarouselLayoutOnLoad();
				checkItemCount();
			}, true);
			
			scrollCarouselLeft();
			scrollCarouselRight();
		}

		const checkItemCount = () => {
			let itemCount = document.getElementById(ID.homepageSearchSuggestionsItemCount)?.innerHTML.toString();
			let carouselControls = document.getElementById(ID.carouselControls) as HTMLElement;
			if (window.innerWidth >= 1280) {
				if (Number(itemCount) > 3) {
					carouselControls?.classList.remove(CLASSES.hide);
				} else {
					carouselControls?.classList.add(CLASSES.hide);
				}
			} else if (window.innerWidth <= 1024 && Number(itemCount) > 2) {
				carouselControls?.classList.remove(CLASSES.hide);
			}
			else if (window.innerWidth <= 620 && Number(itemCount) > 1) {
				carouselControls?.classList.remove(CLASSES.hide);
			}

			if (Number(itemCount) <= 2 && window.innerWidth > 620) {
				carousel.classList.add(CLASSES.carouselWidthAuto);
			} else if (Number(itemCount) <= 2 && window.innerWidth <= 620) {
				carousel.classList.remove(CLASSES.carouselWidthAuto);
			}
		}

		const adjustCarouselLayoutOnLoad = () => {
			if (window.innerWidth <= 1280 && carousel != null) {
				// imageWidth = 376;
				// imageWidth = 328;
				imageWidth = 296;
			}
			else {
				// imageWidth = 420;
				// imageWidth = 372;
				imageWidth = 340;
			}
		}

		const scrollCarouselLeft = () => {
			leftArrow?.addEventListener("click", () => {
				prevIndex = currentIndex;
				currentIndex = (currentIndex - 1 + totalImages) % totalImages;
				if (carousel != null) {
					carousel.style.transform = `translateX(-${imageWidth}px)`;
					carousel?.insertBefore(images[currentIndex], carousel.firstChild);
				}
				setTimeout(() => {
					carousel.style.transform = "";
					carousel.classList.add(CLASSES.slidingTransition);
				}, 10);

				setTimeout(() => {
					carousel.classList.remove(CLASSES.slidingTransition);
				}, 490);
			});
		}

		const scrollCarouselRight = () => {
			rightArrow?.addEventListener("click", () => {
				carousel.classList.add(CLASSES.slidingTransition);

				prevIndex = currentIndex;
				currentIndex = (currentIndex + 1) % totalImages;

				carousel.style.transform = `translateX(-${imageWidth}px)`;

				setTimeout(() => {
					carousel.appendChild(images[prevIndex]);
					carousel.classList.remove(CLASSES.slidingTransition);
					carousel.style.transform = "";
				}, 500);
			});
		}

		init();
	}
}

export default HomepageCarousel;

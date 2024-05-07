/** @format */

class ImageGallery {
	constructor() {
      enum ID {
         caption = 'image-caption',
         slideNavLeft = 'slide-nav-left',
         slideNavRight = 'slide-nav-right',
		 slideCount = 'slide-count'
      }
      enum CLASSES {
         slideItem = 'aliss-media-section__slide__item',
		 slideItemActive = 'aliss-media-section__slide__item--active',
         thumbNail = 'aliss-media-section__thumbnail-image',
		 thumbNailActive = 'aliss-media-section__thumbnail-image--active'
      }
      const init = () => {
      	console.log("image gallery loaded");
      	let i;
      	let slides = document.getElementsByClassName(CLASSES.slideItem) as HTMLCollection;
      	let thumbNail = document.getElementsByClassName(CLASSES.thumbNail) as HTMLCollection;
      	let captionText = document.getElementById(ID.caption) as HTMLElement;
      	let slideCount = document.getElementById(ID.slideCount) as HTMLHeadingElement;

      	let slideIndex = 1 as number;
      	const showSlides = (n: number) => {
      		if (n > slides.length) {
      			slideIndex = 1;
      		}
      		if (n < 1) {
      			slideIndex = slides.length;
      		}
      		for (i = 0; i < slides.length; i++) {
      			let slide = slides[i] as HTMLElement;
      			slide.classList.remove(CLASSES.slideItemActive);
      		}
      		for (i = 0; i < thumbNail.length; i++) {
      			let image = thumbNail[i];
      			image.className = image.className.replace(" " + CLASSES.thumbNailActive, "");
      		}

			if(slides.length > 0){
				slides[slideIndex - 1].className += " " + CLASSES.slideItemActive;
				thumbNail[slideIndex - 1].className += " " + CLASSES.thumbNailActive;
				if(thumbNail[slideIndex - 1].alt != "Embedded Video"){
					captionText.innerHTML = thumbNail[slideIndex - 1].alt;
				}
				else {
					captionText.innerHTML = "";
				}
				slideCount.innerHTML = (slideIndex).toString() + "/" + thumbNail.length;
			}
      	}

      	showSlides(slideIndex);

      	const plusSlides = (n: number) => {
      		showSlides((slideIndex += n));
      	}

      	const currentSlide = (n: number) => {
      		showSlides((slideIndex = n));
      	}

      	const changeSlide = () => {
      		let left = document.getElementById(ID.slideNavLeft);
      		let right = document.getElementById(ID.slideNavRight);

      		left?.addEventListener('click', (event: any) => {
      			console.log("left nav clicked");
      			plusSlides(-1);
      		});
      		right?.addEventListener('click', (event: any) => {
      			console.log("right nav clicked");
      			plusSlides(1);
      		});
      	}
		
      	const selectSlide = () => {
			  for (i = 0; i < thumbNail.length; i++) {
      			let itemIdx = i + 1;
      			let item = thumbNail[i];
      			item.addEventListener('click', (event: any) => {
      				currentSlide(itemIdx);
      			});
      		}
      	}
         
      	changeSlide();
      	selectSlide();

      };

      init();
	}
}

export default ImageGallery;

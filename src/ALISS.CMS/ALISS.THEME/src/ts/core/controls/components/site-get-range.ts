/** @format */

class GetRangeValue {
	constructor() {
		enum CLASSES {
			RangeSlider = 'range-slider__range',
		}

		enum ID {
			RangeSlider = 'custom-radius-input',
			RangeSliderValue = 'custom-radius-input-value',
		}

		const init = () => {
			const selectElement = document.querySelector('.' + CLASSES.RangeSlider) as HTMLInputElement;
			selectElement!.addEventListener('change', (event) => {
				let rangeVal = selectElement!.value.substring(0, selectElement!.value.length - 3)
				document.getElementById(ID.RangeSliderValue)!.innerHTML = rangeVal + 'km';
			});
		};

		!!document.querySelector('.' + CLASSES.RangeSlider) && init();
	}
}

export default GetRangeValue;

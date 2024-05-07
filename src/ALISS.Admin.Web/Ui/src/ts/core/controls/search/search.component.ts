import Aliss from '../../../core/';

class Search {
	constructor() {

		enum ATTRIBUTES {
			src = 'data-search',
			item = 'data-autocomplete',
			title = 'title',
			aria = 'aria-hidden'
		}

		enum OPTIONS {
			url = '/data/search.json',
			minchar = 0
		}

		enum CLASSES {
			container = 'autocomplete',
			item = 'autocomplete__item',
			scroll = 'scroll',
			hide = 'hide',
			capitalize = 'txt--capitalize',
			margin = 'm--0',
			padding = 'p--0',
			itempadding = 'p--1',
			bdrB = 'bdr__bottom--gray',
			bdrT = 'bdr__top--gray',
			bdrL = 'bdr__left--gray',
			bdrR = 'bdr__right--gray',
			itembg = 'bg--white',
			itemcolor = 'color--black'
		}

		let results = [] as any;

		const search = document.querySelector(Aliss.Helpers.getAttribute(ATTRIBUTES.src)) as HTMLFormElement;
		const autocomplete = document.createElement(Aliss.Enums.Elements.Div) as HTMLDivElement;
		const list = document.createElement(Aliss.Enums.Elements.Unordered) as HTMLUListElement;
		const input = search.querySelector(Aliss.Enums.Elements.Input) as HTMLInputElement;

		const createElements = () => {
			Aliss.Helpers.addClass(autocomplete, CLASSES.hide);
			Aliss.Helpers.addClass(autocomplete, CLASSES.container);

			list.setAttribute(ATTRIBUTES.aria, "true");
			Aliss.Helpers.addClass(list, CLASSES.scroll);
			Aliss.Helpers.addClass(list, CLASSES.margin);
			Aliss.Helpers.addClass(list, CLASSES.padding);

			autocomplete.appendChild(list);
			search.appendChild(autocomplete);
		}

		const init = () => {
			createElements();
			
			Aliss.Helpers.getJson(OPTIONS.url, (data: any) => {
				results = JSON.parse(data);
			});

			input.onkeyup = () => {
				let result = [];
				if (input.value.length > OPTIONS.minchar) {
					list.innerHTML = '';
					result = createAutocomplete(input.value.toLowerCase());
					Aliss.Helpers.removeClass(autocomplete, CLASSES.hide);
					for (let i = 0; i < result.length; i++) {
						list.innerHTML += resultItem(result[i]).outerHTML;
					}
				} else {
					result = [];
					list.innerHTML = '';
					Aliss.Helpers.addClass(autocomplete, CLASSES.hide);
				}
			}

			input.onfocus = () => {
				Aliss.Helpers.removeClass(autocomplete, CLASSES.hide);
			}

			document.body.addEventListener(Aliss.Enums.Events.Click, (e: any) => {
				list.innerHTML = '';
				if (e.target.hasAttribute(ATTRIBUTES.item)) {
					input.value = e.target.title;
					search.submit();
				}
			});
		}

		const createAutocomplete = (val: any) => {
			let results_return = [];
			for (let i = 0; i < results.length; i++) {
				if (val === results[i].slice(0, val.length)) {
					results_return.push(results[i]);
				}
			}
			return results_return;
		}

		const resultItem = (result: any) => {
			let item = document.createElement(Aliss.Enums.Elements.ListItem);
			Aliss.Helpers.addClass(item, CLASSES.item);
			Aliss.Helpers.addClass(item, CLASSES.itempadding);
			Aliss.Helpers.addClass(item, CLASSES.bdrT);
			Aliss.Helpers.addClass(item, CLASSES.itembg);
			Aliss.Helpers.addClass(item, CLASSES.capitalize);
			Aliss.Helpers.addClass(item, CLASSES.itemcolor);

			item.setAttribute(ATTRIBUTES.item, '');
			item.setAttribute(ATTRIBUTES.title, result);
			item.textContent = result;
			return item;
		};

		init();

	}
}

export default Search;

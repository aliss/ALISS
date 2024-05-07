class Slug {
	constructor() {

		enum ID {
			name = 'Name',
			slug = 'Slug',
			slugDisplay = 'SlugDisplay'
		}

		const init = () => {
			var categoryElement = document.getElementById(ID.name) as HTMLInputElement;
			if (typeof(categoryElement) != 'undefined' && categoryElement != null) {
				categoryElement.addEventListener("keyup", () => {
					var categoryName = categoryElement.value;
					categoryName = categoryName.replace(/^\s+|\s+$/g, '');
					categoryName = categoryName.toLowerCase();

					// remove accents, swap ñ for n, etc
					var from = "àáãäâèéëêìíïîòóöôùúüûñç·/_,:;";
					var to = "aaaaaeeeeiiiioooouuuunc------";
					for (var i = 0, l = from.length; i < l; i++) {
						categoryName = categoryName.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
					}

					categoryName = categoryName.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
						.replace(/\s+/g, '-') // collapse whitespace and replace by -
						.replace(/-+/g, '-'); // collapse dashes

					var slug = document.getElementById(ID.slug) as HTMLInputElement;
					var slugDisplay = document.getElementById(ID.slugDisplay) as HTMLInputElement;
					slug.value = categoryName;
					slugDisplay.value = categoryName;
				});
			}
		}

		init();

	}
}

export default Slug;

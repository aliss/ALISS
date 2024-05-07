const src = 'src';
const destination = 'dist';

const config = {
	browserSync: {
		files: [`${destination}/css/*`, `${destination}/js/**/*`, `${destination}/*.html`],
		options: {
			host: "109.146.129.79",
			server: destination,
		},
	},
	copy: {
		fonts: {
			files: `${src}/fonts/**/*.*`,
			destination: `${destination}/fonts`,
		},
		images: {
			files: `${src}/img/**/*.*`,
			destination: `${destination}/img`,
		},
	},
	javascript: {
		entry: `${src}/ts/site.ts`,
		files: [`${src}/ts/**/*.ts`],
		destination: `${destination}/js`,
	},
	nunjucks: {
		destination,
		files: `${src}/html/**/**.**`,
		pages: `${src}/html/pages/**/*.+(html|njk)`,
		templates: `${src}/html/templates`,
		partials: `${src}/html/partials`,
		macros: `${src}/html/macros`,
		data: `${src}/html/data/data.json`,
	},
	scss: {
		browserlist: ['> 0.01%', 'last 2 versions'],
		destination: `${destination}/css`,
		entry: `${src}/scss/**/site.scss`,
		files: `${src}/scss/**/*`,
		includePaths: ['node_modules/include-media/dist', 'node_modules/normalize-scss/sass'],
	},
};

module.exports = config;

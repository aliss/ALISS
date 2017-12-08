
// gulpfile.js

var foundation = true;
var bootstrap = false;

var $ = require('gulp-load-plugins')();
var gulp = require('gulp');
var sass = require('gulp-sass');
var changed = require('gulp-changed');
var svg = require('gulp-svg-sprite');
var sequence = require('run-sequence');
var del = require('del');
var cleanCSS = require('gulp-clean-css');
var merge = require('merge-stream');
var browserify = require('browserify');
var babel = require('babelify');
var buffer = require('vinyl-buffer');
var source = require('vinyl-source-stream');
var browserSync = require('browser-sync').create();

var URL = 'localhost/ALISSFrontEnd/build';

// Browsers to target when prefixing CSS.
var COMPATIBILITY = [
	'last 2 versions',
	'ie >= 9',
	'Android >= 2.3'
];

// File paths to various assets are defined here.
if(foundation == true) {
	var PATHS = {
		sass: [
			'node_modules/foundation-sites/scss',
			'node_modules/font-awesome/scss',
			'node_modules/slick-carousel/slick',
			'node_modules/video.js/src/css',
			'node_modules/'
		],
		javascript: [
			'node_modules/',
			'node_modules/foundation-sites/js'
		],
		pkg: [
			'**/*',
			'!**/node_modules/**',
			'!**/components/**',
			'!**/scss/**',
			'!**/bower.json',
			'!**/gulpfile.js',
			'!**/package.json',
			'!**/composer.json',
			'!**/composer.lock',
			'!**/codesniffer.ruleset.xml',
			'!**/packaged/*',
		]
	};
} else if(bootstrap == true) {
	var PATHS = {
		sass: [
			'node_modules/bootstrap-sass/assets/stylesheets',
			'node_modules/font-awesome/scss',
			'node_modules/slick-carousel/slick',
			'node_modules/video.js/src/css',
			'node_modules/'
		],
		javascript: [
			'node_modules/',
			'node_modules/bootstrap-sass/assets/javascripts',
		],
		pkg: [
			'**/*',
			'!**/node_modules/**',
			'!**/components/**',
			'!**/scss/**',
			'!**/bower.json',
			'!**/gulpfile.js',
			'!**/package.json',
			'!**/composer.json',
			'!**/composer.lock',
			'!**/codesniffer.ruleset.xml',
			'!**/packaged/*',
		]
	};
}

// Browsersync
gulp.task('browser-sync', ['build'], function() {
	var files = [
		'**/*.php',
		'**/*.html',
		'images/**/*.{png,jpg,gif}',
	];
	browserSync.init(files, {
		// Proxy address
		proxy: URL,
		// Port #
		port: 3000
	});
});

// Sass Processing
gulp.task('sass', function() {
	return gulp.src('src/scss/*.scss')
	.pipe($.sourcemaps.init())
	.pipe($.sass({
		includePaths: PATHS.sass
	}))
	.on('error', $.notify.onError({
		message: "<%= error.message %>",
		title: "Sass Error"
	}))
	.pipe(cleanCSS())
	.pipe($.autoprefixer({
		browsers: COMPATIBILITY
	}))
	.pipe($.concat('styles.css', {
		newLine:'\n;'
	}))
	// Minify CSS if run with --production flag
	.pipe($.sourcemaps.write('.'))
	.pipe(gulp.dest('build/css'))
	.pipe(browserSync.stream({match: '**/*.css'}));
});

// Javascript Files
gulp.task('lint', function() {
	return gulp.src('src/js/*.js')
		.pipe($.jshint())
		.pipe($.notify(function (file) {
		if (file.jshint.success) {
			return false;
		}
		var errors = file.jshint.results.map(function (data) {
		if (data.error) {
		  return "(" + data.error.line + ':' + data.error.character + ') ' + data.error.reason;
		}
		}).join("\n");
		return file.relative + " (" + file.jshint.results.length + " errors)\n" + errors;
	}));
});
gulp.task('javascript', function() {
	var uglify = $.uglify()
	    .on('error', $.notify.onError({
	      message: "<%= error.message %>",
	      title: "Uglify JS Error"
	    }));
	return browserify({
		entries: 'src/js/scripts.js',
		paths: PATHS.javascript
	})
	.transform(babel)
 	.bundle()
 	.on('error', $.notify.onError({
		message: "<%= error.message %>",
		title: "JS Error"
	}))
	.pipe(source('scripts.js'))
	.pipe(buffer())
	.pipe(uglify)
	.pipe($.sourcemaps.init())
	.pipe(gulp.dest('build/js'))
	.pipe(browserSync.stream());
});

// Copy
// gulp.task('copy', function() {
// 	// Font Awesome
// 	var fontAwesome = gulp.src('node_modules/font-awesome/fonts/**/*.*')
// 		.pipe(gulp.dest('build/fonts'))
// 	return merge(fontAwesome);
// });

// Move
gulp.task('move', function() {
	// Front End
	gulp.src(['src/frontend/**/*', '!src/frontend/ref', '!src/frontend/ref/**'])
		.pipe(changed('build'))
		.pipe(gulp.dest('build'))
		.pipe(browserSync.stream());
	// Fonts
	gulp.src('src/fonts/**/*')
		.pipe(changed('build/fonts'))
		.pipe(gulp.dest('build/fonts'))
		.pipe(browserSync.stream());
	// Images
	gulp.src(['src/img/**/*', '!src/img/svg-sprite/*.svg', '!src/img/svg-sprite'])
		.pipe(changed('build/img'))
		.pipe(gulp.dest('build/img'))
		.pipe(browserSync.stream());
});

// SVG
gulp.task('svg', function() {
	gulp.src(['src/img/svg-sprite/**/*.svg'])
		.pipe(svg({
			"mode" : {
				"symbol" : {
				"dest" : "../img/svg-sprite/",
				"svg" : {
					"xmlDeclaration" : ""
				},
				"sprite" : "symbols.svg"
			}
		}
	}))
	.pipe(gulp.dest('build/img'))
	.pipe(browserSync.stream());
});

// Clean CSS
// Clean JS
gulp.task('clean', function(done) {
	sequence(['clean:javascript', 'clean:css', 'clean:svg'],
		done);
});
gulp.task('clean:javascript', function() {
	return del([
		'build/js/scripts.js'
	]);
});
gulp.task('clean:css', function() {
	return del([
		'build/css/styles.css',
		'build/css/styles.css.map'
	]);
});
gulp.task('clean:javascript', function() {
	return del([
		'build/js/scripts.js'
	]);
});
gulp.task('clean:svg', function() {
	return del([
		'build/img/svg-sprite/symbols.svg'
	]);
});

// Build task
gulp.task('build', ['clean'], function(done) {
	sequence(['move', 'sass', 'javascript', 'lint', 'svg'],
		done);
});

// Default gulp task
gulp.task('default', ['build', 'browser-sync'], function() {
  	// Log file changes to console
	function logFileChange(event) {
		var fileName = require('path').relative(__dirname, event.path);
		console.log('[' + 'WATCH' + '] ' + fileName + ' was ' + event.type + ', running tasks...');
	}
	// PHP
	gulp.watch(['src/frontend/**/*'], ['move'])
		.on('change', function(event) {
			logFileChange(event);
		}
	);
	// SVG
	gulp.watch(['src/img/svg-sprite/*.svg'], ['clean:svg', 'svg'])
		.on('change', function(event) {
			logFileChange(event);
		}
	);
  	// Sass Watch
	gulp.watch(['src/scss/**/*.scss'], ['clean:css', 'sass'])
		.on('change', function(event) {
			logFileChange(event);
		}
	);
	// Move Images
	gulp.watch(['src/img/**/*'], ['move'])
		.on('change', function(event) {
			logFileChange(event);
		}
	);
	// JS Watch
	gulp.watch(['src/js/**/*.js'], ['clean:javascript', 'javascript', 'lint'])
		.on('change', function(event) {
			logFileChange(event);
		}
	);
});





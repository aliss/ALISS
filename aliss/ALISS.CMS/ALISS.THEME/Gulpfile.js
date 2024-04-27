const { parallel } = require('gulp');
const browserSync = require('./gulp-tasks/browserSync');
const { copyFonts, copyImages } = require('./gulp-tasks/copy');
const typescript = require('./gulp-tasks/typescript');
const { lintJS, lintSCSS } = require('./gulp-tasks/lint');
const nunjucks = require('./gulp-tasks/nunjucks');
const scss = require('./gulp-tasks/scss');
const watchFiles = require('./gulp-tasks/watchFiles');

const copy = parallel(copyFonts, copyImages);
const lint = parallel(lintJS, lintSCSS);
const build = parallel(copy, lint, typescript, nunjucks, scss);
const serve = parallel(build, browserSync);

exports.default = parallel(serve, watchFiles);

exports.development = parallel(build);

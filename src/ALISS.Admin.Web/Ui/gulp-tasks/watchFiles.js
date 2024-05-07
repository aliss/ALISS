const { parallel, watch } = require('gulp');
const { copyFonts, copyImages } = require('./copy');
const { lintJS } = require('./lint');
const typescript = require('./typescript');
const nunjucks = require('./nunjucks');
const scss = require('./scss');
const config = require('../GulpConfig');

const watchTS = () => watch(config.javascript.files, parallel(lintJS, typescript));
const watchSCSS = () => watch(config.scss.files, scss);
const watchHTML = () => watch(config.nunjucks.files, nunjucks);
const watchImages = () => watch(config.copy.images.files, copyImages);
const watchFonts = () => watch(config.copy.fonts.files, copyFonts);

const watchFiles = parallel(
  watchTS,
  watchSCSS,
  watchHTML,
  watchImages,
  watchFonts,
);

module.exports = watchFiles;

const { src, dest } = require('gulp');
const rename = require('gulp-rename');
const autoprefixer = require('autoprefixer');
const postcss = require('gulp-postcss');
const sassGlob = require('gulp-sass-glob');
const pcssVmin = require('postcss-vmin');
const pcssOpacity = require('postcss-opacity');
const pcssDiscardComments = require('postcss-discard-comments');
const pcssDiscardEmpty = require('postcss-discard-empty');
const mqpacker = require('css-mqpacker');
const pixrem = require('pixrem');
const cssnano = require('cssnano');
const sass = require('gulp-sass')(require('sass'));
const plumber = require('gulp-plumber');
const printError = require('./printError');
const {
	scss: { browsers, destination, entry, includePaths },
} = require('../GulpConfig');

const prefix = [
  autoprefixer({ browsers }),
];

const compact = [
  pixrem({
    atrules: true,
  }),
  pcssOpacity,
  pcssVmin,
  pcssDiscardComments,
  pcssDiscardEmpty,
  cssnano({ zindex: false }),
];

function scss() {
  return src(entry)
    .pipe(sassGlob())
    .pipe(plumber(printError))
    .pipe(
      sass({
        includePaths,
      }),
    )
    .pipe(postcss(prefix))
    .pipe(dest(destination))
    .pipe(postcss(compact))
    .pipe(rename({ suffix: '.min' }))
    .pipe(dest(destination));
} 

module.exports = scss;

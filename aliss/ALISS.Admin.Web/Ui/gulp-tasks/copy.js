const { src, dest } = require('gulp');
const plumber = require('gulp-plumber');
const {
  copy: { fonts, images },
} = require('../GulpConfig');
const printError = require('./printError');

function performCopy({ destination, files }) {
  return src(files)
    .pipe(plumber(printError))
    .pipe(dest(destination));
}

const copyFonts = () => performCopy(fonts);
const copyImages = () => performCopy(images);

exports.copyFonts = copyFonts;
exports.copyImages = copyImages;

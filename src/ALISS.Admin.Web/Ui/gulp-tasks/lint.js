const { src } = require('gulp');
const eslint = require('gulp-eslint');
const reporter = require('postcss-reporter');
const postcss = require('gulp-postcss');
const syntaxScss = require('postcss-scss');
const { javascript, scss } = require('../GulpConfig');

function lintJS() {
  return src(javascript.files)
    .pipe(eslint())
    .pipe(eslint.format());
}

function lintSCSS() {
  const processors = [
    reporter({
      clearMessages: true,
      throwError: true,
    }),
  ];

  return src(scss.files).pipe(postcss(processors, { syntax: syntaxScss }));
}

module.exports = {
  lintJS,
  lintSCSS,
};

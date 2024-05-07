const { src, dest } = require('gulp');
const data = require('gulp-data');
const render = require('gulp-nunjucks-render');
const plumber = require('gulp-plumber');
const printError = require('./printError');
const {
  nunjucks: {
    pages, templates, partials, macros, destination,
  },
} = require('../GulpConfig');

function nunjucks() {
  return src(pages)
    .pipe(plumber(printError))
    .pipe(data(() => require('../src/html/data/data.json')))
    .pipe(
      render({
        path: [templates, partials, macros],
      }),
    )
    .pipe(dest(destination));
}

module.exports = nunjucks;

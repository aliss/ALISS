// Might want to look at old way if builds are slow/incremental builds
const { dest } = require('gulp');
const browserify = require('browserify');
const source = require('vinyl-source-stream');
const buffer = require('vinyl-buffer');
const uglify = require('gulp-uglify');
const rename = require('gulp-rename');
const {
  javascript: { entry, destination },
} = require('../GulpConfig');

function compileTypeScript() {
  const extensions = ['.js', '.ts'];

  return browserify({ entries: entry, debug: true, extensions })
    .transform('babelify', { extensions })
    .bundle()
    .pipe(source('site.js'))
    .pipe(buffer())
    .pipe(dest(destination))
    .pipe(uglify())
    .pipe(rename({ suffix: '.min' }))
    .pipe(dest(destination));
}

module.exports = compileTypeScript;

const { red } = require('ansi-colors');
const beeper = require('beeper');
const log = require('fancy-log');

function printError(err) {
  beeper();
  log(red(err));
  this.emit('end');
}

module.exports = printError;

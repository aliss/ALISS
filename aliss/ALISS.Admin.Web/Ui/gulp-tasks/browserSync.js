const browserSync = require('browser-sync');
const { browserSync: { files, options } } = require('../GulpConfig');

const browserSyncServer = () => browserSync.init(files, options);

module.exports = browserSyncServer;

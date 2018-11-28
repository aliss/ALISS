$ = global.$ = global.jQuery = require('jquery');

$.extend($.expr[':'], { 
  'containsi': function(elem, i, match, array) {
    return (elem.textContent || elem.innerText || '').toLowerCase()
    .indexOf((match[3] || "").toLowerCase()) >= 0;
  }
});

$.urlParam = function(name, urlString){
  if (!urlString){ urlString = window.location.href; }
  var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(urlString);
  if (results==null){
    return null;
  } else {
    return decodeURI(results[1]) || 0;
  }
};

$.urlParams = function(urlString){
  if (!urlString){ urlString = window.location.href; }
  var hash = {};
  var query_section = urlString.split('?');
  query_section = query_section[1];
  query_section = query_section.split('&');
  var parameters = query_section.map(function(i,e){ return i.split('=')[0]; });
  for (let p of parameters){
    hash[p] = $.urlParam(p, urlString)
  }
  return hash;
};

$.sanitisedParams = function(urlString){
  var hash = $.urlParams(urlString);
  for (let k of Object.keys(hash)){
    if (typeof hash[k] != "string") { continue; }
    hash[k] = hash[k].replace(/\+/gi, " ");
    hash[k] = hash[k].replace(/-/gi, " ");

    if (hash[k].indexOf(" ") >= 0){
      hash[k] = hash[k].trim();
      var uncapped_words = hash[k].split(" ");
      var capped_words = uncapped_words.map(function(word){
        if (word == "and") {
          return word;
        } else {
          word =  word[0].toUpperCase() + word.slice(1);
          return word;
        }
      });
      capped_words = capped_words.join(" ");
      hash[k] = capped_words;
    }
    // hash[] = query_pair[1][0].toUpperCase() + query_pair[1].slice(1);
  }
  return hash;
};
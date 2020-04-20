//  ALISS
//  ================================
//  Habanero + Braw Software
//  http://habanero.digital/
//  http://www.thisisbraw.co.uk/
//

import CC from 'cookieconsent/build/cookieconsent.min.js';

const $ = global.$ = global.jQuery = require('jquery');
require('foundation-sites/dist/js/foundation.min.js');
require('pickadate/lib/picker.js');
require('pickadate/lib/picker.date.js');
$(document).foundation();
require('./partials/select2.min.js');

import matchHeight from './partials/match-height';
import svg4everybody from 'svg4everybody/dist/svg4everybody.js';
import Clipboard from 'clipboard/lib/clipboard.js';
import './partials/extensions';
import easyAutocomplete from 'easy-autocomplete/dist/jquery.easy-autocomplete.min.js';
import observeModals from './modals';

var ALISS = require('./aliss');

$(document).ready(() => {

  window.ALISS = ALISS;

  //CATEGORY SELECTION
  var checkMaxCategories = function() {
    var $check_three = $('.all-categories input');
    $check_three.each(function(index, el) {
      var $thisCheck = $(this);
      var name = $thisCheck.attr('name');
      // console.log(name);
      var limit = 4;
      $(`input[name='${name}']`).on('change', function(evt) {
        if($(`input[name='${name}']:checked`).length >= limit) {
           this.checked = false;
         }
      });
    });
  };

  $('input[name="categories"]').on('change', function(evt) {
    if($('.all-categories input:checkbox:checked').length > 0) {
      $('.selected-categories').addClass('active');
    } else {
      $('.selected-categories').removeClass('active');
    }
    var $thisCheck = $(this);
    var value = $thisCheck.attr('value');
    var label = $thisCheck.parent().children('span.name').html();
    // console.log(label);
    if($thisCheck.prop('checked')) {
      // console.log('checked');
      if($('.all-categories input:checkbox:checked').length < 4) {
        $('.selected-categories .cats').append(`<div class="selected-cat" data-cat="${value}"><span class="remove"></span>${label}</div>`);
      }
      else {
        // console.log("You can't add more categries!");
        if (!$('.cat-warning').length){
          $('.all-categories').prepend("<h3 class='cat-warning'>You can only select 3 categories.</h3>");
        }
      }
    } else {
      // console.log('unchecked');
      $(`.selected-categories .cats .selected-cat[data-cat='${value}']`).remove();
      if ($('.cat-warning').length){
        $('.cat-warning').remove();
      }
    }
    $('.selected-cat span.remove').click(function(){
      var value = $(this).parent().attr('data-cat');
      // console.log(value);
      $(this).parent().remove();
      if ($('.cat-warning').length){
        $('.cat-warning').remove();
      }
      $(`input[value="${value}"]`).prop('checked', false);
      if($('.selected-categories .cats').is(':empty')) {
        $('.selected-categories').removeClass('active');
      }
    });
  });

  if($('.all-categories input:checkbox:checked').length > 0) {
    $('.selected-categories').addClass('active');
    $('.all-categories input:checkbox:checked').each(function(index, el) {
      var $thisCheck = $(this);
      var value = $thisCheck.attr('value');
      var label = $thisCheck.parent().children('span.name').html();
      $('.selected-categories .cats').append(`<div class="selected-cat" data-cat="${value}"><span class="remove"></span>${label}</div>`);
    });
  } else {
    $('.selected-categories').removeClass('active');
  }

  $('.selected-cat span.remove').click(function(){
    var value = $(this).parent().attr('data-cat');
    // console.log(value);
    $(this).parent().remove();
    $(`input[value="${value}"]`).prop('checked', false);
    if($('.selected-categories .cats').is(':empty')) {
      $('.selected-categories').removeClass('active');
    }
  });


  //LOCATIONS
  var isLocationValid = function(){
    var result = true;
    $('div.add-location-form input.required').each(function(i,e){
      e.setCustomValidity('');
      if (e.value == ""){
        e.setCustomValidity("Please fill in this field");
      }
      if (e.checkValidity() == false){
        e.reportValidity();
        result = false;
        return false;
      }
    });
    return result;
  };

  var createLocation = function(createEndpoint) {
    $.ajax({
      headers: { 'X-CSRFToken': $('#location_csrf').val() },
      url: createEndpoint,
      type: "POST",
      dataType: 'json',
      data: {
        name: $('#location_name').val(),
        street_address: $('#location_street_address').val(),
        locality: $('#location_locality').val(),
        postal_code: $('#location_postal_code').val()
      },
      success : function(json) {
        $('#location_name').val('');
        $('#location_street_address').val('');
        $('#location_locality').val('');
        $('#location_postal_code').val('');
        var newOption = new Option(json.address, json.pk, false, false);
        $('#id_locations').append(newOption).trigger('change');
        var selection = $('#id_locations').val();
        selection.push(json.pk);
        $('#id_locations').val(selection);
        $('#add-location-fieldset').removeClass('active');
        $('.add-location-form').slideUp();
      },
      error : function(xhr,errmsg,err) {
        $('#results').html("<div class='alert-box alert radius' data-alert>Oops! We have encountered an error: "+errmsg+
          " <a href='#' class='close'>&times;</a></div>"); // add the error to the dom
        console.log(xhr.status + ": " + xhr.responseText); // provide a bit more info about the error to the console
      },
      complete: function(){
        $('#add-location').removeAttr('disabled');
      }
    });
  };

  // Select2
  $('.multiselect select').hide();
  $('#id_locations').select2({
    placeholder: "Select Locations",
    mutliple: true
  });

  $('#id_service_areas').select2({
    placeholder: "Select Service Areas",
    mutliple: true
  });

  $('#show-add-location').click(function(e){
    e.stopPropagation();
    e.preventDefault();
    $('#add-location-fieldset').toggleClass('active');
    $('.add-location-form').slideToggle();
  });

  $('#add-location').click(function(e){
    e.stopPropagation();
    e.preventDefault();
    if (isLocationValid()){
      var endpoint = $(this).attr('data-create-endpoint');
      $('#add-location').attr('disabled', 'disabled');
      createLocation(endpoint);
    }
  });

  $(document).click(function(){
    $('.navigation').removeClass('active');
    $('body').removeClass('restrict-height');
    $("#menu_toggle").removeClass('active');
    $(".category-selector .cells > ul > li").removeClass('active');
  });

  $('.navigation a, .category-selector .cells > ul > li a, .category-selector .cells > ul > li span').click(function(e){
    e.stopPropagation();
  });

  $('.category-selector a.active-cat').click(function(e){
    e.preventDefault();
  });

  $("#menu_toggle").click(function(e) {
    e.stopPropagation();
    $(this).toggleClass('active');
    $('body').toggleClass('restrict-height');
    $(".navigation").toggleClass('active');
  }).keypress(function(e) {
    e.stopPropagation();
    $(this).toggleClass('active');
    $('body').toggleClass('restrict-height');
    $(".navigation").toggleClass('active');
  });

  // Site Wide Toggles
  $('.toggled').each(function(index, el) {
    var $thisToggle = $(this);
    var id = $thisToggle.attr('id');
    $(`#${id}_toggle`).attr('tabindex', '0');
    $(`#${id}_toggle`).attr('role', 'button');
    $(`#${id}_toggle`).attr('aria-expanded', false);
    $(`#${id}_toggle`).attr('aria-controls', `#${id}`);
    $(`#${id}_toggle`).click(function() {
      $(`#${id}`).toggleClass('active');
      $(this).toggleClass('active');
      if ($(this).hasClass('active')){
        $(`#${id}_toggle`).attr('aria-expanded', true);

      }
      else {
        $(`#${id}_toggle`).attr('aria-expanded', false);
      }
    }).keypress(function() {
      $(`#${id}`).toggleClass('active');
      $(this).toggleClass('active');
      if ($(this).hasClass('active')){
        $(`#${id}_toggle`).attr('aria-expanded', true);
      }
      else {
        $(`#${id}_toggle`).attr('aria-expanded', false);
      }
    });
  });

  // Results Areas Toggle
  $('.service-areas a').click(function() {
    $(this).toggleClass('active');
    var list = $(this).closest('.contact-info').next('.service-areas-list');
    // console.log(list);
    list.toggleClass('active');
  }).keypress(function() {
    $(this).toggleClass('active');
    var list = $(this).closest('.contact-info').next('.service-areas-list');
    // console.log(list);
    list.toggleClass('active');
  });
  $('.location a.more-link').click(function() {
    $(this).toggleClass('active');
    var locations = $(this).parent('.more').next('.locations-list');
    // console.log(locations);
    locations.toggleClass('active');
  }).keypress(function() {
    $(this).toggleClass('active');
    var locations = $(this).parent('.more').next('.locations-list');
    // console.log(locations);
    locations.toggleClass('active');
  });
  $('ul.areas-breakdown > li > a').click(function() {
    $(this).toggleClass('active');
    var services = $(this).next('.region-services-list');
    // console.log(locations);
    services.toggleClass('active');
  });

  // Description Toggle
  if($('.desc.long').length > 0) {
    $('.desc.long').after('<p><a class="read-more"><span class="more">Read More</span><span class="less">Hide</span></a></p>');
  }
  $('a.read-more').click(function() {
    $(this).toggleClass('active');
    $('.desc.long').toggleClass('active');
  });

  // Toggle Child Categories
  var toggleChildCategories = function(target, addOrRemove) {
    $(target).toggleClass('active', addOrRemove);
    var children = $(target).next('ul');
    children.toggleClass('active', addOrRemove);
  };

  $('.radio-list.children .toggle-children, .checkbox-list.children .toggle-children').click(function(){
    toggleChildCategories(this);
  });

  //Handle category filtering in category select
  $("#category-filter").show();
  $("#category-filter input").keyup(function () {
    var rows = $(".all-categories ul").find("li");
    rows.hide();
    if (this.value.length) {
      toggleChildCategories($('span.toggle-children'), true);
      var data = this.value.split(" ");
      $.each(data, function (i, v) {
        rows.filter(":containsi('" + v + "')").show();
      });
    } else {
      toggleChildCategories($('span.toggle-children'), false);
      rows.show();
    }
  });

  // Cat Menu
  $(".category-selector ul > li > a.active-cat, .category-selector .cells > ul > li > a.select-category, .category-selector .cells > ul > li > span.select").click(function(e) {
    var parent = $(this).parent('li');
    parent.toggleClass('active');
  }).keypress(function(e) {
    var parent = $(this).parent('li');
    parent.toggleClass('active');
  });

  // Feedback Form Toggle
  $('.feedback-form a.no').click(function() {
    $(this).toggleClass('active');
    $('.feedback-form .form').toggle();
  });

  // Recommend Modal
  $('#recommend .button').hide();
  $('#recommend select').change(function(){
    if($(this).val() == 'new') {
      $('#recommend input[type=submit]').hide();
      $('#recommend .button').show();
    } else {
      $('#recommend input[type=submit]').show();
      $('#recommend .button').hide();
    }
  });

  // Messages Hide
  if($('.messages').length > 0) {
    setTimeout(function() {
      $('.messages').css('max-height', '0');
    }, 5500);
  }

  // Notifications Toggle
  $(document).click(function(){
    $('#notifications').removeClass('active');
    $('#notifications_toggle').removeClass('active');
  });
  $('.notifications').click(function(e){
    e.stopPropagation();
  });
  $("#notifications_toggle").click(function(e) {
    e.stopPropagation();
    $(this).toggleClass('active');
    $("#notifications").toggleClass('active');
  });

  function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for(var i = 0; i < hashes.length; i++)
    {
      hash = hashes[i].split('=');
      vars.push(hash[0]);
      vars[hash[0]] = hash[1];
    }
    return vars;
  }

  var report = getUrlVars().report;
  if(report == 'True') {
    // console.log('test');
    $(".feedback-form a.no").click();
    $('html, body').animate({
      scrollTop: ($('.feedback-form').offset().top)
    }, 500);
  }

  //SHARE FORM
  if($('.share-form').length > 0) {
    var default_url = $('#share_url').val();
    // console.log(default_url);
    var postcodeGet = $('.share-form input.postcode').val();
    var postcode = postcodeGet.replace(/\s/g,"");
    // console.log(postcode);
    var categoryGet = $('.share-form input.category').val();
    var category = categoryGet.replace(/\s/g,"+");
    // console.log(category);
    if(category == '') {
      $('#share_url').val(default_url + postcode);
    } else {
      $('#share_url').val(default_url + postcode + '/' + category);
    }
    $('.share-form input.postcode').on('input', function() {
      if($(this).val() != "") {
        $('.share-form input.category').prop('disabled', false);
      } else {
        $('.share-form input.category').prop('disabled', true);
      }
    });
    $('.share-form input.postcode').on('change', function() {
      var postcodeGet = $(this).val();
      var postcode = postcodeGet.replace(/\s/g,"");
      $('#share_url').val(default_url + postcode);
    });
    $('.share-form input.category').on('change', function() {
      var categoryGet = $(this).val();
      var category = categoryGet.replace(/\s/g,"+");
      var postcodeGet = $('.share-form input.postcode').val();
      var postcode = postcodeGet.replace(/\s/g,"");
      $('#share_url').val(default_url + postcode + '/' + category);
    });
    if($('.share-form input.postcode').val() != "") {
      $('.share-form input.category').prop('disabled', false);
    } else {
      $('.share-form input.category').prop('disabled', true);
    }
    $('#copy_search_link').click(function(){
      if($('.share-form input.postcode').val() == "") {
        $(".share-error").addClass('active');
        $(".copy-error").removeClass('active');
        $(".share-success").removeClass('active');
      } else {
        $(".share-error").removeClass('active');
        $(".copy-error").removeClass('active');
        var copy_link = new Clipboard('#copy_search_link', {
          text: function() {
            return document.querySelector('input#share_url').value;
          }
        });
        copy_link.on('success', function(e) {
          // console.log(e);
          $(".share-success").addClass('active');
          $(".copy-error").removeClass('active');
        });
        copy_link.on('error', function(e) {
          // console.log(e);
          $(".copy-error").addClass('active');
        });
      }
    });

    if($('.share-success').hasClass('active')) {
      setTimeout(function() {
        $('.share-success').removeClass('active');
      }, 2500);
    }
  }

  $('ul.progress-breadcrumb label').each(function(i,l){
    $(l).click(function(e){
      var target = '#' + $(l).attr('for');
      $(target).addClass('glow');
      $(target).addClass('start-glow');
      setTimeout(function(){ $(target).removeClass('glow'); }, 2000);
      setTimeout(function(){ $(target).removeClass('start-glow'); }, 2000);
    });
  });

  var toggleClearableInputs = function(){
    $('div.clearable-input').each(function(i,target){
      //console.log($(target).find('input'));
      var showClearBtn = ($(target).find('input').val().length > 0);
      $("div.clearable-input").toggleClass('active', showClearBtn);
    });
  };

  var handleClearInputs = function(){
    $("div.clearable-input input").keyup(toggleClearableInputs);
    $("div.clearable-input i.clear-input").click(function(){
      $(this).siblings('input').val('');
      $(this).siblings('input').trigger('keyup');
    });
  };

  var storeSearchUrl = function(){
    var locationURL = $(location).attr('href');
    if ((locationURL.indexOf("search") >= 0) && (locationURL.indexOf('postcode') >= 0)){
      localStorage.setItem('searchURL',locationURL);
    }
  };

  //ORGANISATION FILE UPLOAD
  var handleFileInput = function(){
    $('#id_logo').change(function(){
      if ($(this)[0].files.length > 0) {
        var filename = $(this)[0].files[0].name;
        $('label.create-logo span').text(filename + ' ready for upload');
        $('label.create-logo img').attr('src', '/static/img/image-attached.png');
      } else {
        $('label.create-logo span').text('No image attached');
        $('label.create-logo img').attr('src', 'no-image.png');
      }
    });
  };

  var handleTabs = function(){
    $('.tab').click(function(){
      $(this).siblings().removeClass('active');
      $(this).addClass('active');
      $($(this).attr('data-parent')).children().removeClass('active');
      $($(this).attr('data-tab')).addClass('active');
    });
  };

  window.handleDistanceFiltering = function(){
    var urlString = window.location.href;
    if (urlString.includes('places')){
        $("#custom-radius-radio").hide();
    }
    $("#custom-radius-radio").click(function(){
      $("div.filter-distance > ul").children().removeClass("active");
      $("#custom-radius-radio").addClass("active");
      $("#custom-radius-input").removeAttr("disabled");
    });

    if (!($("div.filter-distance > ul").children().hasClass("active"))){
      urlString = window.location.href;
      var url = new URL(urlString);
      var searchedRadius = url.searchParams.get("radius");
      $("#custom-radius-radio").addClass("active");
      $("#custom-radius-input").removeAttr("disabled");
      $("#custom-radius-input").val(searchedRadius);
    }
  };

  window.handleRangeSlider = function(){
    var slider = $('.range-slider'),
      range = $('.range-slider__range'),
      value = $('.range-slider__value');

    value.each(function(){
      $(this).html($(this).prev().attr('value') / 1000 + "km");
    });

    range.on('input', function(){
      $(this).next(value).html(this.value / 1000 + "km");
    });
  };

  window.category_change_keyword_check = function(){
    var checkword = function(){
      var new_term = $("input[name='q']").val();
      $(".category-selector a").not(".select-category").prop("href", function(i, href){
        return href.replace(/q.*?&/, "q=" + new_term + "&");
      });
    };

    if ($("input[name='q']").val() != "") {
      $(".category-selector *").click(checkword);
    }
  };

  window.setMapSize = function(targetId){
    var innerWidth = window.innerWidth;
    var desiredSize = 0;
    if (innerWidth < 900){
      desiredSize = (0.9 * innerWidth);
    }
    else {
      desiredSize = (0.4 * innerWidth);
    }
    $('#' + targetId).width(desiredSize);
    $('#' + targetId).height(desiredSize);
  };

  window.renderMap = function(targetId) {
    var mymap = L.map(targetId).setView([56.816922, -4.18265], 6);
    L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, ' +
        '<a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, ' +
        'Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    }).addTo(mymap);
    return mymap;
  };

  window.renderFeatures = function(mymap, serviceId){
    var features = $.ajax({
      url: "/api/v4/service-area-spatial/?service_id=" + serviceId,
      dataType : "json",
      success: function(result){
        var singleArea = false;
        if (result.length == 1){
          singleArea = true;
        }
        result.forEach(function(feature){
          if (feature.length != 0){
            var geo_feature = feature;
            var feature_name = '';
            var name_keys = ['lad18nm', 'HBName', 'HIAName', 'ctry17nm'];
            name_keys.forEach(function(name_key){
              if (geo_feature.properties[name_key]){
                feature_name = geo_feature.properties[name_key];
                return feature_name;
              }
            });
            var geoJSON = L.geoJson(geo_feature).addTo(mymap).bindPopup(`<b>${feature_name}</b>`);
            if (singleArea){
              if (geo_feature.properties.long){
                var long = (geo_feature.properties.long);
                var lat = (geo_feature.properties.lat);
                mymap.setView([lat, long], 6);
              }
            }
          }
        });
      }
    });
  };

  window.addLocations = function(mymap, locations, no_geo_features = true){
    var singleLocation = false;
    if (Object.keys(locations).length == 1){
      singleLocation = true;
    }
    $.each(locations, function(key, value){
      L.marker(value).addTo(mymap)
      .bindPopup(`<a title="Click here to view this location on google maps (This will open in a new window)." href=https://maps.google.com/?q=${value[0]},${value[1]} target="_blank">${key}</a>`);
      if (singleLocation && no_geo_features){
          mymap.setView(value, 6);
      }
    });

  };

  window.clearFeatures = function(mymap){
    mymap.eachLayer(function(layer){
      if(!(layer._url && layer._url.includes('tile'))){
        layer.remove();
      }
    });
  };

  window.addLoadingIndicator = function(mapid){
    var loading_indicator = "<h3 id='loading' style='position: relative; top: 50%; text-align: center; margin: 0;'>Loading...</h3>";
    $('#mapid').append(loading_indicator);
  };

  window.setupDataSetLinks = function(list_items, mymap){
    list_items.each(function(){
      $('#'+ this.id).click(function(){
        list_items.removeClass("active");
        $('#'+ this.id).addClass("active");
        var features = $.ajax({
          url: "/api/v4/service-area-spatial/full-set/?type=" + this.value,
          beforeSend: function(){
            $(".leaflet-pane").hide();
          },
          dataType : "json",
          success: function(result){
            clearFeatures(mymap);
            $(".leaflet-pane").show();
            var name_key = result.name_key;
            result.data.forEach(function(feature){
              let feature_name = feature.properties[`${name_key}`];
              // Check whether this is the local_authority dataset
              if (name_key === "lad18nm"){
                // Ensure only Scottish datasets are added by checking code for S prefix
                if(feature.properties.lad18cd[0] === "S"){
                  let geoJSON = L.geoJson(feature).addTo(mymap).bindPopup(`<b>${feature_name}</b>`);
                }
              }
              else {
                let geoJSON = L.geoJson(feature).addTo(mymap).bindPopup(`<b>${feature_name}</b>`);
              }
            });
          }
        });
      });
    });
  };

  window.setupCopyToClipboard = function(){
    $('#iframe_generator_modal').click(function(){
      $('#copy_message').text("");
    });

    $('#copy_to_clipboard').click(function(){
      var iframeSnippet = $('#embedded_code').val();
      if(!!navigator.clipboard){
        navigator.clipboard.writeText(iframeSnippet).then(function() {
          $('#copy_message').text("Copied to clipboard!");
          }, function() {
            $('#copy_message').text("Failed to copy to clipboard.");
          });
        }
      else {
        $('#copy_message').text("Copy not supported in your browser.");
      }
    });
  };

  window.serviceAreaClientValidation = () => {
    var regionalWarning = "To add a national service area remove all regional.";
    var nationalWarning = "To add regional service areas remove all national.";
    var mixedServiceAreaWarning = "Please select either national or regional service areas.";
    var standardMessage = 'If you select Scotland or United Kingdom as a service area, your listing will not appear when a user filters their search to only show local - not national - services.';

    $('#service-area-warning').innerHTML = standardMessage;

    var selectSpanTarget = {};
    $('#id_service_areas').siblings().each((index, item) => {
      if ($(item).is("span")){ selectSpanTarget = item; }
    });

    $(selectSpanTarget).click(function(){
      var nationalOptions = [];
      var regionalOptions = [];
      var optionGroups = $('#id_service_areas').children();

      optionGroups.each((index, group) => {
        var groupArray = [];

        if (group.label == "Country"){
          groupArray = Array.from(group.children);
          $.merge(nationalOptions, groupArray);
        }
        else {
          groupArray = Array.from(group.children);
          $.merge(regionalOptions, groupArray);
        }
      });

      var checkSelected = (option) => {
        if (option.selected){ return option; }
      };

      var nationalSelectedCount = nationalOptions.filter(checkSelected).length;
      var regionalSelectedCount = regionalOptions.filter(checkSelected).length;

      if (nationalSelectedCount == 0 && regionalSelectedCount > 0){
        $('#service-area-warning').text(regionalWarning);
        $('li[aria-label="Country"]').hide();
      }

      if (nationalSelectedCount > 0 && regionalSelectedCount == 0){
        $('#service-area-warning').text(nationalWarning);
        $('li[aria-label="Local Authority"]').hide();
        $('li[aria-label="Health Board"]').hide();
        $('li[aria-label="Integration Authority (HSCP)"]').hide();
      }

      if (nationalSelectedCount > 0 && regionalSelectedCount > 0){
        $('#service-area-warning').text(mixedServiceAreaWarning);
      }

      if (nationalSelectedCount == 0 && regionalSelectedCount == 0){
        $('#service-area-warning').text(standardMessage);
      }
    });

    $(selectSpanTarget).trigger('click');
  };

  window.initSearchAutocomplete = () => {
    var options = {
      url: function(phrase) { return "/api/v4/postcode-locations/?q=" + phrase; },
      getValue: "place_name",
      template: {
        type: "custom",
        method: function(value, item){
          return "<span class=\"icon\">📍</span>" + value;
        }
      },
      minCharNumber: 3,
      list: {
        match: { enabled: true },
        onChooseEvent: function(e){
          var value = $("#postcode").getSelectedItemData().place_name;
          $("#postcode").val(value).trigger("change");
          $('.search-box form').submit();
        },
      }
    };

    $("#postcode").easyAutocomplete(options);

    var triggerAutoCompleteSelection = function() {
      var firstOption = $('#eac-container-postcode li div:visible').first();
      if (firstOption.length){
        $('#eac-container-postcode li div:visible').first().click();
      }
    };

    $('#postcode').keypress(function(e){
      if (e.which === 13) { triggerAutoCompleteSelection(); }
    });

    $('.search-box button').click(function(){
      triggerAutoCompleteSelection();
    });
  };

  svg4everybody();
  handleTabs();
  checkMaxCategories();
  matchHeight();
  storeSearchUrl();
  handleClearInputs();
  toggleClearableInputs();
  handleFileInput();
  observeModals();
});


//  ALISS
//  ================================
//  Habanero + Braw Software
//  http://habanero.digital/
//  http://www.thisisbraw.co.uk/
//

// jQuery
const $ = global.$ = global.jQuery = require('jquery');

// Foundation Includes
require('foundation-sites/dist/js/foundation.min.js');
// require('foundation-sites/js/foundation.abide.js');
// require('foundation-sites/js/foundation.accordion.js');
// require('foundation-sites/js/foundation.accordionMenu.js');
// require('foundation-sites/js/foundation.drilldown.js');
// require('foundation-sites/js/foundation.dropdown.js');
// require('foundation-sites/js/foundation.dropdownMenu.js');
// require('foundation-sites/js/foundation.equalizer.js');
// require('foundation-sites/js/foundation.interchange.js');
// require('foundation-sites/js/foundation.magellan.js');
// require('foundation-sites/js/foundation.offcanvas.js');
// require('foundation-sites/js/foundation.orbit.js');
// require('foundation-sites/js/foundation.responsiveMenu.js');
// require('foundation-sites/js/foundation.responsiveToggle.js');
// require('foundation-sites/js/foundation.reveal.js');
// require('foundation-sites/js/foundation.slider.js');
// require('foundation-sites/js/foundation.sticky.js');
// require('foundation-sites/js/foundation.tabs.js');
// require('foundation-sites/js/foundation.toggler.js');
// require('foundation-sites/js/foundation.tooltip.js');

// Bootstrap Includes
// require('bootstrap-sass/assets/javascripts/bootstrap.min.js');
// require('bootstrap-sass/assets/javascripts/bootstrap-sprockets.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/affix.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/alert.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/button.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/carousel.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/collapse.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/dropdown.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/modal.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/popover.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/scrollspy.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/tab.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/tooltip.js');
// require('bootstrap-sass/assets/javascripts/bootstrap/transition.js');

//Date
require('pickadate/lib/picker.js');
require('pickadate/lib/picker.date.js');

// Foundation
$(document).foundation();

// Select2
require('./partials/select2.min.js');
// require('clipboard/lib/clipboard.js');
// require('svg4everybody/dist/svg4everybody.legacy.js');


// Imports
import matchHeight from './partials/match-height';
import svg4everybody from 'svg4everybody/dist/svg4everybody.js';
import Clipboard from 'clipboard/lib/clipboard.js';
import './partials/extensions';

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
      $('.selected-categories .cats').append(`<div class="selected-cat" data-cat="${value}"><span class="remove"></span>${label}</div>`);
    } else {
      // console.log('unchecked');
      $(`.selected-categories .cats .selected-cat[data-cat='${value}']`).remove();
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
  });

  // Site Wide Toggles
  $('.toggled').each(function(index, el) {
    var $thisToggle = $(this);
    var id = $thisToggle.attr('id');
    $(`#${id}_toggle`).click(function() {
      $(`#${id}`).toggleClass('active');
      $(this).toggleClass('active');
    });
  });

  // Modals
  $('.modal').each(function(index, el) {
    var $thisToggle = $(this);
    var id = $thisToggle.attr('id');
    $(`#${id}_modal, a[data-modal="${id}"]`).click(function(){
      $(`#${id}`).toggleClass('active');
      $('.black').toggleClass('show');
    });
  });

  $('.black').click(function() {
    $(this).removeClass('show');
    $('.modal').removeClass('active');
  });
  $('.modal a.close, .modal a.cancel').click(function() {
    $('.black').removeClass('show');
    $('.modal').removeClass('active');
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

  svg4everybody();

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
      console.log($(target).find('input'));
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

  checkMaxCategories();
  matchHeight();
  storeSearchUrl();
  handleClearInputs();
  toggleClearableInputs();
  handleFileInput();
});

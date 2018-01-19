
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

// Foundation
$(document).foundation();

// Select2
require('./partials/select2.min.js');

// Imports
import matchHeight from './partials/match-height';
// import 'select2/dist/js/select2.min.js';

$(document).ready(() => {
    matchHeight();

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

    $(document).click(function(){
        $('.navigation').removeClass('active');
        $('body').removeClass('restrict-height');
        $("#menu_toggle").removeClass('active');
        $(".category-selector .cells > ul > li").removeClass('active');
    });
    $('.navigation a, .category-selector .cells > ul > li a, .category-selector .cells > ul > li span').click(function(e){
        e.stopPropagation();
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
        $(`#${id}_modal`).click(function() {
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
    });
    $('.location a.more-link').click(function() {
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

    // Toggle Children Categories
    $('.radio-list.children .toggle-children, .checkbox-list.children .toggle-children').click(function() {
        $(this).toggleClass('active');
        var children = $(this).next('ul');
        // console.log(children);
        children.toggleClass('active');
    });

    // Cat Menu
    $(".category-selector .cells > ul > li > a.select-category, .category-selector .cells > ul > li > span.select").click(function(e) {
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
});





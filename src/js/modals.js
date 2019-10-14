const observeModals = () => {
  //Handle modal keyboard trap
  var checkKeyPress = function(modalId){
    // Setup variables for the focusable elments of the modal
    var focusableElements = $('#' + `${modalId}`).find("a, a[role='button'] [tabindex='0'], input, button, textarea").not("input[type='hidden']");
    var focusableLength = focusableElements.length;
    var finalIndex = (focusableLength - 1);
    var firstFocusable = focusableElements[0];
    var lastFocusable = focusableElements[finalIndex];

    if ($(`#${modalId}`).hasClass('active')){
      firstFocusable.focus();
    }

    $('body').keydown(function(e){

      function handleBackwardTab() {
        if (document.activeElement === firstFocusable) {
          e.preventDefault();
          lastFocusable.focus();
        }
      }

      function handleForwardTab() {
        if (document.activeElement === lastFocusable) {
          e.preventDefault();
          firstFocusable.focus();
        }
      }
      // Setup keypress listeners to avoid user tabbing off of modal
      if ($('#' + `${modalId}`).hasClass('active')){

        if (e.key == "Tab" && !e.shiftKey){
          handleForwardTab();
        }
        if (e.key == "Tab" && e.shiftKey) {
          handleBackwardTab();
        }
        if (e.key == "Escape") {
          $('.black').removeClass('show');
          $('.modal').removeClass('active');
          $(`#${modalId}_modal`).focus();
        }
      }
    });
  };

  // Modals
  $('.modal').each(function(index, el) {
    var $thisModal = $(this);
    var id = $thisModal.attr('id');

    // Updating modal attributes
    $(this).attr('role', 'dialog');
    $(this).attr('aria-modal', 'true');

    // Updating modal links
    $('#' + `${id}` + '_modal').attr('role', 'button');
    $('#' + `${id}` + '_modal').attr('tabindex', '0');

    // Adding modal click behaviour
    $(`#${id}_modal, a[data-modal="${id}"], input[data-modal="${id}"]`).click(function(e){
      if ($(this).is(':checkbox') && !e.target.checked){
      } else {
        $(`#${id}`).toggleClass('active');
        $('.black').toggleClass('show');
        checkKeyPress(id);
      }
    // Adding modal on keypress behaviour
    }).keypress(function(e){
      if ($(this).is(':checkbox') && !e.target.checked){
      } else {
        $(`#${id}`).toggleClass('active');
        $('.black').toggleClass('show');
        checkKeyPress(id);
      }
    });
  });

  // Find the active modal and get it's id before closing it and returning focus to the element that launched it.
  function closeModalFocusOnLastClicked(){
    $('.black').removeClass('show');
    let id = '';
    $('.modal').each(function(index, el) {
      if ($(el).hasClass('active')){
        id = $(el).attr('id');
      }
    });
    $('.modal').removeClass('active');
    $(`#${id}_modal`).focus();
  }
  // Allow users to click on black modal surround to close it and shift focus to last selected.
  $('.black').click(function() {
    closeModalFocusOnLastClicked();
  });

  // Add on click listener to cancel and close modal buttons to hide modal and return focus to last element.
  $('.modal a.close, .modal a.cancel').click(function() {
    closeModalFocusOnLastClicked();
  }).keypress(function() {
    closeModalFocusOnLastClicked();
  });
};

export { observeModals as default };
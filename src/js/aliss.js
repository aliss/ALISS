const ALISS = function() {
  this.config = null;
  var context = this;

  ALISS.prototype.init = function() {
    context.config = {
      categoryTarget: '#aliss-category-selector-div',
      categoryChanged: function(event){ console.log("Replace with a callback fn."); }
    };
  };

  ALISS.prototype.renderCategoryDropdown = function() {
    $.ajax({
      url: "https://www.aliss.org/api/v4/categories/",
      data: {},
      success: function(response){
        $(context.config.categoryTarget).empty();
        context.renderCategoryOptions(response);
      }
    });
  };

  ALISS.prototype.renderCategoryOptions = function(response) {
    var dropdownDiv = document.createElement('div');
    dropdownDiv.id = "aliss-dropdown-div";
    dropdownDiv.className = "aliss-category-dropdown-div";
    $(context.config.categoryTarget).append(dropdownDiv);
    var dropdownSelect = document.createElement('select');
    dropdownSelect.id = "aliss-category-dropdown";
    dropdownSelect.className = "aliss-dropdown";

    $('#aliss-dropdown-div').append(dropdownSelect);
    $('#aliss-category-dropdown').append("<option value=categories>Categories</option>");

    $.each(response.data, function(index, category){
      var option = document.createElement("option");
      option.textContent = category.name;
      option.id = category.slug;
      option.className = "aliss-category-option";
      option.value = category.slug;
      $('#aliss-category-dropdown').append(option);
      $('#' + option.id).data(category);
    });

    $('#aliss-category-dropdown').change(context.handleFilterByCategory);
  };

  ALISS.prototype.handleFilterByCategory = (event) => {
    $(event.target).siblings().remove();

    if (context.config.categoryChanged){
      context.config.categoryChanged(event);
    }

    if (event.target.value == "categories"){
      context.renderCategoryDropdown();
      return;
    }

    var categoryObject;
    var parent = $(event.target).parent();
    if (event.target.value == "sub-categories"){
      var parentID = $(event.target).parent()[0].id;
      var sortedID = parentID.replace('-select', "");
      categoryObject = $('#' + sortedID).data();
      parent.children().remove();
      context.renderSubCategoryDropdown(categoryObject, parent);
      return;
    }

    categoryObject = $('#' + event.target.value).data();
    context.renderSubCategoryDropdown(categoryObject, parent);
  };

  ALISS.prototype.renderSubCategoryDropdown = function (categoryObject, target) {
    if (categoryObject.sub_categories.length === 0){ return; }

    var subDiv = document.createElement('div');
    subDiv.id = categoryObject.slug + '-select';

    var subCategoryDropDown = document.createElement('select');
    subCategoryDropDown.className = "aliss-sub-category-dropdown";

    var blankOption = document.createElement('option');
    blankOption.textContent = "Sub-Category";
    blankOption.value = "sub-categories";
    subCategoryDropDown.appendChild(blankOption);

    $(target).append(subDiv);
    $(subDiv).append(subCategoryDropDown);

    $.each(categoryObject.sub_categories, function(index, item){
      var subCategoryOption = document.createElement('option');
      subCategoryOption.textContent = item.name;
      subCategoryOption.id = item.slug;
      subCategoryOption.className = "aliss-sub-category-option";
      subCategoryOption.value = item.slug;
      subCategoryDropDown.appendChild(subCategoryOption);
      $('#' + subCategoryOption.id).data(item);
    });

    $(subCategoryDropDown).change(context.handleFilterByCategory);
  };

  context.init();
};

module.exports = ALISS;
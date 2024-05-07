angular.module("umbraco").controller("muyaFaIconPicker.Controller", function ($scope, muyaFaIconResources, editorService) {

    $scope.modelIsValid = false;
    muyaFaIconResources.loadFaIconsCss();

    if ($scope.model.value !== "") {
        $scope.modelIsValid = true;
    }

    $scope.openDialog = function () {
        var options = {
            view: '/App_Plugins/FaIconPicker/views/faIconPicker.dialog.html',
            title: 'Select an icon - Font awesome v5.13.0',
            size: 'small',
            currentIcon: $scope.model.value,
            pickIcon: function (icon) {
                $scope.model.value = icon;
                if (icon !== "") {
                    $scope.modelIsValid = true;
                }
                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };
        editorService.open(options);
    };
    $scope.remove = function () {
        $scope.model.value = '';
        $scope.modelIsValid = false;
    };
});

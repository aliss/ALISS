angular.module("umbraco").controller("muyaFaIconPicker.Dialog.Controller", function ($scope, $timeout, muyaFaIconResources) {
    $scope.loading = true;
    $timeout(function () {
        //will run on the next digest.
        $scope.model.icons = muyaFaIconResources.getIcons();
        $scope.loading = false;
    }, 1);     
});

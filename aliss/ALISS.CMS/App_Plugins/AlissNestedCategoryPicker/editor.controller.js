angular.module("umbraco").config(function($sceDelegateProvider) {
    $sceDelegateProvider.resourceUrlWhitelist(['**']);
});
angular.module("umbraco").controller("alissNestedCategoryPicker.editorController", function($scope, $http, notificationsService) {
	var url = "/umbraco/api/alissnestedcategorypicker/getcategories";

	$http.get(url).then(function (response) {
		$scope.model.primaryCategories = JSON.parse(JSON.stringify(response.data.data));
	});
	
	$scope.selectPrimaryCategory = function (category) {
		if (category && category != '') {
			$scope.add(category);
			$scope.model.selectedPrimaryCategory = JSON.parse(category);
		}
		else {
			$scope.model.selectedPrimaryCategory = [];
		}
	};

	$scope.selectSecondaryCategory = function (category) {
		if (category && category != '') {
			$scope.add(category);
			$scope.model.selectedSecondaryCategory = JSON.parse(category);
		}
		else {
			$scope.model.selectedSecondaryCategory = [];
			$scope.add($scope.model.primaryCategory);
		}
	};

	$scope.selectTertiaryCategory = function (category) {
		if (category && category != '') {
			$scope.add(category);
			$scope.model.selectedTertiaryCategory = JSON.parse(category);
		}
		else {
			$scope.model.selectedTertiaryCategory = [];
			if ($scope.model.secondaryCategory && $scope.model.secondaryCategory != '') {
				$scope.add($scope.model.secondaryCategory);
			}
		}
	};

	if (angular.isArray($scope.model.value) === false) {
		$scope.model.value = [];
	}

	$scope.add = function (category) {
		$scope.model.value = [];
		if (parseInt($scope.model.config.maxItems) > $scope.model.value.length) {
			$scope.model.value.push(JSON.parse(category));
		}
		else {
			notificationsService.remove(0);
			notificationsService.warning("Too many items!", "You may only select " + $scope.model.config.maxItems + " items.");
		}
	};

	$scope.remove = function (index) {
		$scope.model.value.splice(index, 1);
	};
});
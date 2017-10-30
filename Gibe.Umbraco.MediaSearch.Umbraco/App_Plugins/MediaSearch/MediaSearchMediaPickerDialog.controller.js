function mediaSearchMediaPickerDialogController($scope, umbRequestHelper, $log, $http, mediaResource) {

	$scope.loading = false;

	$scope.term = '';
	$scope.search = function (term) {

		if (term.length > 0 && term.length < 3) return;
		
		$scope.loading = true;
		
		$http.post('/Umbraco/Api/MediaSearchApi/Search/?term=' + term).then(function (response) {
			mediaResource.getByIds(response.data.MediaIds).then(function (media) {
				$scope.media = media;
				$scope.loading = false;
			});
		});

	}

	$scope.search($scope.term);
}

angular.module("umbraco").controller("Gibe.Umbraco.MediaSearch.MediaPickerDialogController", mediaSearchMediaPickerDialogController);
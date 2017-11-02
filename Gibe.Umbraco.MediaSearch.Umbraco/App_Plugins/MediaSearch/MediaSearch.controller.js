function mediaSearchController($scope, $log, $http, $location, umbRequestHelper, mediaResource) {

	$scope.loading = false;

	$scope.term = '';
	$scope.search = function (term) {

		if (term.length > 0 && term.length < 3) return;
		
		$scope.loading = true;
		
		$http.post('/Umbraco/Api/MediaSearchApi/Search/?term=' + term, $scope.facets).then(function (response) {
			mediaResource.getByIds(response.data.MediaIds).then(function (media) {
				$scope.media = media;
				$scope.loading = false;
			});
			if (!$scope.facets) {
				$scope.facets = response.data.Facets;
			}
			else {
				updateFacets(response.data.Facets);
			}
		});
	}
	
	$scope.search($scope.term);

	$scope.checkIfAnySelected = function () {
		if (!$scope.facets) return false;
		for (var i = 0; i < $scope.facets.length; i++) {
			for (var j = 0; j < $scope.facets[i].Values.length; j++) {
				if ($scope.facets[i].Values[j].Selected) return true;				
			}
		}
		return false;
	};

	$scope.click = function (item, $event, $index) {
		item.selected = true;
	};

	$scope.clickItemName = function (item, $event, $index) {
		console.log(item);
		$location.path("/media/media/edit/" + item.id);
	};

	function updateFacets(facets) {
		var atTop = false;
		for (var i = 0; i < $scope.facets.length; i++) {
			for (var j = 0; j < $scope.facets[i].Values.length; j++) {
				$scope.facets[i].Values[j].Count = getNewCount(facets, $scope.facets[i].Key, $scope.facets[i].Values[j].Value);
				if (!atTop) {
					var grid = document.getElementById('gibe-ai-content-grid');
					grid.scrollTop = 0;
					atTop = true;
				}
			}
		}
	}

	function getNewCount(facets, key, value) {
		for (var i = 0; i < facets.length; i++) {
			for (var j = 0; j < facets[i].Values.length; j++) {
				if (facets[i].Key === key && facets[i].Values[j].Value === value) {
					return facets[i].Values[j].Count;
				}
			}
		}
		return 0;
	}

	
}

angular.module("umbraco").controller("Gibe.Umbraco.Dashboard.MediaSearchController", mediaSearchController);
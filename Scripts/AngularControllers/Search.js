app.controller('Search', function ($scope, $element, $window, $timeout, $http) {

    $scope.makeQuery = function () {
        var query = localStorage.getItem("query");
        $scope.query = query;
        $scope.user = localStorage.getItem("loggedIn");
        if ($scope.user.length < 5) {
            window.location.href = "http://" + baseUrl + "/Login/Login";
        }
        $http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/SearchData/retrievePids',
            data: {
                "query": query
            },
        }).then(function successCallback(response) {
            $scope.comments = response.data[0];
            $scope.tags = response.data[1];
            $scope.photos = response.data[2];
            if ($scope.comments.length == 0) {;
                $scope.commentError = "There were no comments that matched your search";
            }
            if ($scope.tags.length == 0) {
                $scope.tagError = "There were no tags that matched your search";
            }
            if ($scope.photos.length == 0) {
                $scope.photoError = "There were no photos that matched your search";
            }
        }, function errorCallback(response) {
        });
    }
});
app.controller('Home', function ($scope, $element, $window, $timeout, $http) {
    "use strict";
    $scope.getUser = function () {
        $scope.user = localStorage.getItem("loggedIn");
        if ($scope.user == undefined || $scope.user == null) {
            $scope.loggedIn = false;
        }
        else {
            $scope.loggedIn = true;
        }

        $http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/FriendsData/getFriends',
            data: {
                "Username": $scope.user
            },
        }).then(function successCallback(response) {
            $scope.friends = response.data;
        }, function errorCallback(response) {
        });
    
        $http({
            method: 'GET',
            url: "http://" + baseUrl + '/api/HomeData/getBestUser',
        }).then(function successCallback(response) {
            $scope.bestUsers = response.data;
        }, function errorCallback(response) {
        });
        $http({
            method: 'GET',
            url: "http://" + baseUrl + '/api/HomeData/popTags',
        }).then(function successCallback(response) {
            $scope.bestTags = response.data;
        }, function errorCallback(response) {
        });



    }

    $scope.getThePhotos = function (tag) {
        $http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/HomeData/getTagPhotos',
            data: {
                "tagName": tag
            },
        }).then(function successCallback(response) {
            debugger;

            $scope.PIDS = response.data;
            setTimeout(function () {
                debugger;
                document.getElementById(tag).style = 'display: block !important;';
                console.log($scope.PIDS)
            }, 500);


            }, function errorCallback(response) {
        });
    }


});
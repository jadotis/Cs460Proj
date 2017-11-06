app.controller('Friends', function ($scope, $element, $window, $timeout, $http) {
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
            if (response.data.length == 0) {
                $scope.noFriends = "Add more friends in order to get recomendations";
            }
            else {
                $scope.noFriends = "";
            }
        }, function errorCallback(response) {
            });

        $http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/FriendsData/getReccomendations',
            data: {
                "Username": $scope.user
            },
        }).then(function successCallback(response) {
            $scope.reccomendations = response.data;
        }, function errorCallback(response) {
            });

        $http({
            method: 'GET',
            url: "http://" + baseUrl + '/api/FriendsData/photoRecs'
        }).then(function successCallback(response) {
            $scope.photoRecs = response.data;
        }, function errorCallback(response) {
        });

    }

    $scope.search = function () {
        if ($scope.query == null || $scope.query == undefined ) {
            $scope.errorText = "Please Enter a query to search";
            return;
        }
        else if ($scope.user == "null") {
            $scope.errorText = "You must be logged in to add friends";
            return;
        }
        else {
            $scope.errorText = "";
            $http({
                method: 'POST',
                url: "http://" + baseUrl + '/api/FriendsData/search',
                data: {
                    "query": $scope.query
                },
            }).then(function successCallback(response) {
                var friendsData = response.data;
                $scope.friendsSearch = [];
                for (var i in friendsData) {
                    var myArray = friendsData[i].split(":")
                    var temp = new Object();
                    for (var j in myArray) {
                        temp["firstname"] = myArray[0];
                        temp["lastname"] = myArray[1];
                        temp["email"] = myArray[2];
                    }
                    if (temp["email"] == $scope.user) {
                        continue;
                    }
                    else {
                        $scope.friendsSearch.push(temp);
                    }
                }
            }, function errorCallback(response) {
            });
        }
    }

    $scope.addFriend = function (friendEmail) {
        $http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/FriendsData/addFriend',
            data: {
                "user": $scope.user,
                "friend": friendEmail
            },
        }).then(function successCallback(response) {
            $scope.results = "successfully added friend";
            $timeout(function () {
                $window.location.reload();
            }, 1000);
        }, function errorCallback(response) {
        });
    }

    $scope.removeFriend = function (friendEmail) {
        $http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/FriendsData/removeFriend',
            data: {
                "user": $scope.user,
                "friend": friendEmail
            },
        }).then(function successCallback(response) {
            $scope.results = "successfully removed friend";
            $timeout(function () {
                $window.location.reload();
            }, 1000);
        }, function errorCallback(response) {
        });
    }

});

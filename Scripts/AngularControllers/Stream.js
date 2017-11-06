app.controller('Stream', function ($scope, $element, $window, $timeout, $http) {
    var myAlbums;
    $scope.albums = "";
    $scope.getUser = function () {
        $scope.user = localStorage.getItem("loggedIn");
        if ($scope.user == "null" || $scope.user == undefined) {
            $scope.showVar = false;
        }
        else {
            $scope.showVar = true;
        }


        ($http({
            method: 'GET',
            url: "http://" + baseUrl + '/api/Stream/returnData',
        }).then(function (success) {
            console.log(success);
            $scope.data = success.data;
            for (var i in $scope.data) {
                $scope.data[i]["comments"] = $scope.data[i]["comments"].split(";");
                for (var j in $scope.data[i]["comments"]) {
                    if ($scope.data[i]["comments"][j] == "") {
                        delete $scope.data[i]["comments"][j];
                    }
                }
            }
            }, function (error) {
                console.log(error.status);
        }));

    }

    $scope.addLike = function (PID) {
        ($http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/Stream/addLike',
            data: {
                "user": $scope.user,
                "photo": PID
            }
        }).then(function (success) {
            location.reload();
        }, function (error) {
            console.log(error.status);
        }));

    }

    $scope.makeComment = function (PID) {
        var myVariable = document.getElementById(PID).value;
        if (myVariable == null || myVariable == undefined) {
            return;
        }
        ($http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/Stream/addComment',
            data: {
                "user": $scope.user,
                "photo": PID,
                "text": myVariable
            }
        }).then(function (success) {
            location.reload();
        }, function (error) {
            console.log(error.status);
        }));
    }
});
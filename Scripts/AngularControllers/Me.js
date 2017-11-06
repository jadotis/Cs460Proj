

app.controller('Me', function ($scope, $element, $window, $timeout, $http) {
    var myAlbums;
    $scope.albums = "";
    $scope.getUser = function () {
        $scope.user = localStorage.getItem("loggedIn");
        if ($scope.user.length < 5) {
            window.location.href = "http://" + baseUrl + "/Login/Login";
        }

        $http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/AlbumPhoto/getAlbums',
            data: {
                "username": $scope.user
            },
        }).then(function successCallback(response) {
            myAlbums = response.data;
            $scope.albums = parse(myAlbums);
            var albumNames = {
                names: [],
                show: []
            }
            for (var album in $scope.albums) {
                albumNames.names.push($scope.albums[album]);
                albumNames.show.push($scope.albums[album]);
            }
            $scope.albumNames = albumNames;
            console.log($scope.albumNames);
        }, function errorCallback(response) {
        });
    }


    function parse(albums) { //removes the extra stuff from the string array.
        var returns = [];
        for (var i = 0; i < albums.length; i++) {
            returns[i] = albums[i].replace(new RegExp("[0-9]", "g"), "").replace(":", "");
        }
        return returns;
    }

    $scope.deleteAlbum = function () {
        if ($scope.toDelete == null || $scope.toDelete == undefined) {
            $scope.deleteError = "Please Select an Album to Delete...";
        }
        $http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/AlbumPhoto/deleteAlbum',
            data: {
                "album": $scope.toDelete,
                "user": $scope.user
            },
            contentType: 'text',
        }).then(function successCallback(response) {
            $scope.deleteError = "";
            $window.location.reload();
        }, function errorCallback(response) {
            $scope.deleteError = "There was an error deleting that Album..."
        });

    }



    $scope.sendPhoto = function () {
        if ($scope.albumChoice == null || $scope.albumChoice == undefined) {
            $scope.photoSendingError = "Please select an Album Type";
            return;
        }
        var photo = document.querySelector('input[type=file]').files[0];
        var reader = new FileReader();
        reader.onloadend = function (e) {
            var text = reader.result;
            var convertedImage = Array.from(new Uint8Array(text));
            $http({
                method: 'POST',
                url: "http://" + baseUrl + '/api/AlbumPhoto/acceptPhoto',
                data: {
                    "photo": convertedImage,
                    "caption": $scope.description,
                    "type": photo.type,
                    "size": photo.size,
                    "albumName": $scope.albumChoice,
                    "user": $scope.user, 
                    "tags": $scope.tags
                },
                contentType: 'text',
            }).then(function successCallback(response) {
                $scope.photoSendingError = "";
                $window.location.reload();
            }, function errorCallback(response) {
            });
        }
        try {
            reader.readAsArrayBuffer(photo);

        } catch (error) {
            $scope.photoSendingError = "Please Select a photo to upload";
        }
    }

    $scope.showPhotos = function (albumSelection) {
        ($http({
            method: 'GET',
            url: "http://" + baseUrl + '/api/AlbumPhoto/getPhotosPerAlbum',
            headers: {
                'album': albumSelection,
                'user': $scope.user
            }
        }).then(function (success) {
            $scope.AIDs = [];
            var j = 0;
            var length = success.data.length;
            while (j != length) {
                var temp = new Object();
                temp["name"] = success.data[j];
                temp["caption"] = success.data[j+1];
                $scope.AIDs.push(temp);
                j+=2;
            }
            $scope.photosPerAlbum = success.data;
            $scope.albumID = albumSelection;
            return;
        }, function (error) {
            switch (error.status) {
                case 400: $scope.error = "You already have an album under that name!";
                default: "You caused a malformed query";

            }
        }));
    }

    $scope.update = function (pic) {
        $scope.toDelete = pic;
        return;
    }

    $scope.deletePhoto = function () {
        if ($scope.toDelete == null || $scope.toDelete == undefined) {
            $scope.deletionError = "Please Enter a value to delete";
            return;
        }
        $scope.deletionError = '';
        ($http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/AlbumPhoto/DeletePhoto',
            data: {
                "PhotoID": $scope.toDelete
            }
        }).then(function (success) {
            $window.location.reload();
        }, function (error) {
        }));

    }


    $scope.showLogin = function () {
        $scope.showAlbum = true;
        return;
    }


    $scope.makeAlbum = function () {
        if ($scope.albumName == "" || $scope.albumName == null || $scope.albumName == undefined) {
            $scope.error = "Please enter an Album name";
            return;
        }

        ($http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/AlbumPhoto/CreateAlbum',
            data: {
                "albumName": $scope.albumName,
                "user": $scope.user
            }
        }).then(function (success) {
            $scope.showAlbum = false;
            $scope.error = "";
            $window.location.reload();
        }, function (error) {
            switch (error.status) {
                case 400: $scope.error = "You already have an album under that name!";
                default: "You caused a malformed query";

            }
        }));
    }



});
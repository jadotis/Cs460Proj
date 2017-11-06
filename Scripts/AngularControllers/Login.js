function logout() {
    localStorage.setItem("loggedIn", null);
    return;
}


window.onload = function () {
    var loggedInPosition = "http://" + baseUrl + "/Login/Login";
    var currentPage = document.location.href;
    var loggedIn = localStorage.getItem("loggedIn");
    if (((currentPage == loggedInPosition) && loggedIn).length > 5) {
        window.location.href = "http://" + baseUrl;
        //redirect back to mainpage as we have a string(null) which has a length of 4
        console.log(loggedIn);
        console.log(((currentPage == loggedInPosition) && loggedIn).length > 5);
    }
    else if (currentPage != loggedInPosition) {
        return; //Other Pages
    }
    //Enter the angular controller.
};

app.controller('Login', function ($scope, $element, $window, $timeout, $http) {
    "use strict";


    $scope.validate = function () {
        $scope.register = {
            "firstName": $scope.firstName,
            "lastName": $scope.lastName,
            "email": $scope.usermail,
            "dob": $scope.dob,
            "hometown": $scope.hometown,
            "gender": $scope.gender,
            "password": $scope.password
        }
        if ($scope.usermail.indexOf("@") <= 0 || $scope.usermail.indexOf(".") <= 0) {
            $scope.error = "Please enter a valid email";
            return;
        }
        else {
            $scope.error = "";
            ($http({
                method: 'POST',
                url: "http://" + baseUrl + '/api/UserName/checkName',
                data: {
                    "firstName": $scope.firstName,
                    "lastName": $scope.lastName,
                    "email": $scope.usermail,
                    "dob": $scope.dob,
                    "hometown": $scope.hometown,
                    "gender": $scope.gender,
                    "password": $scope.password
                }
            }).then(function (success) {
                $scope.error = "";
                $scope.success = "Welcome " + $scope.usermail;
                setTimeout(function () {
                    localStorage.setItem("loggedIn", $scope.usermail);
                    window.location.href = baseUrl;
                }, 2000);
            }, function (error) {
                if (error.status == 400) {
                    $scope.error = "Sorry that email is taken.";
                }
                else {
                    $scope.error = "You entered a malformed query";
                }
            }));
        }

    }

    $scope.login = function () {
        var loginBlock = {
            "loginName": $scope.loginUsername,
            "loginPassword": $scope.loginPassword
        }
        if ($scope.loginPassword.length < 1 || $scope.loginUsername < 1) {
            $scope.loginError = "Please enter a valid login....";
        }
        ($http({
            method: 'POST',
            url: "http://" + baseUrl + '/api/UserName/login',
            data: {
                "username": loginBlock.loginName,
                "password": loginBlock.loginPassword
            }
        }).then(function (success) {
            $scope.loginError = "";
            $scope.loginSuccess = "Welcome back " + loginBlock.loginName;
            setTimeout(function () {
                localStorage.setItem("loggedIn", loginBlock.loginName);
                window.location.href = "http://" + baseUrl;
            }, 2000);

        }, function (error) {
            $scope.loginError = "Invalid Username or Password";
            return;
        }));


    }

});
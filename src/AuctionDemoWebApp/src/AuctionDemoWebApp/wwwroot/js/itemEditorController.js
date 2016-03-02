(function () {
    "use strict";
    angular.module("app-items").controller("itemEditorController", itemEditorController);

    function itemEditorController($routeParams, $http, $scope) {
        var vm = this;

        vm.errorMessage = "";
        vm.isBusy = true;

        
        vm.itemName = $routeParams.itemName;
        vm.bids = [];

        vm.newBid = {};

        var url = "/api/items/" + vm.itemName + "/bids";

        vm.addBid = function () {
            vm.isBusy = true;

            $http.post(url, vm.newBid)
                .then(function(response) {
                    vm.bids.push(response.data);
                    vm.newBid = {};
                }, function (error) {
                    vm.errorMessage = "Unable to add new details. " + error;

                })
                .finally(function() {
                    vm.isBusy = false;
                });

        }

        $http.get(url)
            .then(function(response) {
                angular.copy(response.data, vm.bids);
                

            }, function() {
                vm.errorMessage = "Failed to load details";
            })
            .finally(function() {
                vm.isBusy = false;
            });

       
        var chat = $.connection.communicationHub;
        //$.connection.hub.logging = true;

        //chat.client.broadcastMessage = function (name, message) {
        //    console.log("Communication: " + name + "-" + message);
        //};

        chat.client.priceChanged = function (updItemName, newPrice) {
            if (updItemName == vm.itemName) {
                vm.currentPrice = newPrice;
                $scope.$apply();
            }
        }

        $.connection.hub.start().done(function () {
        });

    }

    

})();       
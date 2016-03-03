(function () {
    "use strict";
    angular.module("app-items").controller("itemEditorController", itemEditorController);

    function itemEditorController($routeParams, $http, $scope, notificationService) {
        var vm = this;

        vm.errorMessage = "";
        vm.isBusy = true;


        vm.itemName = $routeParams.itemName;
        vm.bids = [];

        vm.newBid = {};
        vm.currentPrice = 0;

        var url = "/api/items/" + vm.itemName + "/bids";

        vm.addBid = function () {
            vm.isBusy = true;
            vm.errorMessage = "";

            vm.newBid.price = vm.currentPrice;

            $http.post(url, vm.newBid)
                .then(function (response) {
                    vm.bids.splice(0, 0, response.data);
                    vm.newBid = {};

                    updateCurrentItem();


                }, function (error) {
                    vm.errorMessage = "Unable to add new details. " + JSON.stringify(error);;

                })
                .finally(function () {
                    vm.isBusy = false;
                });

        }

        function getBids() {
            $http.get(url)
            .then(function (response) {
                angular.copy(response.data, vm.bids);
                vm.currentPrice = 0;

                updateCurrentItem();

            }, function (error) {
                vm.errorMessage = "Failed to load details. " +  JSON.stringify(error);
            })
            .finally(function () {
                vm.isBusy = false;
            });

        }

        getBids();

        
        vm.refresh = function() {
            getBids();
        }

        function updateCurrentItem() {
            $http.get("/api/items/" + $routeParams.itemName)
                .then(function (response) {
                    vm.currentItem = response.data;
                    vm.currentPrice = response.data.currentPrice;
                }, function (error) {
                    vm.errorMessage = "Error while getting item status." + JSON.stringify(error);;
                });
        }

        notificationService.chat.client.priceChanged = function (updItemName, newPrice) {
            console.log("priceChanged event...");
            //if (updItemName == vm.itemName) {
            //    vm.currentPrice = newPrice;
            //    $scope.$apply();

            //    $http.get(url)
            //        .then(function (response) {
            //            angular.copy(response.data, vm.bids);
            //        }, function (error) {
            //            vm.errorMessage = "Error while getting item status.";
            //        });

            //}
        }
    }



})();
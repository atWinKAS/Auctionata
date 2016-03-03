﻿(function () {
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
                    //vm.bids.push(response.data);
                    vm.newBid = {};

                    updateCurrentItem();


                }, function (error) {
                    vm.errorMessage = "Unable to add new details. " + error;

                })
                .finally(function () {
                    vm.isBusy = false;
                });

        }

        $http.get(url)
            .then(function (response) {
                angular.copy(response.data, vm.bids);
                vm.currentPrice = 0;

                updateCurrentItem();

            }, function () {
                vm.errorMessage = "Failed to load details";
            })
            .finally(function () {
                vm.isBusy = false;
            });

        function updateCurrentItem() {
            $http.get("/api/items/" + $routeParams.itemName)
                .then(function (response) {
                    vm.currentItem = response.data;
                    vm.currentPrice = response.data.currentPrice;
                }, function (error) {
                    vm.errorMessage = "Error while getting item status.";
                });
        }

        notificationService.chat.client.priceChanged = function (updItemName, newPrice) {
            console.log("priceChanged event handled.");
            if (updItemName == vm.itemName) {
                vm.currentPrice = newPrice;
                $scope.$apply();

                $http.get(url)
                    .then(function (response) {
                        angular.copy(response.data, vm.bids);
                    }, function (error) {
                        vm.errorMessage = "Error while getting item status.";
                    });

            }
        }
    }



})();
(function () {
    "use strict";
    angular.module("app-items").controller("itemEditorController", itemEditorController);

    function itemEditorController($routeParams, $http, $scope, notificationService) {
        var vm = this;

        vm.errorMessage = "";
        vm.infoMessage = "";
        vm.serverMessage = "";
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
            vm.isBusy = true;
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
            vm.errorMessage = "";
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

        notificationService.chat.client.priceChanged = function (userName, itemName, newPrice) {
            vm.serverMessage = "priceChanged event... Item: " + itemName + ", Price: " + newPrice + " (by " + userName + ")";
            $scope.$apply();
        }

        vm.tryBid = function () {
            vm.infoMessage = "";
            var b = {
                ItemName: vm.itemName,
                ClientPrice: vm.currentPrice
            };
            //console.log("tryig to bid item: " + JSON.stringify(b));

            $http.post("/api/items/" + vm.itemName + "/try", b)
                .then(function (response) {

                    //console.log(response.data);

                    vm.infoMessage = "Your bid response: " + JSON.stringify(response.data);


                }, function (error) {
                    vm.errorMessage = "Unable to try to bid on the item. " + JSON.stringify(error);;

                })
                .finally(function () {
                    vm.isBusy = false;
                });
        }
        
        vm.clear = function() {
            vm.isBusy = true;
            $http.delete(url)
            .then(function (response) {
                vm.bids = [];
                vm.currentPrice = 0;

                updateCurrentItem();

            }, function (error) {
                vm.errorMessage = "Failed to delete all bids. " +  JSON.stringify(error);
            })
            .finally(function () {
                vm.isBusy = false;
            });
        }
    }



})();
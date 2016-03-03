(function () {
    "use strict";
    angular.module("app-items").controller("itemsController", itemsController);

    function itemsController($http, notificationService) {
        var vm = this;

        vm.errorMessage = "";
        vm.isBusy = true;

        vm.items = [];

        vm.newItem = {};

        vm.addItem = function() {
            vm.items.push({
                name: vm.newItem.name,
                created: new Date()
            });
            vm.newItem = {};
        }

        vm.addItem = function() {
            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post("/api/items", vm.newItem)
                .then(function(response) {
                    vm.items.push(response.data);
                    vm.newItem = {};
                }, function(error) {
                    vm.errorMessage = "Unable to create new item " + JSON.stringify(error);;
                })
                .finally(function() {
                    vm.isBusy = false;
                });
        }


        $http.get("/api/items")
            .then(function(response) {
                angular.copy(response.data, vm.items);
            }, function(error) {
                vm.errorMessage = "Unable to get items from server." + JSON.stringify(error);;
            })
        .finally(function () {
                vm.isBusy = false;
            });

    }

})();       
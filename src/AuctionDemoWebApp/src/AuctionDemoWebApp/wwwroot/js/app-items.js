(function () {
    "use strict";
    angular.module("app-items", ["simpleControls", "ngRoute"])
    .config(function($routeProvider) {

            $routeProvider.when("/", {
                controller: "itemsController",
                controllerAs: "vm",
                templateUrl: "/views/itemsView.html"
            });

            $routeProvider.when("/editor/:itemName", {
                controller: "itemEditorController",
                controllerAs: "vm",
                templateUrl: "/views/itemEditorView.html"
            });

            $routeProvider.otherwise({
                redirectTo: "/"
            });


        });
})();
(function () {
    "use strict";

    angular.module("app-items")
        .factory("notificationService", notificationService);

    function notificationService() {
        var factory = {};
        var chat = $.connection.communicationHub;
        //$.connection.hub.logging = true;
        
        chat.client.priceChangedStub = function () { };

        $.connection.hub.start().done(function (e) {
            console.log("Communication server started!");
            chat.server.send("JS hub start done.");
        });

        factory.chat = chat;

        return factory;
    }

})();
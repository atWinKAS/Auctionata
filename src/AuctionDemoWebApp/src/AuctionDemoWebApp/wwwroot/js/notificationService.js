(function () {
    "use strict";

    angular.module("app-items")
        .factory("notificationService", notificationService);

    function notificationService() {
        var factory = {};
        var chat = $.connection.communicationHub;
        //$.connection.hub.logging = true;
        
        chat.client.priceChanged = function () { };

        $.connection.hub.start().done(function (e) {
            chat.server.send("info", "connected");
        });

        factory.chat = chat;

        return factory;
    }

})();
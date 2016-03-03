(function () {
    "use strict";

    angular.module("app-items")
        .factory("notificationService", notificationService);

    function notificationService() {
        var factory = {};
        var chat = $.connection.communicationHub;
        $.connection.hub.logging = true;

        chat.client.foo = function () { };

        $.connection.hub.start().done(function (e) {
            chat.server.send("info", "client connected to hub id: " + $.connection.hub.id);
        });

        factory.chat = chat;

        return factory;
    }

})();
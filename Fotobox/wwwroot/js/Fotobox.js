"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/fotoboxHub").build();


connection.on("ReceiveMessage", function (parameter) {
    document.getElementById("text").innerHTML = parameter;
});
"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/fotoboxHub").build();


connection.on("ReceiveMessage", function (parameter) {
    console.log(parameter);
    document.getElementById("text").innerHTML = parameter;
});

connection.start().then(function () {
    console.log("connected");
});
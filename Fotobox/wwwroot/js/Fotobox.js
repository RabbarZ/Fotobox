"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44391/fotoboxHub").build();


connection.on("ReceiveMessage", function (parameter) {
    console.log(parameter);
    document.getElementById("text").innerHTML = parameter;
});

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.log(err.toString());
});

//connection.start().then(function () {
//    console.log("connected");
//});
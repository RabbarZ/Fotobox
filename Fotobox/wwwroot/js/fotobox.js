"use strict";

document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder().withUrl("/fotoboxHub").build();

    const fotoboxContainer = document.querySelector(".fotobox-container");
    const textElement = document.querySelector("#text");

    connection.on("showText", function (text) {
        textElement.innerHTML = text;
    });

    connection.on("reloadPicture", function (path) {
        fotoboxContainer.style.backgroundImage = "url(" + path + ")";

        textElement.innerHTML = "Foto speichern oder löschen...";
    });

    connection.on("reset", function (text) {
        textElement.innerHTML = text;
    });

    connection.start().then(function () {
        console.log("connected");
    }).catch(function (err) {
        return console.log(err.toString());
    });
})
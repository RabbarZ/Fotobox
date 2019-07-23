"use strict";

document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder().withUrl("/fotoboxHub").build();

    const pictureElement = document.querySelector("#picture");
    const textElement = document.querySelector("#text");

    connection.on("showText", function (text) {
        textElement.style.opacity = 1;
        pictureElement.style.opacity = 0;

        textElement.innerHTML = text;
        pictureElement.src = "";
    });

    connection.on("reloadPicture", function (path) {
        textElement.style.opacity = 1;
        pictureElement.style.opacity = 1;

        pictureElement.src = path;
        textElement.innerHTML = "Foto speichern oder löschen...";
    });

    connection.on("reset", function (text) {
        textElement.style.opacity = 1;
        pictureElement.style.opacity = 0;

        textElement.innerHTML = text;
        pictureElement.src = "";
    });

    connection.start().then(function () {
        console.log("connected");
    }).catch(function (err) {
        return console.log(err.toString());
    });
})
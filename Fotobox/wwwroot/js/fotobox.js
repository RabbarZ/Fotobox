"use strict";

document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder().withUrl("/fotoboxHub").build();

    const pictureElement = document.querySelector("#picture");
    // const countdownElement = document.querySelector("#countdown");
    const textElement = document.querySelector("#text");

    connection.on("showText", function (text) {
        textElement.style.opacity = 1;
        pictureElement.style.opacity = 0;
        // countdownElement.style.opacity = 0;

        textElement.innerHTML = text;
        pictureElement.src = "";
        //if (number === 0) {
        //    countdownElement.innerHTML = "Foto!!";
        //}
        //else if (number === -1) {
        //    countdownElement.innerHTML = "";
        //}
        //else {
        //    countdownElement.innerHTML = number;
        //}
    });

    connection.on("reloadPicture", function (path) {
        textElement.style.opacity = 1;
        pictureElement.style.opacity = 1;
        // countdownElement.style.opacity = 0;

        pictureElement.src = path;
        textElement.value = "Foto Speichern oder löschen.";
    });

    connection.on("reset", function (text) {
        textElement.style.opacity = 1;
        pictureElement.style.opacity = 0;
        // countdownElement.style.opacity = 0;

        textElement.innerHTML = text;
        pictureElement.src = "";
    });

    connection.start().then(function () {
        console.log("connected");
    }).catch(function (err) {
        return console.log(err.toString());
    });

    //function fadeOutText(target) {
    //    var fadeEffect = setInterval(function () {
    //        if (!target.style.opacity) {
    //            target.style.opacity = 1;
    //        }
    //        if (target.style.opacity > 0) {
    //            target.style.opacity -= 0.1;
    //        } else {
    //            clearInterval(fadeEffect);
    //            pictureElement.src = "";
    //        }
    //    }, 200);
    //}
})
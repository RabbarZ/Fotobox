"use strict";


document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder().withUrl("/fotoboxHub").build();

    const pictureElement = document.querySelector("#picture");
    const countdownElement = document.querySelector("#countdown");

    connection.on("changeCountdown", function (number) {
        if (number == 0) {
            countdownElement.innerHTML = "Foto!!";
        }
        else if (number == -1) {
            //setTimeout(function () {
            //}, 500);
            countdownElement.innerHTML = "";
        }
        else {
            countdownElement.innerHTML = number;
        }
    });

    connection.on("reloadPicture", function (path) {
        pictureElement.src = path;
    });

    connection.on("reset", function (text) {
        // countdownElement.innerHTML = text;
        // countdownElement.style.opacity = 0;
        // fadeOutText(countdownElement);

        pictureElement.src = "";
    });

    connection.start().then(function () {
        console.log("connected");
    }).catch(function (err) {
        return console.log(err.toString());
    });

    function fadeOutText(target) {
        var fadeEffect = setInterval(function () {
            if (!target.style.opacity) {
                target.style.opacity = 1;
            }
            if (target.style.opacity > 0) {
                target.style.opacity -= 0.1;
            } else {
                clearInterval(fadeEffect);
                pictureElement.src = "";
            }
        }, 200);
    }
})